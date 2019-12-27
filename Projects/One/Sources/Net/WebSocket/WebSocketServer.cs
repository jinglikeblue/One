namespace One
{
    /// <summary>
    /// 提供基于WebSocket协议的套接字服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebSocketServer : BaseServer
    {
        public static WebSocketServer current { get; private set; }

        WebSocketSharp.Server.WebSocketServer _server;

        public string host { get; private set; } = "0.0.0.0";
        public int port { get; private set; }
        public string Url
        {
            get
            {
                return string.Format("ws://{0}:{1}", host, port);
            }
        }

        public WebSocketServer(int port)
        {
            current = this;
            this.port = port;

            _server = new WebSocketSharp.Server.WebSocketServer(Url);
            _server.Log.Level = WebSocketSharp.LogLevel.Error;
            _server.AddWebSocketService<RootBehavior>("/");

            RegisterSeesionType(typeof(WebSocketSession));
        }

        public override void Start()
        {
            _server.Start();
        }

        public override void Stop()
        {
            _server.Stop();
        }
    }
}
