using One.Data;
using One.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace One.Protocol
{
    /// <summary>
    /// WebSocket协议处理器
    /// </summary>
    public sealed class WebSocketProtocolProcess : IProtocolProcess
    {
        /// <summary>
        /// 负载数据内容
        /// </summary>
        internal enum EOpcode
        {
            /// <summary>
            /// 继续帧
            /// </summary>
            CONTINUE = 0,
            /// <summary>
            /// 文本帧
            /// </summary>
            TEXT = 1,
            /// <summary>
            /// 二进制帧
            /// </summary>
            BYTE = 2,
            /// <summary>
            /// 连接关闭
            /// </summary>
            CLOSE = 8,
            PING = 9,
            PONG = 10,
        }

        List<byte[]> _pbList = new List<byte[]>();

        WebSocketRemoteProxy _sender;

        public void SetSender(IRemoteProxy sender)
        {
            _sender = sender as WebSocketRemoteProxy;
        }

        /// <summary>
        /// 用传入的委托方法来接收协议处理器收集到的协议（线程安全）
        /// </summary>
        /// <param name="onReceiveProtocol"></param>
        public void ReceiveProtocols(Action<byte[]> onReceiveProtocol)
        {
            List<byte[]> pbList = new List<byte[]>();

            lock (_pbList)
            {
                pbList.AddRange(_pbList);
                _pbList.Clear();
            }

            for (int i = 0; i < pbList.Count; i++)
            {
                onReceiveProtocol(pbList[i]);
            }
        }

        public int Unpack(byte[] buf, int available)
        {
            ByteArray ba = new ByteArray(buf, available, false);
            int used = 0;
            Unpack(ba, ref used);
            return used;
        }

        void Unpack(ByteArray ba, ref int used)
        {           
            if(ba.ReadEnableSize < 2 * ByteArray.BYTE_SIZE)
            {
                //数据存在半包问题
                return;
            }

            int startPos = ba.Pos;

            //获取第一个byte
            byte byte1 = ba.ReadByte();
            bool fin = (byte1 & 128) == 128 ? true : false;
            var rsv123 = (byte1 & 112);
            var opcode = (EOpcode)(byte1 & 15);

            //获取第二个byte
            byte byte2 = ba.ReadByte();
            bool mask = (byte2 & 128) == 128 ? true : false;
            var payloadLen = (byte2 & 127);

            int dataSize = 0;
            switch (payloadLen)
            {
                case 127:
                    if (ba.ReadEnableSize < 2 * ByteArray.ULONG_SIZE)
                    {
                        //数据存在半包问题
                        return;
                    }
                    dataSize = (int)ba.ReadULong();
                    break;
                case 126:
                    if (ba.ReadEnableSize < 2 * ByteArray.USHORT_SIZE)
                    {
                        //数据存在半包问题
                        return;
                    }
                    dataSize = ba.ReadUShort();
                    break;
                default:
                    dataSize = payloadLen;                    
                    break;
            }

            byte[] maskKeys = null;
            if (mask)
            {
                if (ba.ReadEnableSize < 4 * ByteArray.BYTE_SIZE)
                {
                    //数据存在半包问题
                    return;
                }

                maskKeys = new byte[4];
                for (int i = 0; i < maskKeys.Length; i++)
                {
                    maskKeys[i] = ba.ReadByte();
                }
            }

            switch (opcode)
            {
                case EOpcode.CONTINUE:
                    //使用率低，暂不处理这种情况
                    break;
                case EOpcode.TEXT:
                case EOpcode.BYTE:
                    if (dataSize > 0)
                    {
                        if (ba.ReadEnableSize < dataSize)
                        {
                            //数据存在半包问题
                            return;
                        }

                        byte[] payloadData = ba.ReadBytes(dataSize);
                        if (mask)
                        {
                            for (int i = 0; i < payloadData.Length; i++)
                            {
                                var maskKey = maskKeys[i % 4];
                                payloadData[i] = (byte)(payloadData[i] ^ maskKey);
                            }
                        }

                        OnReceiveProtocol(payloadData);
                    }
                    break;
                case EOpcode.PING:
                    _sender.SendPong();
                    break;
                case EOpcode.PONG:
                    //忽略
                    break;

                case EOpcode.CLOSE:
                default:
                    //不可识别的操作符
                    _sender.Close();
                    return; //注意这里不是break，是返回！！！                    
            }

            used += ba.Pos - startPos;

            Unpack(ba, ref used);
        }

        /// <summary>
        /// 解包时拿到的数据帧中的应用数据
        /// </summary>
        /// <param name="data"></param>
        public void OnReceiveProtocol(byte[] data)
        {
            lock (_pbList)
            {
                _pbList.Add(data);
            }

            //收到的数据原路返回
            _sender.Send(Pack(data));
        }

        /// <summary>
        /// 将要发送的数据封装为WebSocket通信数据帧。
        /// 默认mask为0
        /// </summary>
        /// <param name="data">发送的数据</param>
        /// <returns>封装好的ws数据帧</returns>
        public byte[] Pack(byte[] data)
        {
            return CreateDataFrame(data);
        }

        /// <summary>
        /// 将要发送的数据封装为WebSocket通信数据帧。
        /// 默认mask为0
        /// </summary>
        /// <param name="data">发送的数据</param>
        /// <param name="isFin">是否是结束帧(默认为true)</param>
        /// <param name="opcode">操作码(默认为TEXT)</param>
        internal byte[] CreateDataFrame(byte[] data, bool isFin = true, EOpcode opcode = EOpcode.TEXT)
        {
            int bufferSize = 10;
            if(null != data)
            {
                bufferSize += data.Length;
            }

            ByteArray ba = new ByteArray(bufferSize, false);

            int b1 = 0;
            if (isFin)
            {
                b1 = b1 | 128;
            }
            b1 = b1 | (int)opcode;
            ba.Write((byte)b1);

            if (data != null)
            {
                if (data.Length > 65535)
                {
                    ba.Write((byte)127);
                    ba.Write((long)data.Length);
                }
                else if (data.Length > 125)
                {
                    ba.Write((byte)126);
                    ba.Write((ushort)data.Length);
                }
                else
                {
                    ba.Write((byte)data.Length);
                }

                ba.Write(data);
            }
            else
            {
                ba.Write((byte)0);
            }
            return ba.GetAvailableBytes();
        }


    }
}
