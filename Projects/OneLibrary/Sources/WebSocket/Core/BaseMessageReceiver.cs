﻿using Google.Protobuf;

namespace One.WebSocket
{
    class BaseMessageReceiver<TProtobuf> where TProtobuf: Google.Protobuf.IMessage,new()
    {
        public BaseMessageReceiver()
        {

        }

        void OnReceiveData(byte[] data)
        {
            var pbObj = new TProtobuf();
            pbObj.MergeFrom(data);
            OnReceive(pbObj);
        }

        protected virtual void OnReceive(TProtobuf data)
        {
            
        }
    }
}
