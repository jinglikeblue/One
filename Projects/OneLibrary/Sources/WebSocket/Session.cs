using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

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
        public event Action onOpen;

        /// <summary>
        /// 会话关闭事件
        /// </summary>
        public event Action onClose;

        /// <summary>
        /// 会话出错事件
        /// </summary>
        public event Action onError;

        /// <summary>
        /// 接收数据的事件，如果发过来的本来是字符串，请使用TransformData方法转换byte[] => string
        /// </summary>
        public event Action<byte[]> onMessage;

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
        public bool isClosed { get; private set; } = false;

        public Session(Behavior behavior, SessionManager sessionMgr)
        {            
            this.behavior = behavior;            
            behavior.BindingCallback(onBehaviorOpen, onBehaviorClose, onBehaviorError, onBehaviorMessage);
        }

        void onBehaviorOpen()
        {
            Console.WriteLine("连接打开");
            SwitchState(ESessionState.OPEN);
            onOpen?.Invoke();
        }

        void onBehaviorClose(CloseEventArgs e)
        {
            Console.WriteLine("连接关闭");
            SwitchState(ESessionState.CLOSE);
            onClose?.Invoke();
        }

        void onBehaviorError(ErrorEventArgs e)
        {
            Console.WriteLine("连接出错");
            SwitchState(ESessionState.ERROR);
            onError?.Invoke();
        }

        void onBehaviorMessage(MessageEventArgs e)
        {
            byte[] msgBytes = null;
            if (e.IsBinary)
            {
                msgBytes = e.RawData;
                Console.WriteLine(string.Format("收到二进制消息: {0}", e.RawData.ToString()));
            }
            else if (e.IsText)
            {
                msgBytes = MessageUtility.TransformData(e.Data);
                Console.WriteLine(string.Format("收到文本消息: {0}", e.Data));
            }

            onMessage?.Invoke(msgBytes);
        }

        /// <summary>
        /// 关闭会话
        /// </summary>
        public void Close()
        {
            if (!isClosed)
            {
                if (behavior != null)
                {
                    behavior.CloseSliently();
                    behavior = null;
                }

                isClosed = true;
                sessionManager.RemoveSession(this);
            }
        }

        /// <summary>
        /// 发送数据，如果希望发送字符串数据，请使用TransformData方法转换
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            Console.WriteLine("发送二进制消息：" + data.ToString());
            behavior.Send(data);
        }

        void SwitchState(ESessionState state)
        {
            this.state = state;
            onStateChange?.Invoke(this);
        }
    }
}
