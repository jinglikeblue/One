using System;
using System.Net;
using System.Net.Sockets;

namespace One
{
    public class UdpServer
    {
        /// <summary>
        /// 收到UDP数据的事件（多线程事件）
        /// </summary>
        public event Action<UdpChannel, byte[]> onReceiveData;

        /// <summary>
        /// 监听的端口
        /// </summary>
        public Socket Socket { get; private set; }

        IPEndPoint _endPoint;

        SocketAsyncEventArgs _receiveEA;

        byte[] _buffer;

        /// <summary>
        /// 线程同步器，将异步方法同步到调用Refresh的线程中
        /// </summary>
        public ThreadSyncActions Tsa { get; } = new ThreadSyncActions();

        /// <summary>
        /// 启动Socket服务
        /// </summary>
        /// <param name="host">监听的地址</param>
        /// <param name="bindPort">坚挺的端口</param>
        /// <param name="bufferSize">每一个连接的缓冲区大小</param>
        public void Start(int bindPort, int bufferSize)
        {
            Log.CI(ConsoleColor.DarkGreen, "Start Lisening {0}:{1}", IPAddress.Any, bindPort);

            Tsa.Clear();
            _endPoint = new IPEndPoint(IPAddress.Any, bindPort);
            Socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            Socket.Bind(_endPoint);
            _buffer = new byte[bufferSize];
            _receiveEA = new SocketAsyncEventArgs();
            _receiveEA.Completed += OnAsyncEventCompleted;
            _receiveEA.RemoteEndPoint = _endPoint;

            StartReceive();
        }

        public void Refresh()
        {
            Tsa.RunSyncActions();
        }

        /// <summary>
        /// 异步事件完成（多线程事件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            Tsa.AddToSyncAction(() =>
            {
                ProcessReceive(e);
            });
        }

        /// <summary>
        /// 开始接受链接
        /// </summary>
        /// <param name="e"></param>
        void StartReceive()
        {
            _receiveEA.SetBuffer(_buffer, 0, _buffer.Length);
            bool willRaiseEvent = Socket.ReceiveFromAsync(_receiveEA);
            if (!willRaiseEvent)
            {
                ProcessReceive(_receiveEA);
            }
        }

        /// <summary>
        /// 接收到连接完成的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ProcessReceive(SocketAsyncEventArgs e)
        {
            byte[] data = new byte[e.BytesTransferred];
            Array.Copy(e.Buffer, data, e.BytesTransferred);

            ProcessReceiveDataTask(e.RemoteEndPoint, data);

            StartReceive();
        }

        void ProcessReceiveDataTask(EndPoint remoteEndPOINT, byte[] data)
        {
            UdpChannel client = new UdpChannel(this, remoteEndPOINT);
            onReceiveData?.Invoke(client, data);
        }
    }
}
