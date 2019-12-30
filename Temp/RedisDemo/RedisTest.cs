using Jing;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RedisDemo
{
    class RedisTest:ASingleton<RedisTest>
    {
        /// <summary>
        /// 连接通道
        /// </summary>
        public ConnectionMultiplexer conn { get; private set; }

        protected override void Init()
        {
            var watch = new Stopwatch();



            ConfigurationOptions co = ConfigurationOptions.Parse("47.52.33.131:9530");
            co.Password = "!QAZ2wsx";

            
            

            watch.Start();
            conn = ConnectionMultiplexer.Connect(co);
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);

            watch.Start();
            for(int i = 0; i < 100; i++)
            {
                SaveTestData(i);
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);

            Console.ReadKey();


            //watch.Start();
            
            //watch.Stop();
            //Console.WriteLine(watch.ElapsedMilliseconds);


            //Console.WriteLine(value);
        }

        void SaveTestData(int index)
        {
            var rKey = "test_" + Guid.NewGuid().ToString("N"); 
            var rValue = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");            
            var task = conn.GetDatabase().StringSetAsync(rKey, rValue);
            task.ContinueWith(OnSet, null);
            //conn.GetDatabase().set
        }

        private void OnSet(Task<bool> arg1, object arg2)
        {
            Console.WriteLine("Set");            
        }

        void LoadTestData()
        {
            //RedisValue value = conn.GetDatabase().StringGet(rKey);
        }

        public override void Destroy()
        {
            
        }
    }
}
