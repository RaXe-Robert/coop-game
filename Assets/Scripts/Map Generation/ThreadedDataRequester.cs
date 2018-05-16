using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

namespace Assets.Scripts.Map_Generation
{
    public class ThreadedDataRequester : MonoBehaviour
    {
        private static ThreadedDataRequester instance;

        private Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

        private void Awake()
        {
            instance = FindObjectOfType<ThreadedDataRequester>();
        }

        private void Update()
        {
            lock (dataQueue)
            {
                while (dataQueue.Count > 0)
                {
                    ThreadInfo threadInfo = dataQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }

        public static void RequestData(Func<object> generateData, Action<object> callback)
        {
            ThreadStart threadStart = delegate
            {
                instance.DataThread(generateData, callback);
            };

            new Thread(threadStart).Start();
        }

        private void DataThread(Func<object> generateData, Action<object> callback)
        {
            object data = generateData();
            lock (dataQueue)
                dataQueue.Enqueue(new ThreadInfo(callback, data));
        }

        private struct ThreadInfo
        {
            public readonly Action<object> callback;
            public readonly object parameter;

            public ThreadInfo(Action<object> callback, object parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }
    }
}
