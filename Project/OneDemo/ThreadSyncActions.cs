using System;
using System.Collections.Generic;
using System.Text;

namespace OneDemo
{
    /// <summary>
    /// 在线程A中通过AddToSyncAction将Action添加进来，在线程B中通过RunSyncActions来执行这些方法。
    /// </summary>
    class ThreadSyncActions
    {
        List<Action> _toSyncActinList = new List<Action>();
        List<Action> _actionCacheList = new List<Action>();

        public void AddToSyncAction(Action action)
        {
            lock (_toSyncActinList)
            {
                _toSyncActinList.Add(action);
            }
        }

        public void RunSyncActions()
        {           
            lock (_toSyncActinList)
            {
                _actionCacheList.AddRange(_toSyncActinList);
                _toSyncActinList.Clear();
            }

            for (int i = 0; i < _actionCacheList.Count; i++)
            {
                _actionCacheList[i].Invoke();
            }

            _actionCacheList.Clear();
        }
    }
}
