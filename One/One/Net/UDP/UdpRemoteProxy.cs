using One.Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace One.Net
{
    public class UdpRemoteProxy:IRemoteProxy
    {       
        SocketAsyncEventArgs _sendEA;

        /// <summary>
        /// 客户端连接关闭事件
        /// </summary>
        internal event EventHandler<UdpRemoteProxy> onShutdown;

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

        EndPoint _remoteEndPoint;

        Socket _socket;

        public UdpRemoteProxy(Socket socket, EndPoint remoteEndPoint, IProtocolProcess protocolProcess, int bufferSize)
        {
            _socket = socket;
            _buffer = new byte[bufferSize];
            //_socket = new Socket(SocketType.Dgram, ProtocolType.Udp);            
            _remoteEndPoint = remoteEndPoint;
            _sendEA = new SocketAsyncEventArgs();
            _sendEA.RemoteEndPoint = remoteEndPoint;
            _sendEA.Completed += OnSendCompleted;            
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
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
                _sendEA.BufferList = _sendBufferList;

                _sendBufferList.Clear();

                bool willRaiseEvent = _socket.SendToAsync(_sendEA);
                if (!willRaiseEvent)
                {
                    OnSendCompleted(null, _sendEA);
                }
            }
        }

        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        protected void Shutdown()
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
                _buffer = null;
                _sendEA.Dispose();
                _sendEA = null;
                isClosed = true;
                _wantShutdown = false;

                onShutdown?.Invoke(this, this);
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

                if (null == _socket)
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
