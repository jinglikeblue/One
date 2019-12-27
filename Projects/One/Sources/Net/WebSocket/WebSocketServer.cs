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

        public WebSocketServer(int port)
        {
            current = this;            

            var url = string.Format("ws://0.0.0.0:{0}", port);
            _server = new WebSocketSharp.Server.WebSocketServer(url);                  
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
