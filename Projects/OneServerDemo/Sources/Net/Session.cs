using Jing;
using Newtonsoft.Json;
using One;
using Share;
using System;
using System.Reflection;

namespace OneServer
{
    class Session : WebSocketSession
    {
        static Type _msgAtt = typeof(MsgAttribute);

        protected override void OnClose()
        {
            Log.I(ConsoleColor.DarkCyan, "连接关闭：{0}", id);
        }

        protected override void OnError()
        {
            Log.I("连接出错：{0}", id);
        }

        protected override void OnMessage(object msgObj)
        {
            MsgPackageVO mp = null;
            try
            {
                string msg = (string)msgObj;
                mp = JsonConvert.DeserializeObject<MsgPackageVO>(msg);
            }
            catch(Exception e)
            {
                Log.E($"{behavior.Context.UserEndPoint.ToString()}发过来的协议不合法:{msgObj}");
                Close();
            }

            Log.I("收到协议: {0}({1}) \r\n {2}", mp.id, "", mp.content);

            //创建对应的Receiver对象
            //IMessageReceiver receiver = null;
            //Type dataType = null;// receiver.GetDataType();
            //var dataObj = JsonConvert.DeserializeObject(mp.content, dataType);

            //var receiverMethod = receiver.GetType().GetMethod("OnReceive", BindingFlags.NonPublic | BindingFlags.Instance);
            //receiverMethod.Invoke(receiver, new object[] { this, mp.requestId, dataObj });
        }

        protected override void OnOpen()
        {
            Log.I(ConsoleColor.DarkMagenta, "建立连接：{0}", id);
        }

        public void Send(object data, int requestId)
        {
            var dataType = data.GetType();
            MsgAttribute att;
            try
            {
                att = (MsgAttribute)dataType.GetCustomAttributes(_msgAtt, false)[0];
            }
            catch(Exception e)
            {
                throw new Exception($"{dataType.FullName} 不是一个正确的协议数据对象: \r\n {e.Message}");
            }            

            MsgPackageVO mp = new MsgPackageVO();
            mp.id = att.id;
            mp.requestId = requestId;
            mp.content = JsonConvert.SerializeObject(data);
            string msg = JsonConvert.SerializeObject(mp);

            Log.I("发送协议: {0}({1}) \r\n {2}", att.id, att.name, mp.content);

            behavior.SendData(msg);
        }

        public override void Send(object data)
        {
            Send(data, 0);
        }
    }
}
