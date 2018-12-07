using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using One.Data;
using One.Protocol;

namespace One.Net
{
    /// <summary>
    /// 连接到服务器的客户端对象
    /// </summary>
    public class WebSocketClient : TcpClient,ISender
    {
        /// <summary>
        /// 负载数据内容
        /// </summary>
        enum EOpcode
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

        /// <summary>
        /// 客户端请求升级发送的KEY
        /// </summary>
        const string CLIENT_UPGRADE_REQEUST_KEY = "Sec-WebSocket-Key";

        /// <summary>
        /// 协议升级为WebSocket使用的GUID
        /// </summary>
        const string WEB_SOCKET_UPGRADE_GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        bool _isUpgraded = false;

        public WebSocketClient(Socket clientSocket, IProtocolProcess protocolProcess, int bufferSize) : base(clientSocket, protocolProcess, bufferSize)
        {
            protocolProcess.SetSender(this);
        }

        public override void Send(byte[] bytes)
        {
            if (null == _clientSocket)
            {
                return;
            }

            _sendBufferList.Add(new ArraySegment<byte>(bytes));

            SendBufferList();
        }

        protected override void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                _bufferAvailable += e.BytesTransferred;

                int used = 0;
                if (false == _isUpgraded)
                {
                    used = Upgrade();
                }
                else
                {
                    used = protocolProcess.Unpack(_buffer, _bufferAvailable);
                }

                //Console.WriteLine("Thread [{0}] : bytes (receive [{1}] , totoal [{2}] , used [{3}] , remains [{4}])", Thread.CurrentThread.ManagedThreadId, e.BytesTransferred, _bufferAvailable, used, _bufferAvailable - used);

                if (used > 0)
                {
                    _bufferAvailable = _bufferAvailable - used;
                    if (0 != _bufferAvailable)
                    {
                        //将还没有使用的数据移动到数据开头
                        byte[] newBytes = new byte[_buffer.Length];
                        Array.Copy(_buffer, used, newBytes, 0, _bufferAvailable);
                        _buffer = newBytes;
                    }
                }

                StartReceive();
            }
            else
            {
                Shutdown();
            }
        }

        /// <summary>
        /// 升级协议为WebSocket协议
        /// </summary>
        int Upgrade()
        {
            //获取客户端发来的升级协议KEY
            ByteArray ba = new ByteArray(_buffer, _bufferAvailable);
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
                Close();
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
            Send(responseBytes);

            //Console.WriteLine("response:\r\n {0}", responseData);
            _isUpgraded = true;
            return _bufferAvailable;
        }



        /// <summary>
        /// 处理WebSocket通信数据帧
        /// </summary>
        int LoadDataFrame()
        {
            ByteArray ba = new ByteArray(_buffer, _bufferAvailable, false);
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
                    dataSize = (int)ba.ReadULong();
                    break;
                case 126:
                    dataSize = ba.ReadUShort();
                    break;
                default:
                    dataSize = payloadLen;
                    break;
            }

            byte[] maskKeys = null;
            if (mask)
            {
                maskKeys = new byte[4];
                for (int i = 0; i < maskKeys.Length; i++)
                {
                    maskKeys[i] = ba.ReadByte();
                }
            }



            switch (opcode)
            {
                case EOpcode.CONTINUE:
                    break;
                case EOpcode.TEXT:                    
                case EOpcode.BYTE:
                    if (dataSize > 0)
                    {
                        byte[] payloadData = ba.ReadBytes(dataSize);
                        if (mask)
                        {
                            for (int i = 0; i < payloadData.Length; i++)
                            {
                                var maskKey = maskKeys[i % 4];
                                payloadData[i] = (byte)(payloadData[i] ^ maskKey);
                            }
                        }

                        //数据发回去
                        Send(CreateDataFrame(payloadData));

                        //var test = new ByteArray(payloadData, payloadData.Length);
                        //string v = test.ReadStringBytes(test.ReadEnableSize);
                    }
                    break;
                case EOpcode.CLOSE:
                    Close();
                    break;
                case EOpcode.PING:
                    SendPong();
                    break;
                case EOpcode.PONG:
                    break;
                default:
                    Console.WriteLine("危险！");
                    Close();
                    break;
            }

            return ba.Pos;
        }

        void SendPong()
        {

        }

        /// <summary>
        /// 将要发送的数据封装为WebSocket通信数据帧。
        /// 默认mask为0
        /// </summary>
        /// <param name="data">发送的数据</param>
        /// <param name="isFin">是否是结束帧(默认为true)</param>
        /// <param name="opcode">操作码(默认为TEXT)</param>
        byte[] CreateDataFrame(byte[] data, bool isFin = true, EOpcode opcode = EOpcode.TEXT)
        {
            ByteArray ba = new ByteArray(data.Length + 20, false);

            int b1 = 0;
            if(isFin)
            {
                b1 = b1 | 128;
            }
            b1 = b1 | (int)opcode;
            ba.Write((byte)b1);            

            if(data.Length > 65535)
            {
                ba.Write((byte)127);
                ba.Write((long)data.Length);
            }
            else if(data.Length > 125)
            {
                ba.Write((byte)126);
                ba.Write((ushort)data.Length);
            }
            else
            {
                ba.Write((byte)data.Length);
            }

            ba.Write(data);            
            return ba.GetAvailableBytes();
        }

    }
}
