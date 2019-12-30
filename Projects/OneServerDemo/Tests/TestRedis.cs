using Newtonsoft.Json;
using One;
using System.Collections.Generic;

namespace OneServer.Tests
{
    class TestRedis
    {
        public TestRedis()
        {
            RedisMgr.Ins.LoadAsync<TTT>("test_redis", OnLoad);

            TTT t = new TTT();
            Log.I("开始测试Rediss");
            RedisMgr.Ins.SaveAsync("test_redis", t, OnSave);
        }

        private void OnSave(bool obj)
        {
            Log.I("保存成功");
            RedisMgr.Ins.LoadAsync<TTT>("test_redis", OnLoad);
        }

        private void OnLoad(TTT obj)
        {
            Log.I("读取成功");
            Log.I(JsonConvert.SerializeObject(obj,Formatting.Indented));
        }
    }
}
