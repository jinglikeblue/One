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
        /// 接收数据的事件，如果发过来的本来是字符串，请使用TransformData方法转换byte[] => string
        /// </summary>
        public event Action<Session, byte[]> onMessage;

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
        public bool IsClosed {
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
            One.Log.I(ConsoleColor.DarkGreen, id + " 连接打开");
            SwitchState(ESessionState.OPEN);
            onOpen?.Invoke(this);
        }

        void onBehaviorClose(CloseEventArgs e)
        {
            One.Log.I(ConsoleColor.DarkGreen, id + " 连接关闭");
            SwitchState(ESessionState.CLOSE);           
            Close();
        }

        void onBehaviorError(ErrorEventArgs e)
        {
            One.Log.I(ConsoleColor.DarkGreen, id + " 连接出错");
            SwitchState(ESessionState.ERROR);            
            Close();
        }

        void onBehaviorMessage(MessageEventArgs e)
        {
            byte[] msgBytes = null;
            if (e.IsBinary)
            {
                msgBytes = e.RawData;
                One.Log.I(ConsoleColor.DarkGreen, string.Format(id + " 收到二进制消息: {0}", e.RawData.ToString()));
            }
            else if (e.IsText)
            {
                msgBytes = MessageUtility.TransformData(e.Data);
                One.Log.I(ConsoleColor.DarkGreen, string.Format(id + " 收到文本消息: {0}", e.Data));
            }

            onMessage?.Invoke(this, msgBytes);
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
        /// 发送数据，如果希望发送字符串数据，请使用TransformData方法转换
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            if (behavior.ConnectionState == WebSocketState.Open)
            {
                Console.WriteLine("发送二进制消息：" + data.ToString());
                behavior.Send(data);
            }
        }

        void SwitchState(ESessionState state)
        {
            this.state = state;
            onStateChange?.Invoke(this);
        }
    }
}
