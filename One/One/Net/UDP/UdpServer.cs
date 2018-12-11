using One.Data;
using One.Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace One.Net
{
    public class UdpServer<T> where T : IProtocolProcess, new()
    {
        /// <summary>
        /// 新的客户端进入的事件（非线程安全）
        /// </summary>
        public event EventHandler<IRemoteProxy> onClientEnterHandler;

        /// <summary>
        /// 客户端退出的事件（非线程安全）
        /// </summary>
        public event EventHandler<IRemoteProxy> onClientExitHandler;

        /// <summary>
        /// 监听的端口
        /// </summary>
        protected Socket _socket;

        protected int _clientCount = 0;
        /// <summary>
        /// 已连接的客户端总数
        /// </summary>
        public int ClientCount
        {
            get
            {
                return _clientCount;
            }
        }

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        int _bufferSize;

        IPEndPoint _endPoint;

        byte[] _buffer;

        SocketAsyncEventArgs _receiveEA;

        Dictionary<string, UdpRemoteProxy> _remoteProxyDic = new Dictionary<string, UdpRemoteProxy>();

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
            _bufferSize = bufferSize;
            _buffer = new byte[bufferSize];
            _socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(_endPoint);

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
            ByteArray ba = new ByteArray(_bufferSize);
            ba.Write(e.Buffer, 0, e.BytesTransferred);
            ba.SetPos(0);
            //添加一个成功链接
            ProcessReceiveData(e.RemoteEndPoint, ba);

            StartReceive();
        }

        void ProcessReceiveData(EndPoint remoteEndPoint, ByteArray ba)
        {
            var key = remoteEndPoint.ToString();
            UdpRemoteProxy client;
            if (false == _remoteProxyDic.ContainsKey(key))
            {
                client = new UdpRemoteProxy(_socket, remoteEndPoint, null, _bufferSize);
                _remoteProxyDic[key] = client;
            }
            else
            {
                client = _remoteProxyDic[key];
            }
            
            //返回数据
            client.Send(ba.ReadBytes(ba.ReadEnableSize));
            

            Console.WriteLine("Thread [{0}]: enter  total:{1}", Thread.CurrentThread.ManagedThreadId, _clientCount);
        }
    }
}
