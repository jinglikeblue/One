using One.Data;
using One.Protocol;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace One.Net
{
    public class UdpServer<T> where T : IProtocolProcess, new()
    {       
        /// <summary>
        /// 收到UDP数据的事件（多线程事件）
        /// </summary>
        public event EventHandler<UdpRemoteProxy> onReceiveDataEvent;

        /// <summary>
        /// 监听的端口
        /// </summary>
        protected Socket _socket;

        IPEndPoint _endPoint;

        SocketAsyncEventArgs _receiveEA;

        byte[] _buffer;

        /// <summary>
        /// 启动Socket服务
        /// </summary>
        /// <param name="host">监听的地址</param>
        /// <param name="bindPort">坚挺的端口</param>
        /// <param name="bufferSize">每一个连接的缓冲区大小</param>
        public void Start(int bindPort, int bufferSize)
        {
            Console.WriteLine(string.Format("Start Lisening {0}:{1}", IPAddress.Any, bindPort));

            _endPoint = new IPEndPoint(IPAddress.Any, bindPort);
            _socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(_endPoint);
            _buffer = new byte[bufferSize];
            _receiveEA = new SocketAsyncEventArgs();            
            _receiveEA.Completed += OnReceiveCompleted;
            _receiveEA.RemoteEndPoint = _endPoint;

            StartReceive();
        }

        /// <summary>
        /// 开始接受链接
        /// </summary>
        /// <param name="e"></param>
        void StartReceive()
        {
            _receiveEA.SetBuffer(_buffer, 0, _buffer.Length);
            bool willRaiseEvent = _socket.ReceiveFromAsync(_receiveEA);
            if (!willRaiseEvent)
            {
                OnReceiveCompleted(null, _receiveEA);
            }
        }

        /// <summary>
        /// 接收到连接完成的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            byte[] data = new byte[e.BytesTransferred];
            Array.Copy(e.Buffer,data,e.BytesTransferred);
            
            Task.Run(
                ()=> ProcessReceiveDataTask(e.RemoteEndPoint, data, data.Length)
            );
            
            StartReceive();
        }


        void ProcessReceiveDataTask(EndPoint remoteEndPOINT, byte[] data, int available)
        {            
            UdpRemoteProxy client = new UdpRemoteProxy(_socket, remoteEndPOINT, new T());
            client.protocolProcess.Unpack(data, data.Length);
            onReceiveDataEvent?.Invoke(this, client);
        }
    }
}
