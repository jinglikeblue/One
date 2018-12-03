﻿using One.Core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace One.Net
{
    public class TcpSocketServer
    {
        /// <summary>
        /// 新的客户端进入的事件（非线程安全）
        /// </summary>
        public event EventHandler<TcpClient> onClientEnterHandler;

        /// <summary>
        /// 客户端退出的事件（非线程安全）
        /// </summary>
        public event EventHandler<TcpClient> onClientExitHandler;

        /// <summary>
        /// 监听的端口
        /// </summary>
        Socket _socket;

        int _clientCount = 0;
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
        ushort _bufferSize;

        /// <summary>
        /// 启动Socket服务
        /// </summary>
        /// <param name="host">监听的地址</param>
        /// <param name="port">坚挺的端口</param>
        /// <param name="bufferSize">每一个连接的缓冲区大小</param>
        public void Start(string host, int port, ushort bufferSize)
        {            
            Console.WriteLine(string.Format("Start Lisening {0}:{1}", host, port));

            _bufferSize = bufferSize;
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
            TcpClient client = new TcpClient(this, clientSocket, _bufferSize);
            onClientEnterHandler?.Invoke(this, client);
        }

        internal void Exit(TcpClient client)
        {
            Interlocked.Decrement(ref _clientCount);
            onClientExitHandler?.Invoke(this, client);
        }
    }
}
