using One;
using Share;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OneServer
{
    class InitMsgInfoTableCommand : BaseCommand
    {
        public event Action<MsgInfoTable> onCreated;

        static readonly Type baseReceiverType = typeof(BaseMessageReceiver<>);

        Dictionary<Type, Type> _dataTypeToReceiverType = new Dictionary<Type, Type>();
        Dictionary<Type, MsgAttribute> _dataTypeToMsgAtt = new Dictionary<Type, MsgAttribute>();


        public override void Excute()
        {
            var allTypes = baseReceiverType.Assembly.GetTypes();
            foreach (var type in allTypes)
            {
                RegisterDataTypeToReceiverType(type);
            }

            var shareTypes = typeof(MsgAttribute).Assembly.GetTypes();
            foreach (var type in shareTypes)
            {
                RegisterDataTypeToMsgId(type);
            }

            MsgInfoTable table = new MsgInfoTable();
            //构建协议映射表
            foreach (var kv in _dataTypeToMsgAtt)
            {
                if (_dataTypeToReceiverType.ContainsKey(kv.Key))
                {
                    var vo = new MsgInfoVO();
                    vo.id = kv.Value.id;
                    vo.name = kv.Value.name;
                    vo.dataType = kv.Key;
                    vo.receiverType = _dataTypeToReceiverType[kv.Key];
                    vo.receiveMethodInfo = vo.receiverType.GetMethod("OnReceive", BindingFlags.NonPublic | BindingFlags.Instance);
                    table.AddMsgInfo(vo);
                }
                else
                {
                    Log.E("协议[{0}]没有对应的Receiver", kv.Value.name);
                }
            }
            onCreated?.Invoke(table);
        }

        /// <summary>
        /// 找到所有继承了BaseMessageReceiver接口的对象,获取Receiver对象的泛型数据类型
        /// </summary>
        /// <param name="type"></param>
        void RegisterDataTypeToReceiverType(Type type)
        {
            if (type.BaseType == null || false == type.BaseType.IsGenericType)
            {
                return;
            }

            if (type.BaseType.GetGenericTypeDefinition() == baseReceiverType)
            {
                var getDataTypeMethod = type.BaseType.GetMethod("GetDataType", BindingFlags.Static | BindingFlags.Public);
                var dataType = getDataTypeMethod.Invoke(null, null);
                _dataTypeToReceiverType[(Type)dataType] = type;
            }
        }

        void RegisterDataTypeToMsgId(Type type)
        {
            var msgAtt = type.GetCustomAttribute<MsgAttribute>();

            if (null != msgAtt)
            {
                _dataTypeToMsgAtt[type] = msgAtt;
            }
        }

        public override void Terminate()
        {
            
        }
    }
}
