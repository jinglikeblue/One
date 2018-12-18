﻿using One.Protocol;
using System;
using System.Net.Sockets;

namespace One.Net
{
    /// <summary>
    /// 提供基于WebSocket协议的套接字服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebSocketServer:TcpServer<WebSocketProtocolProcess>
    {
        protected override TcpReomteProxy CreateRemoteProxy(Socket clientSocket, int bufferSize, Action<TcpReomteProxy> onShutdown)
        {
            return new WebSocketRemoteProxy(clientSocket, new WebSocketProtocolProcess(), bufferSize, onShutdown);
        }
    }
}
