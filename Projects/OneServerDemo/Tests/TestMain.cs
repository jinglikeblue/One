using System.Collections.Generic;

namespace OneServer.Tests
{
    class TTT
    {
        public string a = "中文";
        public ulong b = 1;
        public long c = 2;
        public int d = 3;
        public uint e = 4;
        public float f = 0.1f;
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

    class TestMain
    {
        public TestMain()
        {
            new TestRedis();
        }
    }
}
