using System.Collections.Generic;

namespace One.WebSocket
{
    /// <summary>
    /// 会话管理器
    /// </summary>
    public class SessionManager
    {
        Dictionary<string, Session> _sessionDic = new Dictionary<string, Session>();

        public int Count
        {
            get
            {
                return _sessionDic.Count;
            }
        }

        /// <summary>
        /// 获取会话列表
        /// </summary>
        /// <returns></returns>
        public Session[] GetSessionList()
        {
            Session[] list = new Session[_sessionDic.Count];
            _sessionDic.Values.CopyTo(list, 0);
            return list;
        }

        /// <summary>
        /// 添加会话
        /// </summary>
        /// <param name="session"></param>
        internal void Add(Session session)
        {
            _sessionDic[session.id] = session;
        }

        /// <summary>
        /// 移除会话
        /// </summary>
        /// <param name="session"></param>
        internal void Remove(Session session)
        {
            if (_sessionDic.ContainsKey(session.id))
            {
                _sessionDic.Remove(session.id);
            }
        }

        /// <summary>
        /// 关闭会话，如果会话没有关闭，则将其关闭
        /// </summary>
        /// <param name="id"></param>
        public void CloseSession(string id)
        {
            if (_sessionDic.ContainsKey(id))
            {
                var session = _sessionDic[id];
                CloseSession(session);
            }
        }

        /// <summary>
        /// 关闭会话，如果会话没有关闭，则将其关闭
        /// </summary>
        /// <param name="session"></param>
        public void CloseSession(Session session)
        {
            session.Close();
        }

        /// <summary>
        /// 给所有会话推送消息
        /// </summary>
        /// <param name="data"></param>
        public void Push(byte[] bytes)
        {
            foreach (var session in _sessionDic.Values)
            {
                session.Send(bytes);
            }
        }

        /// <summary>
        /// 给所有会话推送消息
        /// </summary>
        /// <param name="str"></param>
        public void Push(string str)
        {
            foreach (var session in _sessionDic.Values)
            {
                session.Send(str);
            }
        }

        /// <summary>
        /// 发送消息，如果messageExpress存在，则会对数据进行Pack
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="msg"></param>
        public void PushPackage(object msg)
        {
            foreach (var session in _sessionDic.Values)
            {
                session.SendPackage(msg);
            }
        }
    }
}
