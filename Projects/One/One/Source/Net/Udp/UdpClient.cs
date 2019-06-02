using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace One
{
    public class UdpClient
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
        /// 收到UDP数据的事件（多线程事件）
        /// </summary>
        public event Action<UdpClient, byte[]> onReceiveData;

        /// <summary>
        /// 是否正在发送数据
        /// </summary>
        bool _isSending = false;

        public string RemoteHost { get; private set; }
        public int RemotePort { get; private set; }
        public int LocalPort { get; private set; }

        IPEndPoint _remoteEndPoint;

        IPEndPoint _localEndPoint;

        /// <summary>
        /// 线程同步器，将异步方法同步到调用Refresh的线程中
        /// </summary>
        ThreadSyncActions _tsa = new ThreadSyncActions();

        public UdpClient()
        {
          
        }


        public void Bind(string remoteHost, int remotePort, int localPort, ushort bufferSize)
        {
            _receiveBuffer = new byte[bufferSize];
            _sendEA = new SocketAsyncEventArgs();
            _sendEA.Completed += OnAsyncEventCompleted;
            _receiveEA = new SocketAsyncEventArgs();
            _receiveEA.Completed += OnAsyncEventCompleted;

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
        /// 异步事件完成（多线程事件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            _tsa.AddToSyncAction(() => {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.ReceiveFrom:
                        ProcessReceive(e);
                        break;
                    case SocketAsyncOperation.SendTo:
                        ProcessSend(e);
                        break;
                    default:
                        break;
                }
            });
        }

        public void Refresh()
        {
            _tsa.RunSyncActions();
        }

        /// <summary>
        /// 开始接受数据
        /// </summary>
        protected void StartReceive()
        {
            _receiveEA.SetBuffer(_receiveBuffer, 0, _receiveBuffer.Length);

            if (!_socket.ReceiveFromAsync(_receiveEA))
            {
                ProcessReceive(_receiveEA);
            }
        }

        /// <summary>
        /// 处理接收到的消息
        /// </summary>        
        void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                byte[] data = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, data, e.BytesTransferred);
                onReceiveData?.Invoke(this, data);
                StartReceive();
            }
            else
            {
                //Close();
            }
        }

        private void OnReceiveData(byte[] protocolData)
        {
            
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {
            if (null == _socket)
            {
                return;
            }


            _sendBufferList.Add(new ArraySegment<byte>(bytes));

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

            if (!_socket.SendToAsync(_sendEA))
            {
                ProcessSend(_sendEA);
            }
        }

        void ProcessSend(SocketAsyncEventArgs e)
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
}
