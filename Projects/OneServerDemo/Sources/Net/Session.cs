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

            var msgInfo = Global.Ins.core.msgInfoTable.GetMsgInfo(mp.id);
            if(null != msgInfo)
            {
                Log.I("收到协议: {0}({1}) \r\n {2}", mp.id, msgInfo.name, mp.content);

                var dataObj = JsonConvert.DeserializeObject(mp.content, msgInfo.dataType);
                var receiverIns = Activator.CreateInstance(msgInfo.receiverType);
                msgInfo.receiveMethodInfo.Invoke(receiverIns, new object[] { this, mp.requestId, dataObj });
            }
            else
            {
                Log.E($"协议[Id:{mp.id}]没有匹配的信息");
            }            
        }

        protected override void OnOpen()
        {
            Log.I(ConsoleColor.DarkMagenta, "建立连接：{0}", id);
        }

        public void Send(object data, int requestId)
        {
            var dataType = data.GetType();
            var msgInfo = Global.Ins.core.msgInfoTable.GetMsgInfo(dataType);
            if(null == msgInfo)
            {
                throw new Exception($"{dataType.FullName} 不是一个正确的协议数据对象");
            }          

            MsgPackageVO mp = new MsgPackageVO();
            mp.id = msgInfo.id;
            mp.requestId = requestId;
            mp.content = JsonConvert.SerializeObject(data);
            string msg = JsonConvert.SerializeObject(mp);

            Log.I("发送协议: {0}({1}) \r\n {2}", msgInfo.id, msgInfo.name, mp.content);

            behavior.SendData(msg);
        }

        public override void Send(object data)
        {
            Send(data, 0);
        }
    }
}
