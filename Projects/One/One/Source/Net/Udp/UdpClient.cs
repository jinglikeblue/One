using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace One
{
    public class UdpClient : IRemoteProxy
    {
        SocketAsyncEventArgs _receiveEA;
        SocketAsyncEventArgs _sendEA;
        protected Socket _socket;

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
        public IProtocolProcess protocolProcess { get; internal set; }

        public string RemoteHost { get; private set; }
        public int RemotePort { get; private set; }
        public int LocalPort { get; private set; }

        IPEndPoint _remoteEndPoint;

        IPEndPoint _localEndPoint;

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

        public UdpClient(IProtocolProcess protocolProcess)
        {
            this.protocolProcess = protocolProcess;            
        }

        public void Bind(string remoteHost, int remotePort, int localPort, ushort bufferSize)
        {
            _receiveBuffer = new byte[bufferSize];
            _sendEA = new SocketAsyncEventArgs();
            _sendEA.Completed += OnSendCompleted;
            _receiveEA = new SocketAsyncEventArgs();
            _receiveEA.Completed += OnReceiveCompleted;

            RemoteHost = remoteHost;
            RemotePort = remotePort;
            LocalPort = LocalPort;
            
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(RemoteHost), RemotePort);
            _localEndPoint = new IPEndPoint(IPAddress.Any, localPort);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(_localEndPoint);

             
            _receiveEA.RemoteEndPoint = _localEndPoint;
            _sendEA.RemoteEndPoint = _remoteEndPoint;
            StartReceive();
        }

        /// <summary>
        /// 开始接受数据
        /// </summary>
        protected void StartReceive()
        {
            if (IsFade)
            {
                return;
            }

            _receiveEA.SetBuffer(_receiveBuffer, 0, _receiveBuffer.Length);

            if (!_socket.ReceiveFromAsync(_receiveEA))
            {
                OnReceiveCompleted(null, _receiveEA);
            }
        }

        /// <summary>
        /// 处理接收到的消息（多线程事件）
        /// </summary>        
        void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                _bufferAvailable += e.BytesTransferred;

                ProcessReceivedData();

                StartReceive();
            }
            else
            {
                //Close();
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

        private void OnReceiveData(byte[] protocolData)
        {
            
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        public virtual void Send(byte[] bytes)
        {
            lock (this)
            {
                if (null == _socket)
                {
                    return;
                }


                _sendBufferList.Add(new ArraySegment<byte>(bytes));

                SendBufferList();
            }
        }

        void SendBufferList()
        {
            //如果没有在发送状态，则调用发送
            if (_isSending || _sendBufferList.Count == 0)
            {
                return;
            }

            _isSending = true;
            _sendEA.BufferList = _sendBufferList;

            _sendBufferList.Clear();

            if (!_socket.SendToAsync(_sendEA))
            {
                OnSendCompleted(null, _sendEA);
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            lock (this)
            {
                if (e.SocketError == SocketError.Success)
                {
                    _isSending = false;
                    //尝试一次发送
                    SendBufferList();
                }
                else
                {
                    //Close();
                }
            }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 是否关闭，进行检查，如果返回true，则表示该远端代理结束
        /// </summary>
        bool IsFade
        {
            get
            {
                if (null == _socket)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
