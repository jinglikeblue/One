using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace One.WebSocket
{
    /// <summary>
    /// 连接到服务器的会话
    /// </summary>    
    public class Session : WebSocketBehavior
    {
        static int _id = 0;
        
        /// <summary>
        /// 创建一个唯一的会话Id，用来标记这个连接
        /// </summary>
        /// <returns></returns>
        static int CreateId()
        {
            _id++;
            return _id;
        }

        public int id { get; private set; } = -1;

        public Session() : base()
        {
            id = CreateId();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("连接关闭");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine("连接出错");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                //TODO 文本消息 e.Data;     
                Console.WriteLine(string.Format("收到文本消息: {0}", e.Data));

            }
            else
            {
                //TODO 二进制消息 e.RawData
                Console.WriteLine(string.Format("收到二进制消息: {0}", e.RawData.ToString()));
            }
        }

        public void SendData(string data)
        {
            Console.WriteLine("发送文本消息：" + data);
            Send(data);
        }

        public void SendData(byte[] data)
        {
            Console.WriteLine("发送二进制消息：" + data.ToString());
            Send(data);
        }
    }
}
