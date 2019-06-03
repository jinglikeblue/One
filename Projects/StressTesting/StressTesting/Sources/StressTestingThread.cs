using System;
using System.Collections.Generic;
using System.Threading;
using One;

namespace StressTesting.Sources
{
    class StressTestingThread
    {
        public int aliveCount { get; private set; }

        Thread _thread;
        bool _isDispose = false;
        StressTestingVO _vo;
        List<TcpClient> _clients = new List<TcpClient>();        

        public StressTestingThread(StressTestingVO vo)
        {
            _vo = vo;            
        }

        public void Start()
        {
            if (null == _thread)
            {
                _thread = new Thread(Testing);
                _thread.Start();
            }
        }

        private void Testing()
        {
            for(int i = 0; i < _vo.connectionCountPerThread; i++)
            {
                var client = new TcpClient();
                client.Connect(_vo.host, _vo.port, 1024);
                client.onConnectFail += OnConnectFail;
                _clients.Add(client);
                Thread.Sleep(100);
            }

            while(false == _isDispose)
            {
                aliveCount = 0;
                foreach (var client in _clients)
                {
                    client.Refresh();
                    if (client.IsConnected)
                    {
                        aliveCount++;
                        client.Send(_vo.msgData);
                    }
                }

                Thread.Sleep(1000);
            }

            foreach(var client in _clients)
            {
                client.Close(true);
            }
            _clients.Clear();
        }

        private void OnConnectFail(TcpClient obj)
        {
            Log.I("连接失败");
        }

        public void Stop()
        {
            _thread = null;
            _isDispose = true;
        }
    }
}
