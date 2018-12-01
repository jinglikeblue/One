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
        byte[] _buffer;

        public Client(Socket socket)
        {
            Console.WriteLine("A client has been connected");

            _socket = socket;

            _buffer = new byte[4096];

            _receiveEA = new SocketAsyncEventArgs();
            _receiveEA.SetBuffer(_buffer, 0, _buffer.Length);
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
            _sendEA.SetBuffer(bytes);
            
            bool willRaiseEvent = _socket.SendAsync(_sendEA);
            if (!willRaiseEvent)
            {
                ProcessSend(_sendEA);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {            
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {                               
                Console.WriteLine("Thread[{0}]: The server has read a total of {1} bytes", Thread.CurrentThread.ManagedThreadId, e.BytesTransferred);

                byte[] ba = new byte[e.BytesTransferred];

                Array.Copy(e.Buffer, e.Offset, ba, 0, e.BytesTransferred);
                Send(ba);
                StartReceive();
            }
            else
            {
                Close();
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                Console.WriteLine("Thread[{0}]: Send data success!", Thread.CurrentThread.ManagedThreadId);
            }
            else
            {
                Close();
            }
        }

        public void Close()
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
    }
}
