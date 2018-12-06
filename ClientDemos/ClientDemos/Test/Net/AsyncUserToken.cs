using System.Net.Sockets;

namespace One.Test.Net
{
    internal class AsyncUserToken
    {
        public AsyncUserToken()
        {
        }

        public Socket Socket { get; internal set; }
    }
}