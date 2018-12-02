using One.Net;
using One.Protocol;
using OneDemo.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneDemo
{
    class User
    {
        public Client client { get; }

        bool _destroyFlag = false;

        /// <summary>
        /// 标记为要销毁对象
        /// </summary>
        public void MarkDestroy()
        {
            _destroyFlag = true;
        }

        public User(Client client)
        {
            this.client = client;            
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

            client.protocolProcess.ReceiveProtocols(OnReceiveProtocol);
            return true;
        }

        /// <summary>
        /// 接收到协议的处理
        /// </summary>
        /// <param name="obj"></param>
        private void OnReceiveProtocol(ProtocolBody obj)
        {
            client.Send(client.protocolProcess.Pack(obj));
        }

        public void Destroy()
        {
            
        }
    }    
}
