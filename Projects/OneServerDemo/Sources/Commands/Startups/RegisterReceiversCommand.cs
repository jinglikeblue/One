using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Share;

namespace OneServer
{
    /// <summary>
    /// 注册协议接收器
    /// </summary>
    class RegisterReceiversCommand : BaseCommand
    {
        static readonly Type baseReceiverType = typeof(BaseMessageReceiver<>);

        Dictionary<Type, Type> _dataTypeToReceiverType = new Dictionary<Type, Type>();
        Dictionary<Type, MsgAttribute> _dataTypeToMsgAtt = new Dictionary<Type, MsgAttribute>();        

        public override void Excute()
        {                     
            var allTypes = baseReceiverType.Assembly.GetTypes();

            foreach (var type in allTypes)
            {
                RegisterDataTypeToReceiverType(type);
                RegisterDataTypeToMsgId(type);
            }



            //找到所有的协议数据对象

            //确定泛型数据类型和协议数据对象一致的，拿到协议ID，建立映射
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
                _dataTypeToReceiverType[type] = (Type)dataType;
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
