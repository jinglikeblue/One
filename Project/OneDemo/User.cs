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

        bool _isDestroy = false;

        /// <summary>
        /// 标记为要销毁对象
        /// </summary>
        public void MarkDestroy()
        {
            _isDestroy = true;
        }

        public User(Client client)
        {
            this.client = client;            
        }

        public void Update()
        {
            if(_isDestroy)
            {
                Destroy();
                return;
            }

            client.protocolProcess.ReceiveProtocols(OnReceiveProtocol);
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
            UserMgr.Ins.RemoveUser(this);
        }
    }    
}
