using System.Net.Sockets;

namespace One.Net
{
    internal class AsyncUserToken
    {
        public AsyncUserToken()
        {
        }

        public Socket Socket { get; internal set; }
    }
}