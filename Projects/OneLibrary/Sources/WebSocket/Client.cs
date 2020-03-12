using System;
using WebSocketSharp;

namespace One.WebSocket
{
    /// <summary>
    /// 客户端连接
    /// </summary>
    public class Client
    {
        WebSocketSharp.WebSocket _socket;

        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// 主机端口
        /// </summary>
        public int Port { get; private set; }

        public string Url
        {
            get
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
        /// 连接打开
        /// </summary>
        public event Action onOpen;

        /// <summary>
        /// 连接关闭
        /// </summary>
        public event Action onClose;

        /// <summary>
        /// 收到消息
        /// </summary>
        public event Action<byte[]> onMessage;

        /// <summary>
        /// 关闭连接
        /// </summary>        
        public void Close()
        {
            if (null != _socket)
            {
                _socket.OnOpen -= OnOpen;
                _socket.OnClose -= OnClose;
                _socket.OnError -= OnError;
                _socket.OnMessage -= OnMessage;
                _socket.CloseAsync();
                _socket = null;
                onClose?.Invoke();
            }
        }

        /// <summary>
        /// 关闭链接，并且不触发任何事件
        /// </summary>
        public void CloseSliently()
        {
            if (null != _socket)
            {
                _socket.OnOpen -= OnOpen;
                _socket.OnClose -= OnClose;
                _socket.OnError -= OnError;
                _socket.OnMessage -= OnMessage;
                _socket.CloseAsync();
                _socket = null;
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public void Connect(string host, int port)
        {
            Host = host;
            Port = port;
            Reconnect();
        }

        /// <summary>
        /// 重连服务器
        /// </summary>
        public void Reconnect()
        {
            CloseSliently();
            Log.I("连接WebSocket服务 {0}", Url);
            _socket = new WebSocketSharp.WebSocket(Url);
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnError += OnError;
            _socket.OnMessage += OnMessage;
            _socket.Connect();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            if (IsAlive)
            {
                _socket.Send(data);
            }
        }

        private void OnOpen(object sender, EventArgs e)
        {
            One.Log.I(ConsoleColor.DarkGreen, "连接打开");
            onOpen?.Invoke();
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            One.Log.I(ConsoleColor.DarkGreen, "连接关闭");
            Close();
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            One.Log.I(ConsoleColor.DarkGreen, "连接出错");
            Close();
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            byte[] data = null;
            if (e.IsText)
            {
                One.Log.I(ConsoleColor.DarkGreen, string.Format("收到文本消息: {0}", e.Data));
                data = MessageUtility.TransformData(e.Data);
            }
            else if (e.IsBinary)
            {
                One.Log.I(ConsoleColor.DarkGreen, string.Format("收到二进制消息: {0}", e.RawData.ToString()));
                data = e.RawData;
            }
            onMessage?.Invoke(data);
        }
    }
}
