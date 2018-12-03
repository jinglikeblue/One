using One.Protocol;
using System;
using System.Net.Sockets;
using System.Threading;

namespace One.Net
{
    public class TcpClient
    {
        SocketAsyncEventArgs _receiveEA;
        SocketAsyncEventArgs _sendEA;
        TcpSocketServer _server;
        Socket _clientSocket;

        byte[] _buffer;
        /// <summary>
        /// 缓冲区可用字节长度
        /// </summary>
        int _bufferAvailable = 0;

        /// <summary>
        /// 协议处理器
        /// </summary>
        public BaseTcpProtocolProcess protocolProcess { get; }

        /// <summary>
        /// 是否客户端已关闭
        /// </summary>
        public bool isClosed { get; private set; } = false;

        public TcpClient(TcpSocketServer server, Socket clientSocket, ushort bufferSize)
        {
            Console.WriteLine("Thread [{0}]: A client has been connected", Thread.CurrentThread.ManagedThreadId);

            _server = server;
            _clientSocket = clientSocket;

            protocolProcess = new BaseTcpProtocolProcess();
            _buffer = new byte[bufferSize];

            _receiveEA = new SocketAsyncEventArgs();            
            _sendEA = new SocketAsyncEventArgs();
            _receiveEA.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);            
            _sendEA.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);

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
                    throw new ArgumentException(string.Format("The last operation completed on the socket was not a receive or send : {0}", e.LastOperation));
            }
        }

        void StartReceive()
        {
            _receiveEA.SetBuffer(_buffer, _bufferAvailable, _buffer.Length - _bufferAvailable);

            bool willRaiseEvent = _clientSocket.ReceiveAsync(_receiveEA);
            if (!willRaiseEvent)
            {
                ProcessReceive(_receiveEA);
            }
        }

        public void Send(byte[] bytes)
        {
            if(null == _clientSocket)
            {
                return;
            }

            _sendEA.SetBuffer(bytes, 0, bytes.Length);
            
            bool willRaiseEvent = _clientSocket.SendAsync(_sendEA);
            if (!willRaiseEvent)
            {
                ProcessSend(_sendEA);
            }
        }

        /// <summary>
        /// 处理接收到的消息（多线程事件）
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {            
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {                               
                _bufferAvailable += e.BytesTransferred;

                //协议处理器处理协议数据
                int used = protocolProcess.Unpack(_buffer, _bufferAvailable);

                //Console.WriteLine("Thread [{0}] : bytes (receive [{1}] , totoal [{2}] , used [{3}] , remains [{4}])", Thread.CurrentThread.ManagedThreadId, e.BytesTransferred, _bufferAvailable, used, _bufferAvailable - used);

                if(used > 0)
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

        /// <summary>
        /// 处理发送的消息回调（多线程事件）
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                //Console.WriteLine("Thread[{0}]: send {1} bytes!", Thread.CurrentThread.ManagedThreadId, e.Buffer.Length);
            }
            else
            {
                Shutdown();
            }
        }

        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        void Shutdown()
        {
            try
            {
                _clientSocket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            catch (Exception) { }
            _clientSocket.Close();
            _clientSocket = null;
            _buffer = null;
            Console.WriteLine("A client has shutdown");
            _server.Exit(this);
        }

        /// <summary>
        /// 关闭客户端
        /// </summary>
        public void Close()
        {
            isClosed = true;
        }
    }
}
