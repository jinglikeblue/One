using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace One
{
    /// <summary>
    /// 提供基于TCP协议的套接字服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TcpServer<T> where T : IProtocolProcess, new()
    {
        /// <summary>
        /// 新的客户端进入的事件（非线程安全）
        /// </summary>
        public event EventHandler<IRemoteProxy> onClientEnterHandler;

        /// <summary>
        /// 客户端退出的事件（非线程安全）
        /// </summary>
        public event EventHandler<IRemoteProxy> onClientExitHandler;

        /// <summary>
        /// 监听的端口
        /// </summary>
        protected Socket _socket;

        protected int _clientCount = 0;
        /// <summary>
        /// 已连接的客户端总数
        /// </summary>
        public int ClientCount
        {
            get
            {
                return _clientCount;
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
            Console.WriteLine(string.Format("Start Lisening {0}:{1}", IPAddress.Any, port));

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
        /// 接收到连接完成的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        void ProcessAccept(SocketAsyncEventArgs e)
        {
            //添加一个成功链接
            Enter(e.AcceptSocket);

            StartAccept(e);
        }

        void Enter(Socket clientSocket)
        {
            Interlocked.Increment(ref _clientCount);
            TcpReomteProxy client = CreateRemoteProxy(clientSocket, _bufferSize, OnClientShutdown);
            DispatchRemoteProxyEnterEvent(client);
            Console.WriteLine("Thread [{0}]: enter total:{1}", Thread.CurrentThread.ManagedThreadId, _clientCount);            
        }

        //object clientExitLock = new object();
        private void OnClientShutdown(TcpReomteProxy client)
        {
            //lock (clientExitLock)
            //{
                Interlocked.Decrement(ref _clientCount);
                DispatchRemoteProxyExitEvent(client);
                Console.WriteLine("Thread [{0}]: exit total:{1}", Thread.CurrentThread.ManagedThreadId, _clientCount);
            //}
        }

        protected virtual TcpReomteProxy CreateRemoteProxy(Socket clientSocket, int bufferSize, Action<TcpReomteProxy> onShutdown)
        {
            return new TcpReomteProxy(clientSocket, new T(), bufferSize, onShutdown);
        }

        protected void DispatchRemoteProxyEnterEvent(IRemoteProxy remoteProxy)
        {
            onClientEnterHandler?.Invoke(this, remoteProxy);
        }

        protected void DispatchRemoteProxyExitEvent(IRemoteProxy remoteProxy)
        {
            onClientExitHandler?.Invoke(this, remoteProxy);
        }
    }
}
