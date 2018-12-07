using One.Net;
using One.Protocol;
using System;

namespace OneDemo
{
    class User
    {
        public IRemoteProxy client { get; }

        BaseTcpProtocolProcess _protocolProcess;

        bool _destroyFlag = false;

        /// <summary>
        /// 标记为要销毁对象
        /// </summary>
        public void MarkDestroy()
        {
            _destroyFlag = true;
        }

        public User(IRemoteProxy client)
        {
            this.client = client;
            _protocolProcess = client.protocolProcess as BaseTcpProtocolProcess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>返回false表示不再存活</returns>
        public bool Update()
        {
            if(_destroyFlag)
            {
                Destroy();
                return false;
            }

            _protocolProcess.ReceiveProtocols(OnReceiveProtocol);
            return true;
        }

        /// <summary>
        /// 接收到协议的处理
        /// </summary>
        /// <param name="obj"></param>
        private void OnReceiveProtocol(BaseTcpProtocolBody obj)
        {
            //Console.WriteLine("msg: {0}", obj.value);
            client.Send(_protocolProcess.Pack(obj));
        }

        public void Destroy()
        {
            
        }
    }    
}
