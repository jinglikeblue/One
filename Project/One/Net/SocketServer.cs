using One.Core;
using System;
using System.Net;
using System.Net.Sockets;

namespace One.Net
{
    public class SocketServer: IServer
    {
        /// <summary>
        /// 监听的端口
        /// </summary>
        Socket _socket;

        public void Start(string host, int port)
        {
            Console.WriteLine(string.Format("Start Lisening {0}:{1}", host, port));

            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(host), port));
            _socket.Listen(100);
            StartAccept(null);
        }

        /// <summary>
        /// 开始接受链接
        /// </summary>
        /// <param name="e"></param>
        void StartAccept(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else
            {                
                e.AcceptSocket = null;
            }
            
            bool willRaiseEvent = _socket.AcceptAsync(e);
            if (!willRaiseEvent)
            {
                ProcessAccept(e);
            }
        }

        /// <summary>
        /// 接收到连接完成的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        void ProcessAccept(SocketAsyncEventArgs e)
        {
            //添加一个成功链接
            ClientManager.Enter(e.AcceptSocket);

            StartAccept(e);
        }
    }
}
