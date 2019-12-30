using Jing;
using Newtonsoft.Json;
using One;
using Share;
using System;

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

        protected override void OnMessage(object data)
        {
            if (data is string)
            {
                Log.I("收到消息：{0} data:{1}", id, data);
            }
            else if (data is byte[])
            {
                ByteArray ba = new ByteArray(data as byte[]);
                Log.I("收到消息：{0}", ba.ReadStringBytes(ba.Available));
            }

            Send(data);
        }

        protected override void OnOpen()
        {
            Log.I(ConsoleColor.DarkMagenta, "建立连接：{0}", id);
        }

        public override void Send(object data)
        {
            var dataType = data.GetType();
            MsgAttribute att;
            try
            {
                att = (MsgAttribute)dataType.GetCustomAttributes(_msgAtt, false)[0];
            }
            catch(Exception e)
            {
                throw new Exception($"{dataType.FullName} 不是一个正确的协议数据对象");
            }            

            MsgPackageVO mp = new MsgPackageVO();
            mp.id = att.id;
            mp.content = JsonConvert.SerializeObject(data);
            string msg = JsonConvert.SerializeObject(mp);

            Log.I("发送协议: {0}({1}) \r\n {2}", att.id, att.name, mp.content);

            base.Send(msg);
        }
    }
}
