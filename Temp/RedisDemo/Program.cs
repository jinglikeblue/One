using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RedisDemo
{
    class TTT
    {
        public string a = "中文";
        public ulong b = 1;
        public long c = 2;
        public int d = 3;
        public uint e = 4;
        public float f = 0.1f;
        [JsonIgnore]
        public double g = 0.2f;
        public List<int> h = new List<int>(new int[] { 1, 2, 3, 4 });
        public Dictionary<string, TTT> dic = new Dictionary<string, TTT>();

        public TTT()
        {

        }

        public void InitDic()
        {
            dic.Add("a", new TTT());
            dic.Add("b", new TTT());
            dic.Add("c", new TTT());
        }
    }

    class Program
    {



        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var t = new TTT();
            t.InitDic();

            string json = JsonConvert.SerializeObject(t, Formatting.Indented);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for(int i = 0; i  < 1000; i++)
            {
                var a = JsonConvert.SerializeObject(t);                
            }
            sw.Stop();
            Console.WriteLine("Newtonsoft: {0} \r\n {1}", sw.ElapsedMilliseconds, json);

            //sw.Start();
            //for (int i = 0; i < 1000; i++)
            //{                
            //    var b = LitJson.JsonMapper.ToJson(t);
            //}
            //sw.Stop();
            //Console.WriteLine("LitJson: {0}", sw.ElapsedMilliseconds);

            //new RedisTest();
        }
    }
}
