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
        public event Action<Client> onOpen;

        /// <summary>
        /// 连接关闭
        /// </summary>
        public event Action<Client> onClose;

        /// <summary>
        /// 收到二进制消息
        /// </summary>
        public event Action<Client, byte[]> onBytesMessage;

        /// <summary>
        /// 收到字符串消息
        /// </summary>
        public event Action<Client, string> onStringMessage;

        /// <summary>
        /// <para>通过实现接口来进行自定义数据处理</para>
        /// 消息处理器，如果不为null,则收到消息时会调用IMessageExpress的Unpack方法。        
        /// </summary>
        public IMessageExpress messageExpress = null;

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
                _socket.Close();
                _socket = null;
                onClose?.Invoke(this);
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
                _socket.Close();
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
            OneLog.I("连接WebSocket服务 {0}", Url);
            _socket = new WebSocketSharp.WebSocket(Url);
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnError += OnError;
            _socket.OnMessage += OnMessage;
            _socket.Connect();
        }

        /// <summary>
        /// 发送二进制数据
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] bytes)
        {
            if (IsAlive)
            {
                _socket.Send(bytes);
            }
        }

        /// <summary>
        /// 发送字符串数据
        /// </summary>
        /// <param name="str"></param>
        public void Send(string str)
        {
            if (IsAlive)
            {
                _socket.Send(str);
            }
        }

        /// <summary>
        /// 发送消息，如果messageExpress存在，则会对数据进行Pack
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="msg"></param>
        public void SendPackage(object msg)
        {
            if(null == messageExpress)
            {
                throw new Exception("必须有messageExpress对象才能使用该方法");
            }

            var data = messageExpress.Pack(msg);

            if(data is string)
            {
                Send((string)data);
            }
            else if(data is byte[])
            {
                Send((byte[])data);
            }
            else
            {
                throw new Exception(data.GetType().FullName + "数据类型无法发送");
            }
        }

        private void OnOpen(object sender, EventArgs e)
        {
            OneLog.I(ConsoleColor.DarkGreen, "连接打开");
            onOpen?.Invoke(this);
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            OneLog.I(ConsoleColor.DarkGreen, "连接关闭");
            Close();
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            OneLog.I(ConsoleColor.DarkGreen, "连接出错");
            Close();
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                OneLog.I(ConsoleColor.DarkGreen, string.Format("收到文本消息: {0}", e.Data));

                messageExpress?.Unpack(this, e.Data);
                onStringMessage?.Invoke(this, e.Data);
            }
            else if (e.IsBinary)
            {
                OneLog.I(ConsoleColor.DarkGreen, string.Format("收到二进制消息: size[{0}]", e.RawData.Length));

                messageExpress?.Unpack(this, e.RawData);
                onBytesMessage?.Invoke(this, e.RawData);
            }
        }
    }
}
