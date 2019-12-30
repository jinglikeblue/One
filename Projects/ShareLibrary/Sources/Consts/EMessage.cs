using System;
using System.Collections.Generic;
using System.Text;

namespace Share
{
    public enum EMsgType { 
        C2S,
        S2C,
    }


    public enum EC2S
    {
        PING,
        LOGIN_REQUEST
    }

    public enum ES2C
    {
        PONG,
        LOGIN_RESPONSE
    }
}
