using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace One.WebSocket
{
    public class ProtobufExpress : IMessageExpress
    {
        const int MSG_ID_SIZE = 4;

        Dictionary<Type, int> _idDic;
        Dictionary<int, Type> _pbTypeDic;
        Dictionary<int, Type> _receiverTypeDic;

        public ProtobufExpress()
        {
            _idDic = new Dictionary<Type, int>();
            _pbTypeDic = new Dictionary<int, Type>();
            _receiverTypeDic = new Dictionary<int, Type>();
        }

        /// <summary>
        /// 注册协议Id和对应的结构体
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="pbType"></param>
        public void RegisterMsg(int msgId, Type pbType)
        {
            _idDic[pbType] = msgId;
            _pbTypeDic[msgId] = pbType;
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

        /// <summary>
        /// 打包PB协议为通信使用的二进制
        /// </summary>
        /// <param name="pbObj"></param>
        /// <returns></returns>
        public object Pack(object pbObj)
        {
            var msgId = _idDic[pbObj.GetType()];
            var datas = ((IMessage)pbObj).ToByteArray();

            Jing.ByteArray ba = new Jing.ByteArray(MSG_ID_SIZE + datas.Length);
            ba.Write(msgId);
            ba.Write(datas);
            return ba.Bytes;
        }

        /// <summary>
        /// 解包PB协议，并调用注册的receiver进行分发
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bytes"></param>
        public void Unpack(object target, object bytes)
        {
            int msgId;
            IMessage pbObj;
            GetMsgIdAndDatas((byte[])bytes, out msgId, out pbObj);

            if (false == _receiverTypeDic.ContainsKey(msgId))
            {
                throw new Exception($"协议Id:[{msgId}] 没有对应的接收器");
            }

            var receiverType = _receiverTypeDic[msgId];
            var receiverObj = Activator.CreateInstance(receiverType);
            var receiverMethod = receiverType.GetMethod("OnReceive", BindingFlags.NonPublic | BindingFlags.Instance);
            receiverMethod.Invoke(receiverObj, new object[] { target, pbObj });
        }

        void GetMsgIdAndDatas(byte[] bytes, out int msgId, out IMessage pbObj)
        {
            Jing.ByteArray ba = new Jing.ByteArray(bytes);
            msgId = ba.ReadInt();
            var datas = ba.ReadBytes(ba.ReadEnableSize);

            if (false == _pbTypeDic.ContainsKey(msgId))
            {
                throw new Exception($"协议Id:[{msgId}] 没有对应的结构体");
            }

            var pbType = _pbTypeDic[msgId];
            pbObj = (IMessage)Activator.CreateInstance(pbType);
            pbObj.MergeFrom(datas);
        }
    }
}
