using System.Collections.Generic;

namespace One.WebSocket
{
    public class SessionManager
    {
        Dictionary<string, Session> _sessionDic = new Dictionary<string, Session>();

        /// <summary>
        /// 针对behavior创建会话
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public Session CreateSession(Behavior behavior)
        {
            var session = new Session(behavior, this);
            _sessionDic[session.id] = session;
            return session;
        }

        /// <summary>
        /// 移除会话，如果会话没有关闭，则将其关闭
        /// </summary>
        /// <param name="id"></param>
        public void RemoveSession(string id)
        {
            if (_sessionDic.ContainsKey(id))
            {
                var session = _sessionDic[id];
                _sessionDic.Remove(id);
                session.Close();
            }
        }

        /// <summary>
        /// 移除会话，如果会话没有关闭，则将其关闭
        /// </summary>
        /// <param name="session"></param>
        public void RemoveSession(Session session)
        {
            RemoveSession(session.id);
        }

        /// <summary>
        /// 给所有会话推送消息
        /// </summary>
        /// <param name="data"></param>
        public void Push(byte[] data)
        {
            foreach(var session in _sessionDic.Values)
            {
                session.Send(data);
            }
        }
    }
}
