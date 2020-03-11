namespace One
{
    public abstract class BaseSession
    {               
        /// <summary>
        /// Session的ID
        /// </summary>
        public string id { get; internal set; }     
        
        public SessionCenter sessions { get; internal set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sessions"></param>
        internal void Init(string id, SessionCenter sessions)
        {
            this.id = id;
            this.sessions = sessions;            
        }

        protected abstract void OnOpen();
        protected abstract void OnClose();
        protected abstract void OnError();
        protected abstract void OnMessage(object data);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        public abstract void Send(object data);

        /// <summary>
        /// 关闭Session
        /// </summary>
        public abstract void Close();
    }
}
