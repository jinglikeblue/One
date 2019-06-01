using System;
using System.Collections.Generic;

namespace One
{
    /// <summary>
    /// 在线程A中通过AddToSyncAction将Action添加进来，在线程B中通过RunSyncActions来执行这些方法。
    /// </summary>
    public class ThreadSyncActions
    {
        List<Action> _toSyncActinList = new List<Action>();         

        public void AddToSyncAction(Action action)
        {
            lock (_toSyncActinList)
            {
                _toSyncActinList.Add(action);
            }
        }

        public void RunSyncActions()
        {
            List<Action> actionCacheList = null;

            lock (_toSyncActinList)
            {
                actionCacheList = _toSyncActinList.GetRange(0, _toSyncActinList.Count);                
                _toSyncActinList.Clear();
            }

            for (int i = 0; i < actionCacheList.Count; i++)
            {
                actionCacheList[i].Invoke();
            }            
        }
    }
}
