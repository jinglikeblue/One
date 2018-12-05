using One.Protocol;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace One.Net
{
    public class TcpSocketClient
    {
        /// <summary>
        /// 连接成功事件(多线程事件）
        /// </summary>
        public event EventHandler<TcpSocketClient> onConnectSuccess;

        /// <summary>
        /// 连接断开事件(多线程事件）
        /// </summary>
        public event EventHandler<TcpSocketClient> onDisconnect;

        /// <summary>
        /// 连接失败事件(多线程事件）
        /// </summary>
        public event EventHandler<TcpSocketClient> onConnectFail;

        SocketAsyncEventArgs _receiveEA;
        SocketAsyncEventArgs _sendEA;        
        Socket _socket;

        byte[] _buffer;

        /// <summary>
        /// 缓冲区可用字节长度
        /// </summary>
        int _bufferAvailable = 0;

        /// <summary>
        /// 协议处理器
        /// </summary>
        public IProtocolProcess protocolProcess { get; internal set; }

        public string Host { get; private set; }
        public int Port { get; private set; }

        /// <summary>
        /// 是否已连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            if(null != _socket)
            {
                return true;
            }
            return false;
        }

        public TcpSocketClient(IProtocolProcess protocolProcess)
        {
            this.protocolProcess = protocolProcess;
        }
        
        /// <summary>
        /// 连接指定的服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="bufferSize"></param>
        public void Connect(string host, int port, ushort bufferSize)
        {
            Disconnect();

            _sendEA = new SocketAsyncEventArgs();
            _sendEA.Completed += OnSendCompleted;
            _receiveEA = new SocketAsyncEventArgs();
            _receiveEA.Completed += OnReceiveCompleted;

            Host = host;
            Port = port;

            Reconnect();
        }

        /// <summary>
        /// 断开客户端连接
        /// </summary>
        public void Disconnect()
        {
            if(null != _socket)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Send);
                }
                catch (Exception) { }
                _socket.Close();
                _socket = null;
                _buffer = null;

                onDisconnect?.Invoke(this, this);
            }
        }

        public void Reconnect()
        {
            Disconnect();       
            
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(Host), Port);
            Socket socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);            
            SocketAsyncEventArgs connectEA = new SocketAsyncEventArgs();
            connectEA.RemoteEndPoint = ipe;
            connectEA.Completed += OnConnectCompleted;
            if(!socket.ConnectAsync(connectEA))
            {
                OnConnectCompleted(null, connectEA);
            }
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= OnConnectCompleted;
            if(null == e.AcceptSocket)
            {
                onConnectFail?.Invoke(this, this);
                return;
            }

            _socket = e.ConnectSocket;
            onConnectSuccess?.Invoke(this, this);
            StartReceive();
        }

        /// <summary>
        /// 开始接受数据
        /// </summary>
        void StartReceive()
        {
            _receiveEA.SetBuffer(_buffer, _bufferAvailable, _buffer.Length - _bufferAvailable);

            if (!_socket.ReceiveAsync(_receiveEA))
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

                //协议处理器处理协议数据
                int used = protocolProcess.Unpack(_buffer, _bufferAvailable);                

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
                Disconnect();
            }
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

            _sendEA.SetBuffer(bytes, 0, bytes.Length);

            if(!_socket.SendAsync(_sendEA))
            {
                OnSendCompleted(null, _sendEA);
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            if(e.SocketError != SocketError.Success)
            {
                Disconnect();
            }
        }
    }
}
