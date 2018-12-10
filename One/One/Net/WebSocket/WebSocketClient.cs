using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using One.Data;
using One.Protocol;

namespace One.Net
{
    public class WebSocketClient : TcpSocketClient, IRemoteProxy
    {
        /// <summary>
        /// 协议是否已升级
        /// </summary>
        public bool IsUpgrade { get; internal set; } = false;


        public WebSocketClient():base(new WebSocketProtocolProcess())
        {
            base.protocolProcess.SetSender(this);
        }

        protected override void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= OnConnectCompleted;
            if (null == e.ConnectSocket)
            {
                DispatchConnectFailEvent();
                return;
            }

            _socket = e.ConnectSocket;            
            StartReceive();
            RequestUpgrade();
        }

        protected override void ProcessReceivedData()
        {
            if(false == IsUpgrade)
            {
                bool isUpgradeSuccess = UpgradeResponse();
                if(isUpgradeSuccess)
                {                    
                    DispatchConnectSuccessEvent();
                }
                else
                {
                    DispatchConnectFailEvent();
                }
            }
            else
            {
                base.ProcessReceivedData();
            }
        }

        public void Send(string content)
        {
            var data = Encoding.UTF8.GetBytes(content);
            var sendData = (base.protocolProcess as WebSocketProtocolProcess).CreateDataFrame(data);
            Send(sendData);
        }        

        void RequestUpgrade()
        {
            //生成升级协议确认KEY
            string requestValue = "One WebSocket";
            byte[] bytes = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(requestValue));
            string base64Value = Convert.ToBase64String(bytes);

            //构建升级回复协议
            var builder = new StringBuilder();
            builder.Append("HTTP/1.1 101 Switching Protocols\r\n");
            builder.Append("Upgrade: websocket\r\n");
            builder.Append("Connection: Upgrade\r\n");
            builder.Append("Sec-WebSocket-Version: 13\r\n");
            builder.AppendFormat("Sec-WebSocket-Key: {0}\r\n", base64Value);
            builder.Append("\r\n");
            string requestData = builder.ToString();

            byte[] responseBytes = Encoding.ASCII.GetBytes(requestData);
            //请求升级
            Send(responseBytes);
        }

        /// <summary>
        /// 检查收到的协议是否是升级协议
        /// </summary>
        bool UpgradeResponse()
        {
            //获取客户端发来的升级协议KEY
            ByteArray ba = new ByteArray(_receiveBuffer, _bufferAvailable);
            string clientRequest = ba.ReadStringBytes(Encoding.ASCII, ba.ReadEnableSize);
            string[] datas = clientRequest.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);            
            try
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    if (datas[i].Contains("Sec-WebSocket-Accept"))
                    {
                        IsUpgrade = true;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                
            }

            return IsUpgrade;
        }
    }
}
