using One.Data;
using One.Net;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace One.Protocol
{
    /// <summary>
    /// WebSocket协议处理器
    /// </summary>
    public sealed class WebSocketProtocolProcess : IProtocolProcess
    {
        /// <summary>
        /// 客户端请求升级发送的KEY
        /// </summary>
        const string CLIENT_UPGRADE_REQEUST_KEY = "Sec-WebSocket-Key";

        /// <summary>
        /// 协议升级为WebSocket使用的GUID
        /// </summary>
        const string WEB_SOCKET_UPGRADE_GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

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
            if(0 == _pbList.Count)
            {
                return;
            }

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
            if(false == _sender.isUpgraded)
            {
                //首先升级协议
                return Upgrade(buf, available);
            }

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

        /// <summary>
        /// 升级协议为WebSocket协议
        /// </summary>
        int Upgrade(byte[] buffer, int bufferAvailable)
        {
            //获取客户端发来的升级协议KEY
            ByteArray ba = new ByteArray(buffer, bufferAvailable);
            string clientRequest = ba.ReadStringBytes(Encoding.ASCII, ba.ReadEnableSize);
            string[] datas = clientRequest.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string value = null;
            try
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    if (datas[i].Contains(CLIENT_UPGRADE_REQEUST_KEY))
                    {
                        string[] keyValue = datas[i].Split(':');
                        value = keyValue[1].Trim();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                value = null;
            }

            if (null == value)
            {
                _sender.Close();
                return 0;
            }

            //生成升级协议确认KEY
            string responseValue = value + WEB_SOCKET_UPGRADE_GUID;
            byte[] bytes = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(responseValue));
            string base64Value = Convert.ToBase64String(bytes);

            //构建升级回复协议
            var builder = new StringBuilder();
            builder.Append("HTTP/1.1 101 Switching Protocols\r\n");
            builder.Append("Upgrade: websocket\r\n");
            builder.Append("Connection: Upgrade\r\n");
            builder.AppendFormat("Sec-WebSocket-Accept: {0}\r\n", base64Value);
            builder.Append("\r\n");
            string responseData = builder.ToString();

            byte[] responseBytes = Encoding.ASCII.GetBytes(responseData);
            //回执升级协议
            _sender.Send(responseBytes);

            //Console.WriteLine("response:\r\n {0}", responseData);
            _sender.isUpgraded = true;
            return bufferAvailable;
        }
    }
}
