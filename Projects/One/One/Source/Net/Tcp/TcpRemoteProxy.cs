using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace One
{
    public class TcpReomteProxy : IRemoteProxy
    {
        SocketAsyncEventArgs _receiveEA;

        SocketAsyncEventArgs _sendEA;

        /// <summary>
        /// 客户端连接关闭事件
        /// </summary>
        Action<TcpReomteProxy> _onShutdown;

        protected Socket _clientSocket;

        protected byte[] _buffer;

        /// <summary>
        /// 数据发送队列
        /// </summary>
        protected List<ArraySegment<byte>> _sendBufferList = new List<ArraySegment<byte>>();

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
        public IProtocolProcess protocolProcess { get; internal set; }

        /// <summary>
        /// 是否客户端已关闭
        /// </summary>
        public bool isClosed { get; private set; } = false;

        /// <summary>
        /// 期望关闭
        /// </summary>
        bool _wantShutdown = false;

        public TcpReomteProxy(Socket clientSocket, IProtocolProcess protocolProcess, int bufferSize, Action<TcpReomteProxy> onShutDown)
        {
            _clientSocket = clientSocket;
            _onShutdown = onShutDown;
            _buffer = new byte[bufferSize];

            this.protocolProcess = protocolProcess;
            protocolProcess.SetSender(this);
            _receiveEA = new SocketAsyncEventArgs();
            _sendEA = new SocketAsyncEventArgs();
            _receiveEA.Completed += OnIOCompleted;
            _sendEA.Completed += OnIOCompleted;

            StartReceive();
        }

        private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException(string.Format("Wrong last operation : {0}", e.LastOperation));
            }
        }

        protected void StartReceive()
        {
            if (IsFade)
            {
                return;
            }

            _receiveEA.SetBuffer(_buffer, _bufferAvailable, _buffer.Length - _bufferAvailable);

            bool willRaiseEvent = _clientSocket.ReceiveAsync(_receiveEA);
            if (!willRaiseEvent)
            {
                ProcessReceive(_receiveEA);
            }
        }

        /// <summary>
        /// 处理接收到的消息（多线程事件）
        /// </summary>
        /// <param name="e"></param>
        virtual protected void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                _bufferAvailable += e.BytesTransferred;

                //协议处理器处理协议数据
                int used = protocolProcess.Unpack(_buffer, _bufferAvailable);

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

        virtual public void Send(byte[] bytes)
        {
            if (IsFade)
            {
                return;
            }

            _sendBufferList.Add(new ArraySegment<byte>(bytes));

            SendBufferList();
        }

        protected void SendBufferList()
        {
            if (IsFade)
            {               
                return;
            }

            lock (this)
            {
                //如果没有在发送状态，则调用发送
                if (_isSending || _sendBufferList.Count == 0)
                {
                    return;
                }

                _isSending = true;
                _sendEA.BufferList = _sendBufferList.ToArray();

                _sendBufferList.Clear();

                bool willRaiseEvent = _clientSocket.SendAsync(_sendEA);
                if (!willRaiseEvent)
                {
                    ProcessSend(_sendEA);
                }
            }
        }

        /// <summary>
        /// 处理发送的消息回调（多线程事件）
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                _isSending = false;
                //尝试一次发送
                SendBufferList();
            }
            else
            {
                Shutdown();
            }
        }

        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        protected void Shutdown()
        {
            if (null != _clientSocket)
            {
                try
                {
                    _clientSocket.Shutdown(SocketShutdown.Send);
                }
                catch (Exception) { }
                _clientSocket.Close();
                _clientSocket = null;
                _buffer = null;
                _receiveEA.Dispose();
                _receiveEA = null;
                _sendEA.Dispose();
                _sendEA = null;
                isClosed = true;
                _wantShutdown = false;

                _onShutdown.Invoke(this);
                
            }
        }

        /// <summary>
        /// 是否关闭，进行检查，如果返回true，则表示该远端代理结束
        /// </summary>
        bool IsFade
        {
            get
            {
                if (_wantShutdown)
                {
                    Shutdown();
                    return true;
                }

                if (null == _clientSocket)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 关闭客户端
        /// </summary>
        public void Close()
        {
            _wantShutdown = true;
        }
    }
}
