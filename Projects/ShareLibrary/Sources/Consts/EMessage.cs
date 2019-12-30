﻿using System;
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
        //请求部分
        PING,        
        LOGIN_REQUEST,
    }

    public enum ES2C
    {
        //响应部分
        PONG,
        LOGIN_RESPONSE,

        //推送部分
        UTC_PUSH,
    }
}