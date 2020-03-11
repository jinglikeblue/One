using System;
using System.Collections.Generic;
using System.Text;

namespace One.WebSocket
{
    /// <summary>
    /// 服务器
    /// </summary>
    public class Server
    {
        public string host { get; private set; } = "0.0.0.0";
        public int port { get; private set; }
        public string Url
        {
            get
            {
                return string.Format("ws://{0}:{1}", host, port);
            }
        }

        WebSocketSharp.Server.WebSocketServer _server;

        public Server(int port)
        {            
            this.port = port;

            _server = new WebSocketSharp.Server.WebSocketServer(Url);
            _server.Log.Level = WebSocketSharp.LogLevel.Error;
            _server.AddWebSocketService<Session>("/", onCreateSession);            
        }

        private void onCreateSession(Session session)
        {
            
        }

        public void Start()
        {
            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();            
        }
    }
}
