using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace One.WebSocket
{
    public class JsonExpress : IMessageExpress
    {
        class MsgVO
        {
            public int id;
            public string data;
        }

        Dictionary<Type, int> _idDic;
        Dictionary<int, Type> _pbTypeDic;
        Dictionary<int, Type> _receiverTypeDic;

        /// <summary>
        /// 注册协议Id和对应的结构体
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="pbType"></param>
        public void RegisterMsg(int msgId, Type voType)
        {
            _idDic[voType] = msgId;
            _pbTypeDic[msgId] = voType;
        }

        /// <summary>
        /// 注册协议Id和接收器
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="pbType"></param>
        public void RegisterReceiver(int msgId, Type receiverType)
        {
            _receiverTypeDic[msgId] = receiverType;
        }

        public object Pack(object vo)
        {
            var msg = new MsgVO();
            msg.id = _idDic[vo.GetType()];
            msg.data = JsonConvert.SerializeObject(vo);
            return JsonConvert.SerializeObject(msg);
        }

        public void Unpack(object target, object messageData)
        {
            var msg = JsonConvert.DeserializeObject<MsgVO>((string)messageData);
            var msgId = msg.id;

            if (false == _pbTypeDic.ContainsKey(msgId))
            {
                throw new Exception($"协议Id:[{msgId}] 没有对应的结构体");
            }

            if (false == _receiverTypeDic.ContainsKey(msgId))
            {
                throw new Exception($"协议Id:[{msgId}] 没有对应的接收器");
            }

            var msgData = JsonConvert.DeserializeObject(msg.data, _pbTypeDic[msgId]);

            var receiverType = _receiverTypeDic[msgId];
            var receiverObj = Activator.CreateInstance(receiverType);
            var receiverMethod = receiverType.GetMethod("OnReceive", BindingFlags.NonPublic | BindingFlags.Instance);
            receiverMethod.Invoke(receiverObj, new object[] { target, msgData });
        }
    }
}
