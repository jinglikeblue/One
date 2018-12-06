using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Util
{
    /// <summary>
    /// 在线程A中通过AddToSyncAction将Action添加进来，在线程B中通过RunSyncActions来执行这些方法。
    /// </summary>
    public class ThreadSyncActions
    {
        /// <summary>
        /// 线程安全的先入先出队列
        /// </summary>
        ConcurrentQueue<Action> _syncActionsQueue = new ConcurrentQueue<Action>();

        /// <summary>
        /// 如果不同的线程并发调用该方法，无法保证添加的Action是有序的
        /// </summary>
        /// <param name="action"></param>
        public void AddToSyncAction(Action action)
        {
            _syncActionsQueue.Enqueue(action);
        }

        /// <summary>
        /// 如果不同的线程并发调用该方法，无法保证添加的方法是按照先进先出的顺序执行
        /// </summary>
        public void RunSyncActions()
        {
            Action action;
            while(_syncActionsQueue.TryDequeue(out action))
            {
                action.Invoke();
            }
        }
    }
}
