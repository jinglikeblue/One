using System;
using System.Collections.Generic;
using System.Text;

namespace One
{
    /// <summary>
    /// 服务器基类
    /// </summary>
    public abstract class BaseServer
    {
        public Type sessionType
        {
            get
            {
                if(null == sessions)
                {
                    throw new Exception("Please RegisterSeesionType");
                }
                return sessions.sessionType;
            }
        }

        public SessionCenter sessions { get; private set; }

        List<Type> _receiverTypeList = new List<Type>();

        /// <summary>
        /// 启动服务
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 停止服务
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// 注册Session的类型
        /// </summary>
        /// <param name="sessionType"></param>
        public void RegisterSeesionType(Type sessionType)
        {
            sessions = new SessionCenter(sessionType);            
        }

        /// <summary>
        /// 注册接收器类型
        /// </summary>
        /// <param name="receiverType"></param>
        public void AddReceiverType(Type receiverType)
        {
            _receiverTypeList.Add(receiverType);
        }
    }
}
