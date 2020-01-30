using System;
using WebSocketSharp;

namespace One
{
    /// <summary>
    /// WebSocket通信客户端
    /// </summary>
    public class WebSocketClient : BaseClient
    {
        WebSocket _socket;

        /// <summary>
        /// 连接打开
        /// </summary>
        public event Action onOpen;

        /// <summary>
        /// 连接关闭
        /// </summary>
        public event Action onClose;

        /// <summary>
        /// 连接出错
        /// </summary>
        public event Action onError;

        /// <summary>
        /// 收到消息
        /// </summary>
        public event Action<object> onMessage;

        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// 主机端口
        /// </summary>
        public int Port { get; private set; }

        public string Url { get
            {
                return string.Format("ws://{0}:{1}", Host, Port);
            } 
        }

        /// <summary>
        /// 是否连接中
        /// </summary>
        public bool IsAlive
        {
            get
            {
                if (_socket != null && _socket.IsConnected && _socket.IsAlive)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="isSilently">true，关闭并且不触发任何事件</param>
        public void Close(bool isSilently = true)
        {
            if (null != _socket)
            {
                if (isSilently)
                {
                    _socket.OnOpen -= OnOpen;
                    _socket.OnClose -= OnClose;
                    _socket.OnError -= OnError;
                    _socket.OnMessage -= OnMessage;
                }
                _socket.CloseAsync();
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public void Connect(string host, int port)
        {
            Log.I("连接服务器 {0}:{1}", host, port);
            Host = host;
            Port = port;
            Close();
            _socket = new WebSocket(Url);
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnError += OnError;
            _socket.OnMessage += OnMessage;
            _socket.Connect();            
        }

        private void OnOpen(object sender, EventArgs e)
        {
            onOpen?.Invoke();
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            onClose?.Invoke();
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            onError?.Invoke();
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                onMessage?.Invoke(e.Data);
            }
            else if (e.IsBinary)
            {
                onMessage?.Invoke(e.RawData);
            }
        }

        public void Send(string data)
        {
            _socket.Send(data);
        }

        public void Send(byte[] data)
        {
            _socket.Send(data);
        }

        public void Send(object data)
        {
            if(data is string)
            {
                Send((string)data);
            }
            else if(data is byte[])
            {
                Send((byte[])data);
            }
            
            throw new Exception($"WebSocketClient不支持Send的对象:{data}");                       
        }
    }
}
