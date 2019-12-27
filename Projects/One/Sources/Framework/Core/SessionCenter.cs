using System;
using System.Collections.Generic;
using System.Linq;

namespace One
{
    /// <summary>
    /// Session中心
    /// </summary>
    public class SessionCenter
    {
        Type sessionType;

        Dictionary<string, BaseSession> _sessionDic = new Dictionary<string, BaseSession>();

        /// <summary>
        /// 创建一个ID
        /// </summary>
        /// <returns></returns>
        string CreateId()
        {
            return Guid.NewGuid().ToString("N");
        }

        public BaseSession Create()
        {           
            var session = Activator.CreateInstance(sessionType) as BaseSession;
            session.Init(CreateId(), this);
            _sessionDic[session.id] = session;            
            return session;
        }

        /// <summary>
        /// 销毁一个Session
        /// </summary>
        /// <param name="session"></param>
        internal void Destroy(string id)
        {
            _sessionDic.Remove(id);
        }

        public BaseSession[] GetSessions()
        {
            return _sessionDic.Values.ToArray();
        }

        /// <summary>
        /// 目前的Session数量
        /// </summary>
        public int Count
        {
            get
            {
                return _sessionDic.Count;
            }
        }

        /// <summary>
        /// 查找Session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseSession Find(string id)
        {
            BaseSession session;
            _sessionDic.TryGetValue(id, out session);
            return session;
        }

        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="bytes"></param>
        public void Boradcost(object data)
        {
            foreach(var kv in _sessionDic)
            {
                kv.Value.Send(data);
            }
        }       


    }
}
