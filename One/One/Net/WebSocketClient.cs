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
    public class WebSocketClient : TcpClient
    {
        /// <summary>
        /// 客户端请求升级发送的KEY
        /// </summary>
        const string CLIENT_UPGRADE_REQEUST_KEY = "Sec-WebSocket-Key";

        /// <summary>
        /// 协议升级为WebSocket使用的GUID
        /// </summary>
        const string WEB_SOCKET_UPGRADE_GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        bool _isUpgraded = false;

        public WebSocketClient(Socket clientSocket, IProtocolProcess protocolProcess, ushort bufferSize) : base(clientSocket, protocolProcess, bufferSize)
        {
            
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
                    used = ProcessPayloadData();
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
        /// 处理WS携带的数据
        /// </summary>
        int ProcessPayloadData()
        {
            ByteArray ba = new ByteArray(_buffer, _bufferAvailable);

            return 0;
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
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                value = null;
            }

            if(null == value)
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
    }
}
