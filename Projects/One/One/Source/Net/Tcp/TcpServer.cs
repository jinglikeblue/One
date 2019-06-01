using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace One
{
    /// <summary>
    /// 提供基于TCP协议的套接字服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TcpServer
    {
        /// <summary>
        /// 新的客户端进入的事件
        /// </summary>
        public event Action<TcpReomteProxy> onClientEnter;

        /// <summary>
        /// 客户端退出的事件
        /// </summary>
        public event Action<TcpReomteProxy> onClientExit;

        /// <summary>
        /// 线程同步器，将异步方法同步到调用Refresh的线程中
        /// </summary>
        ThreadSyncActions _tsa = new ThreadSyncActions();

        List<TcpReomteProxy> _clientList = new List<TcpReomteProxy>();

        /// <summary>
        /// 监听的端口
        /// </summary>
        protected Socket _socket;

        /// <summary>
        /// 已连接的客户端总数
        /// </summary>
        public int ClientCount
        {
            get
            {
                return _clientList.Count;
            }
        }

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        int _bufferSize;

        /// <summary>
        /// 启动Socket服务
        /// </summary>        
        /// <param name="port">监听的端口</param>
        /// <param name="bufferSize">每一个连接的缓冲区大小</param>
        public void Start(int port, int bufferSize)
        {
            Log.CI(ConsoleColor.DarkGreen, "Start Lisening {0}:{1}", IPAddress.Any, port);

            _bufferSize = bufferSize;
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _socket.Bind(new IPEndPoint(IPAddress.Any, port));
                _socket.Listen(100);
                StartAccept(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Refresh()
        {
            _tsa.RunSyncActions();
            foreach(var client in _clientList)
            {
                client.Refresh();
            }
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
                e.Completed += OnAcceptCompleted;
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
        /// 接收到连接完成的事件（多线程事件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            _tsa.AddToSyncAction(() =>
            {
                ProcessAccept(e);
            });
        }

        void ProcessAccept(SocketAsyncEventArgs e)
        {
            //添加一个成功链接
            Enter(e.AcceptSocket);

            StartAccept(e);
        }

        void Enter(Socket clientSocket)
        {            
            TcpReomteProxy client = new TcpReomteProxy(clientSocket, new TcpProtocolProcess(), _bufferSize);           
            client.onShutdown += OnClientShutdown;
            _clientList.Add(client);            
            Log.I("连接总数:{0}", ClientCount);
            onClientEnter?.Invoke(client);            
        }
        
        private void OnClientShutdown(TcpReomteProxy client)
        {
            client.onShutdown -= OnClientShutdown;
            _clientList.Remove(client);
            Log.I("连接总数:{0}", ClientCount);
            onClientExit?.Invoke(client);
        }
    }
}
