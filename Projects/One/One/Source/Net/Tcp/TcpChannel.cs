using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace One
{
    public class TcpReomteProxy : IChannel
    {
        SocketAsyncEventArgs _receiveEA;

        SocketAsyncEventArgs _sendEA;

        /// <summary>
        /// 线程同步器，将异步方法同步到调用Refresh的线程中
        /// </summary>
        ThreadSyncActions _tsa = new ThreadSyncActions();

        /// <summary>
        /// 收到数据
        /// </summary>
        public event ReceiveDataEvent onReceiveData;

        /// <summary>
        /// 客户端连接关闭事件
        /// </summary>
        internal event Action<TcpReomteProxy> onShutdown;

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
        protected IProtocolProcess protocolProcess;

        /// <summary>
        /// 是否客户端连接中
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (_clientSocket != null && _clientSocket.Connected)
                {
                    return true;
                }
                return false;
            }
        }

        public TcpReomteProxy(Socket clientSocket, int bufferSize)
        {
            _clientSocket = clientSocket;
            _buffer = new byte[bufferSize];

            this.protocolProcess = new TcpProtocolProcess();
            _receiveEA = new SocketAsyncEventArgs();
            _sendEA = new SocketAsyncEventArgs();
            _receiveEA.Completed += OnAsyncEventCompleted;
            _sendEA.Completed += OnAsyncEventCompleted;

            StartReceive();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        internal void Refresh()
        {
            _tsa.RunSyncActions();
        }

        /// <summary>
        /// 异步事件完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            _tsa.AddToSyncAction(() =>
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
            });
        }

        protected void StartReceive()
        {
            if (false == IsConnected)
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
                int used = protocolProcess.Unpack(_buffer, _bufferAvailable, OnReceiveData);

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
                Close();
            }
        }

        /// <summary>
        /// 收到数据时触发
        /// </summary>
        /// <param name="protocolData"></param>
        protected void OnReceiveData(byte[] protocolData)
        {
            onReceiveData?.Invoke(this, protocolData);
        }

        virtual public void Send(byte[] bytes)
        {
            if (false == IsConnected)
            {
                return;
            }

            var protocolData = protocolProcess.Pack(bytes);
            _sendBufferList.Add(new ArraySegment<byte>(protocolData));

            SendBufferList();
        }

        protected void SendBufferList()
        {
            if (false == IsConnected)
            {
                return;
            }

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
                Close();
            }
        }

        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        public void Close()
        {
            if (null != _clientSocket)
            {
                try
                {
                    _clientSocket.Shutdown(SocketShutdown.Send);
                }
                catch
                {
                }

                _clientSocket.Close();
                _clientSocket = null;
                _buffer = null;
                _receiveEA.Dispose();
                _receiveEA = null;
                _sendEA.Dispose();
                _sendEA = null;

                onShutdown?.Invoke(this);
            }
        }
    }
}
