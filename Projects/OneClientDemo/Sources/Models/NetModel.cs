using Newtonsoft.Json;
using One;
using Share;
using System;

namespace OneClient
{
    class NetModel : BaseModel
    {
        public readonly WebSocketClient ws = new WebSocketClient();

        public readonly MsgInfoTable msgInfoTable = new MsgInfoTable();

        int _requestId = 1;

        public NetModel()
        {
            ws.onOpen += OnOpen;
            ws.onMessage += OnMessage;
            ws.onClose += OnClose;
            ws.onError += OnError;
        }

        private void OnClose()
        {
            
        }

        private void OnError()
        {
            
        }

        private void OnMessage(object msgObj)
        {
            MsgPackageVO mp = null;
            try
            {
                string msg = (string)msgObj;
                mp = JsonConvert.DeserializeObject<MsgPackageVO>(msg);
            }
            catch (Exception e)
            {
                Log.E($"服务器发过来的协议不合法:{msgObj}");
                ws.Close();
            }

            var msgInfo = msgInfoTable.GetMsgInfo(mp.id);
            if (null != msgInfo)
            {
                Log.I("收到协议: [0]{1} \r\n {2}", mp.id, msgInfo.name, mp.content);

                var dataObj = JsonConvert.DeserializeObject(mp.content, msgInfo.dataType);
                var receiverIns = Activator.CreateInstance(msgInfo.receiverType);
                msgInfo.receiveMethodInfo.Invoke(receiverIns, new object[] { mp.requestId, dataObj });
            }
            else
            {
                Log.E($"协议[Id:{mp.id}]没有匹配的信息");
            }
        }

        private static void OnOpen()
        {

        }

        public void Send<TData>(TData data)
        {
            var dataType = data.GetType();
            var msgInfo = msgInfoTable.GetMsgInfo(dataType);
            if (null == msgInfo)
            {
                throw new Exception($"{dataType.FullName} 不是一个正确的协议数据对象");
            }

            MsgPackageVO mp = new MsgPackageVO();
            mp.id = msgInfo.id;
            mp.requestId = _requestId++;
            mp.content = JsonConvert.SerializeObject(data);
            string msg = JsonConvert.SerializeObject(mp);            
                
            Log.I("发送协议: [{0}]{1} \r\n {2}", msgInfo.id, msgInfo.name, mp.content);

            ws.Send(msg);
        }


        protected override void Reset()
        {

        }
    }
}
