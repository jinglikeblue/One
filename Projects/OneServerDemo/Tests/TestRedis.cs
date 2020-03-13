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
            OneLog.I("开始测试Rediss");
            RedisMgr.Ins.SaveAsync("test_redis", t, OnSave);
            RedisMgr.Ins.conn.GetDatabase().HashSet("a", "a1", "a11");
            OneLog.I("HashSet保存成功");
            //RedisMgr.Ins.conn.GetDatabase().HashSet("a", "a1", "a12");
            //RedisMgr.Ins.conn.GetDatabase().HashSet("a", "b1", "b11");
            //RedisMgr.Ins.conn.GetDatabase().HashSet("a", "b1", "b12");

            var db = RedisMgr.Ins.conn.GetDatabase(3);
            db.StringSetAsync("t", "1");
            db.StringSet("t", "2");
        }

        private void OnSave(bool obj)
        {
            OneLog.I("保存成功");
            RedisMgr.Ins.LoadAsync<TTT>("test_redis", OnLoad);
        }

        private void OnLoad(TTT obj)
        {
            OneLog.I("读取成功");
            OneLog.I(JsonConvert.SerializeObject(obj,Formatting.Indented));
        }
    }
}
