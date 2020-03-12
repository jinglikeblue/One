using System;

namespace One.WebSocket
{
    /// <summary>
    /// 服务器
    /// </summary>
    public class Server
    {
        public event Action<Session> onNewSession;

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

        /// <summary>
        /// 会话管理器
        /// </summary>
        public SessionManager sessionManager { get; private set; }

        public Server(int port)
        {            
            this.port = port;

            sessionManager = new SessionManager();
            _server = new WebSocketSharp.Server.WebSocketServer(Url);
            _server.KeepClean = true;
            _server.Log.Level = WebSocketSharp.LogLevel.Error;
            _server.AddWebSocketService<Behavior>("/", onBehaviorInitialized);
            
        }

        /// <summary>
        /// 当会话被创建时触发
        /// </summary>
        /// <param name="behavior"></param>
        private void onBehaviorInitialized(Behavior behavior)
        {
            var session = new Session(behavior, sessionManager);
            onNewSession?.Invoke(session);
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            _server.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _server.Stop();            
        }
    }
}
