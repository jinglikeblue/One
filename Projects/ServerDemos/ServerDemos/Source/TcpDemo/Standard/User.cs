using System;
using One;

namespace ServerDemo
{
    class User
    {
        public IRemoteProxy client { get; }

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
            this.client.onReceiveData += OnReceiveData;            
        }

        private void OnReceiveData(IRemoteProxy sender, byte[] data)
        {
            ByteArray ba = new ByteArray(data);
            Log.I("收到消息:{0}", ba.ReadString());

            ba.Reset();
            ba.Write("Server Got It!");
            client.Send(ba.GetAvailableBytes());
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

            //_protocolProcess.ReceiveProtocols(OnReceiveProtocol);
            return true;
        }

        /// <summary>
        /// 接收到协议的处理
        /// </summary>
        /// <param name="obj"></param>
        //private void OnReceiveProtocol(BaseTcpProtocolBody obj)
        //{
        //    //Console.WriteLine("msg: {0}", obj.value);
        //    client.Send(_protocolProcess.Pack(obj));
        //}

        public void Destroy()
        {
            
        }
    }    
}
