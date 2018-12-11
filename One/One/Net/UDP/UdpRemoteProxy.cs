using One.Data;
using One.Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace One.Net
{
    public class UdpRemoteProxy : IRemoteProxy
    {
        SocketAsyncEventArgs _sendEA;

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

        EndPoint _remoteEndPoint;

        Socket _socket;

        public ByteArray ba;

        public UdpRemoteProxy(Socket socket, EndPoint remoteEndPoint, IProtocolProcess protocolProcess)
        {
            _socket = socket;
            _remoteEndPoint = remoteEndPoint;
            this.protocolProcess = protocolProcess;
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                _isSending = false;
                //尝试一次发送
                SendBufferList();
            }
        }

        virtual public void Send(byte[] bytes)
        {
            _sendEA = new SocketAsyncEventArgs();
            _sendEA.RemoteEndPoint = _remoteEndPoint;
            _sendEA.Completed += OnSendCompleted;
            _sendBufferList.Add(new ArraySegment<byte>(bytes));

            SendBufferList();
        }

        protected void SendBufferList()
        {
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
        /// 关闭客户端
        /// </summary>
        public void Close()
        {
            _socket = null;
            _remoteEndPoint = null;
        }
    }
}
