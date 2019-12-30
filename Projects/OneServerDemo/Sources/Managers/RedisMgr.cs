using System;
using Jing;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace OneServer
{
    /// <summary>
    /// Redis管理工具
    /// </summary>
    class RedisMgr : ASingleton<RedisMgr>
    {
        /// <summary>
        /// 连接通道
        /// </summary>
        public ConnectionMultiplexer conn { get; private set; }

        public RedisConfigVO cfg { get; private set; }

        protected override void Init()
        {
            cfg = Global.Ins.core.settings.redis;
            Connect();
        }

        public override void Destroy()
        {

        }

        public void Connect()
        {
            if (null != conn && (conn.IsConnecting || conn.IsConnected))
            {
                return;
            }
            ConfigurationOptions co = ConfigurationOptions.Parse(cfg.address);
            co.Password = cfg.pwd;
            conn = ConnectionMultiplexer.Connect(co);
        }        

        public void Save(string key, object value, int db = RedisDBDefine.SYSTEM)
        {
            var valueJson = JsonConvert.SerializeObject(value);
            Save(key, valueJson, db);
        }

        public T Load<T>(string key, int db = RedisDBDefine.SYSTEM)
        {
            string valueJson = Load(key, db);
            return valueJson == null ? default(T) : JsonConvert.DeserializeObject<T>(valueJson);
        }

        public void Save(string key, string value, int db = RedisDBDefine.SYSTEM)
        {
            conn.GetDatabase(db).StringSet(key, value);
        }

        public string Load(string key, int db = RedisDBDefine.SYSTEM)
        {
            return conn.GetDatabase(db).StringGet(key);
        }

        public void SaveAsync(string key, object value, Action<bool> onComplete, int db = RedisDBDefine.SYSTEM)
        {
            var valueJson = JsonConvert.SerializeObject(value);
            SaveAsync(key, valueJson, onComplete, db);
        }

        public void SaveAsync(string key, string value, Action<bool> onComplete, int db = RedisDBDefine.SYSTEM)
        {
            var task = conn.GetDatabase(db).StringSetAsync(key, value);
            task.ContinueWith((t) =>
            {
                //线程同步到逻辑线程
                Global.Ins.core.RunOnMainThread(() =>
                {
                    onComplete?.Invoke(t.Result);
                });
            });
        }

        public void LoadAsync(string key, Action<string> onComplete, int db = RedisDBDefine.SYSTEM)
        {            
            var task = conn.GetDatabase(db).StringGetAsync(key);
            task.ContinueWith((rv) =>
            {
                //线程同步到逻辑线程
                Global.Ins.core.RunOnMainThread(() =>
                {
                    onComplete?.Invoke(rv.Result);
                });
            });
        }

        public void LoadAsync<T>(string key, Action<T> onComplete, int db = RedisDBDefine.SYSTEM)
        {
            LoadAsync(key, (s) =>
            {
                var value = s == null ? default(T) : JsonConvert.DeserializeObject<T>(s);
                onComplete?.Invoke(value);
            }, db);
        }

    }
}
