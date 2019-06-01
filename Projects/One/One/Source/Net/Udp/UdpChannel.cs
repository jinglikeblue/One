using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace One
{
    public class UdpChannel
    {
        SocketAsyncEventArgs _sendEA;

        /// <summary>
        /// 数据发送队列
        /// </summary>
        protected List<ArraySegment<byte>> _sendBufferList = new List<ArraySegment<byte>>();


        public event ReceiveDataEvent onReceiveData;

        /// <summary>
        /// 是否正在发送数据
        /// </summary>
        bool _isSending = false;

        /// <summary>
        /// 远端地址
        /// </summary>
        public EndPoint RemoteEndPoint { get; private set; }

        UdpServer _server;

        public UdpChannel(UdpServer server, EndPoint remoteEndPoint)
        {
            _server = server;
            RemoteEndPoint = remoteEndPoint;
        }

        private void ProcessSend(SocketAsyncEventArgs e)
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
            _sendEA.RemoteEndPoint = RemoteEndPoint;
            _sendEA.Completed += OnAsyncEventCompleted;
            _sendBufferList.Add(new ArraySegment<byte>(bytes));

            SendBufferList();
        }

        private void OnAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            _server.Tsa.AddToSyncAction(() =>
            {
                ProcessSend(e);
            });
        }

        protected void SendBufferList()
        {
            //如果没有在发送状态，则调用发送
            if (_isSending || _sendBufferList.Count == 0)
            {
                return;
            }

            _isSending = true;
            _sendEA.BufferList = _sendBufferList.ToArray();

            _sendBufferList.Clear();

            bool willRaiseEvent = _server.Socket.SendToAsync(_sendEA);
            if (!willRaiseEvent)
            {
                ProcessSend(_sendEA);
            }
        }
    }
}
