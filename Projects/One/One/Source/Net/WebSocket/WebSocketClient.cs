using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace One
{
    public class WebSocketClient
    {
        /// <summary>
        /// 协议是否已升级
        /// </summary>
        public bool IsUpgrade { get; internal set; } = false;

        /// <summary>
        /// TCP连接（WebSocket其实就是基于Tcp连接的)
        /// </summary>
        WebSocketChannel _tcpClient;

        /// <summary>
        /// 收到数据
        /// </summary>
        public event Action<TcpClient, byte[]> onReceiveData;

        public WebSocketClient():base()
        {
            
        }

        //protected override void InitProtocolProcess()
        //{
        //    protocolProcess = new WebSocketProtocolProcess();
        //}

        //protected override void OnAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
        //{
        //    e.Completed -= OnAsyncEventCompleted;
        //    if (null == e.ConnectSocket)
        //    {                
        //        DispatchConnectFailEvent();
        //        return;
        //    }

        //    _socket = e.ConnectSocket;            
        //    StartReceive();
        //    RequestUpgrade();
        //}

        //protected override void ProcessReceivedData()
        //{            
        //    if(false == IsUpgrade) //协议没有升级
        //    {
        //        bool isUpgradeSuccess = UpgradeResponse();
        //        if(isUpgradeSuccess)
        //        {
        //            _bufferAvailable = 0;
        //            DispatchConnectSuccessEvent();
        //        }
        //        else
        //        {
        //            DispatchConnectFailEvent();
        //        }
        //    }
        //    else
        //    {
        //        base.ProcessReceivedData();
        //    }
        //}

        //public void SendData(byte[] bytes)
        //{
        //    if (null == _socket)
        //    {
        //        return;
        //    }

        //    var data = protocolProcess.Pack(bytes);

        //    base.Send(data);
        //}

        //public void SendData(string content)
        //{
        //    var bytes = Encoding.UTF8.GetBytes(content);            
        //    SendData(bytes);
        //}        




    }
}
