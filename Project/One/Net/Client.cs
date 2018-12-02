using System;
using System.Net.Sockets;
using System.Threading;

namespace One.Net
{
    class Client
    {
        SocketAsyncEventArgs _receiveEA;
        SocketAsyncEventArgs _sendEA;
        Socket _socket;

        ByteArray _ba;

        /// <summary>
        /// 是否客户端已关闭
        /// </summary>
        public bool isClosed { get; private set; } = false;

        public Client(Socket socket)
        {
            Console.WriteLine("A client has been connected");

            _socket = socket;

            _ba = new ByteArray(4096);

            _receiveEA = new SocketAsyncEventArgs();
            _receiveEA.SetBuffer(_ba.Available, 0, _buffer.Length);
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
            bool willRaiseEvent = _socket.ReceiveAsync(_receiveEA);
            if (!willRaiseEvent)
            {
                ProcessReceive(_receiveEA);
            }
        }

        public void Send(byte[] bytes)
        {
            _sendEA.SetBuffer(bytes, 0, bytes.Length);
            
            bool willRaiseEvent = _socket.SendAsync(_sendEA);
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
                Console.WriteLine("Thread[{0}]: The server has read a total of {1} bytes", Thread.CurrentThread.ManagedThreadId, e.BytesTransferred);

                //将读取的数据写入到缓存中              
                Array.Copy(e.Buffer,e.Offset,_buffer,)

                //byte[] ba = new byte[e.BytesTransferred];
                //Array.Copy(e.Buffer, e.Offset, ba, 0, e.BytesTransferred);

                StartReceive();


                //Send(ba);                             
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
                Console.WriteLine("Thread[{0}]: send {1} bytes!", Thread.CurrentThread.ManagedThreadId, e.Buffer.Length);
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
                _socket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            catch (Exception) { }
            _socket.Close();
            Console.WriteLine("A client has been disconnected");
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
