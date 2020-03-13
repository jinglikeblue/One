using System;
using WebSocketSharp;

namespace One.WebSocket
{
    /// <summary>
    /// 连接到服务器的会话
    /// </summary>    
    public class Session
    {
        /// <summary>
        /// 会话开启事件
        /// </summary>
        public event Action<Session> onOpen;

        /// <summary>
        /// 会话关闭事件
        /// </summary>
        public event Action<Session> onClose;

        /// <summary>
        /// 收到二进制消息
        /// </summary>
        public event Action<Session, byte[]> onBytesMessage;

        /// <summary>
        /// 收到字符串消息
        /// </summary>
        public event Action<Session, string> onStringMessage;

        /// <summary>
        /// <para>通过实现接口来进行自定义数据处理</para>
        /// 消息处理器，如果不为null,则收到消息时会调用IMessageExpress的Unpack方法。        
        /// </summary>
        public IMessageExpress messageExpress = null;

        /// <summary>
        /// 会话状态改变事件
        /// </summary>
        public event Action<Session> onStateChange;

        /// <summary>
        /// 会话Id
        /// </summary>
        public string id
        {
            get
            {
                return this.behavior.ID;
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public ESessionState state { get; private set; } = ESessionState.IDLE;

        public Behavior behavior { get; private set; }

        public SessionManager sessionManager { get; private set; }

        /// <summary>
        /// 是否关闭了
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return this.behavior == null ? true : false;
            }
        }

        internal Session(Behavior behavior, SessionManager sessionMgr)
        {
            this.behavior = behavior;
            this.sessionManager = sessionMgr;
            behavior.BindingCallback(onBehaviorOpen, onBehaviorClose, onBehaviorError, onBehaviorMessage);
        }

        void onBehaviorOpen()
        {
            sessionManager.Add(this);
            One.OneLog.I(ConsoleColor.DarkGreen, id + " 连接打开");
            SwitchState(ESessionState.OPEN);
            onOpen?.Invoke(this);
        }

        void onBehaviorClose(CloseEventArgs e)
        {
            One.OneLog.I(ConsoleColor.DarkGreen, id + " 连接关闭");
            SwitchState(ESessionState.CLOSE);
            Close();
        }

        void onBehaviorError(ErrorEventArgs e)
        {
            One.OneLog.I(ConsoleColor.DarkGreen, id + " 连接出错");
            SwitchState(ESessionState.ERROR);
            Close();
        }

        void onBehaviorMessage(MessageEventArgs e)
        {
            try
            {
                if (e.IsText)
                {
                    One.OneLog.I(ConsoleColor.DarkGreen, string.Format(id + " 收到文本消息:\n{0}", e.Data));

                    messageExpress?.Unpack(this, e.Data);
                    onStringMessage?.Invoke(this, e.Data);
                }
                else if (e.IsBinary)
                {
                    One.OneLog.I(ConsoleColor.DarkGreen, string.Format(id + " 收到二进制消息: size[]", e.RawData.Length));

                    messageExpress?.Unpack(this, e.RawData);
                    onBytesMessage?.Invoke(this, e.RawData);
                }
            }
            catch (Exception ex)
            {
                OneLog.E(ex.StackTrace);
                Close();
            }
        }

        /// <summary>
        /// 关闭会话
        /// </summary>
        public void Close()
        {
            if (!IsClosed)
            {
                sessionManager.Remove(this);
                behavior.CloseSliently();
                behavior = null;

                onClose?.Invoke(this);
            }
        }

        /// <summary>
        /// 发送二进制数据
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {
            if (behavior.ConnectionState == WebSocketState.Open)
            {
                OneLog.D("向[{0}]发送二进制数据 Size:{1}", id, bytes.Length);
                behavior.Send(bytes);
            }
        }

        /// <summary>
        /// 发送字符串数据
        /// </summary>
        /// <param name="data"></param>
        public void Send(string str)
        {
            if (behavior.ConnectionState == WebSocketState.Open)
            {
                OneLog.D("向[{0}]发送字符串:\n{1}", id, str);
                behavior.Send(str);
            }
        }

        /// <summary>
        /// 发送消息，如果messageExpress存在，则会对数据进行Pack
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="msg"></param>
        public void SendPackage(object msg)
        {
            if (null == messageExpress)
            {
                throw new Exception("必须有messageExpress对象才能使用该方法");
            }

            var data = messageExpress.Pack(msg);

            if (data is string)
            {
                Send((string)data);
            }
            else if (data is byte[])
            {
                Send((byte[])data);
            }
            else
            {
                throw new Exception(data.GetType().FullName + "数据类型无法发送");
            }
        }

        void SwitchState(ESessionState state)
        {
            this.state = state;
            onStateChange?.Invoke(this);
        }
    }
}
