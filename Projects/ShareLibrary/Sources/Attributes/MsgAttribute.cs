using System;

namespace Share
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MsgAttribute : Attribute
    {
        public int id { get; private set; }

        public string name { get; private set; }

        public EMsgType type { get; private set; }        

        public MsgAttribute(ES2C msgId)
        {
            this.id = (int)msgId;
            name = msgId.ToString();
            type = EMsgType.S2C;
        }

        public MsgAttribute(EC2S msgId)
        {
            this.id = (int)msgId;
            name = msgId.ToString();
            type = EMsgType.C2S;
        }
    }
}
