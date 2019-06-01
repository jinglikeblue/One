using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace One
{
    public class TcpClient : IRemoteProxy
    {
        /// <summary>
        /// 连接成功事件(多线程事件）
        /// </summary>
        public event Action<IRemoteProxy> onConnectSuccess;

        /// <summary>
        /// 连接断开事件(多线程事件）
        /// </summary>
        public event Action<IRemoteProxy> onDisconnect;

        /// <summary>
        /// 连接失败事件(多线程事件）
        /// </summary>
        public event Action<IRemoteProxy> onConnectFail;

        /// <summary>
        /// 收到数据
        /// </summary>
        public event ReceiveDataEvent onReceiveData;

        /// <summary>
        /// 接收数据的SocketAsyncEventArgs
        /// </summary>
        SocketAsyncEventArgs _receiveEA;

        /// <summary>
        /// 发送数据的SocketAsyncEventArgs
        /// </summary>
        SocketAsyncEventArgs _sendEA;

        /// <summary>
        /// 线程同步器，将异步方法同步到调用Refresh的线程中
        /// </summary>
        ThreadSyncActions _tsa = new ThreadSyncActions();

        /// <summary>
        /// Socket对象
        /// </summary>
        protected Socket _socket;

        /// <summary>
        /// 接收数据的缓冲区
        /// </summary>
        protected byte[] _receiveBuffer;

        /// <summary>
        /// 数据发送队列
        /// </summary>
        List<ArraySegment<byte>> _sendBufferList = new List<ArraySegment<byte>>();

        /// <summary>
        /// 是否正在发送数据
        /// </summary>
        bool _isSending = false;

        /// <summary>
        /// 缓冲区可用字节长度
        /// </summary>
        protected int _bufferAvailable = 0;

        /// <summary>
        /// 协议处理器
        /// </summary>
        protected IProtocolProcess protocolProcess;

        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// 主机端口
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 是否已连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected
        {
            get
            {
                if (null != _socket)
                {
                    return true;
                }
                return false;
            }
        }

        public TcpClient()
        {
            InitProtocolProcess();
        }

        virtual protected void InitProtocolProcess()
        {
            this.protocolProcess = new TcpProtocolProcess();
        }

        /// <summary>
        /// 连接指定的服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="bufferSize"></param>
        public void Connect(string host, int port, ushort bufferSize)
        {
            _receiveBuffer = new byte[bufferSize];
            _sendEA = new SocketAsyncEventArgs();
            _sendEA.Completed += OnSendCompleted;
            _receiveEA = new SocketAsyncEventArgs();
            _receiveEA.Completed += OnReceiveCompleted;

            Host = host;
            Port = port;

            Reconnect();
        }

        public void Reconnect()
        {
            Close();

            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(Host), Port);
            Socket socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            SocketAsyncEventArgs connectEA = new SocketAsyncEventArgs();
            connectEA.RemoteEndPoint = ipe;
            connectEA.Completed += OnConnectCompleted;
            if (!socket.ConnectAsync(connectEA))
            {
                OnConnectCompleted(null, connectEA);
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            _tsa.RunSyncActions();
        }

        /// <summary>
        /// 断开客户端连接，并且不触发任何事件
        /// </summary>
        public void CloseSilently()
        {
            onDisconnect = null;
            Close();
        }

        /// <summary>
        /// 断开客户端连接
        /// </summary>
        public void Close()
        {
            if (null != _socket)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Send);
                }
                catch (Exception) { }
                _socket.Close();
                _socket = null;
                _receiveBuffer = null;
                _bufferAvailable = 0;

                onDisconnect?.Invoke(this);
            }
        }

        protected void DispatchConnectFailEvent()
        {
            onConnectFail?.Invoke(this);
        }

        protected void DispatchConnectSuccessEvent()
        {
            onConnectSuccess?.Invoke(this);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        public virtual void Send(byte[] bytes)
        {
            if (false == IsConnected)
            {
                return;
            }

            var protocolData = protocolProcess.Pack(bytes);
            _sendBufferList.Add(new ArraySegment<byte>(protocolData));

            SendBufferList();            
        }

        void SendBufferList()
        {
            //如果没有在发送状态，则调用发送
            if (_isSending || _sendBufferList.Count == 0)
            {
                return;
            }

            _isSending = true;
            _sendEA.BufferList = _sendBufferList.ToArray();

            _sendBufferList.Clear();

            if (!_socket.SendAsync(_sendEA))
            {
                OnSendCompleted(null, _sendEA);
            }
        }

        /// <summary>
        /// 处理接收到的数据
        /// </summary>
        protected virtual void ProcessReceivedData()
        {
            //协议处理器处理协议数据
            int used = protocolProcess.Unpack(_receiveBuffer, _bufferAvailable, OnReceiveData);

            if (used > 0)
            {
                _bufferAvailable = _bufferAvailable - used;
                if (0 != _bufferAvailable)
                {
                    //将还没有使用的数据移动到数据开头
                    byte[] newBytes = new byte[_receiveBuffer.Length];
                    Array.Copy(_receiveBuffer, used, newBytes, 0, _bufferAvailable);
                    _receiveBuffer = newBytes;
                }
            }
        }

        /// <summary>
        /// 收到数据时触发
        /// </summary>
        /// <param name="protocolData"></param>
        private void OnReceiveData(byte[] protocolData)
        {
            onReceiveData?.Invoke(this, protocolData);
        }

        /// <summary>
        /// 开始接受数据
        /// </summary>
        protected void StartReceive()
        {
            if (false == IsConnected)
            {
                return;
            }

            _receiveEA.SetBuffer(_receiveBuffer, _bufferAvailable, _receiveBuffer.Length - _bufferAvailable);

            if (!_socket.ReceiveAsync(_receiveEA))
            {
                OnReceiveCompleted(null, _receiveEA);
            }
        }

        /// <summary>
        /// 连接完成（多线程事件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            _tsa.AddToSyncAction(() =>
            {
                e.Completed -= OnConnectCompleted;
                if (null == e.ConnectSocket)
                {
                    DispatchConnectFailEvent();
                    return;
                }

                _socket = e.ConnectSocket;
                DispatchConnectSuccessEvent();
                StartReceive();
            });
        }

        /// <summary>
        /// 处理接收到的消息（多线程事件）
        /// </summary>        
        void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            _tsa.AddToSyncAction(() =>
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    _bufferAvailable += e.BytesTransferred;

                    ProcessReceivedData();

                    StartReceive();
                }
                else
                {
                    Close();
                }
            });
        }

        /// <summary>
        /// 消息发送完毕（多线程事件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            _tsa.AddToSyncAction(() =>
            {
                _isSending = false;
                if (e.SocketError == SocketError.Success)
                {                    
                    //尝试一次发送
                    SendBufferList();
                }
                else
                {
                    Close();
                }
            });
        }
    }
}
