using System.Collections.Generic;
using UnityEngine;

namespace Ly.Tools.Timer
{
    public class TimerComponent : MonoBehaviour
    {
        private TimerClass _timer;

        private List<string> m_eventQueueList;

        public int paramOffset
        {
            get { return _timer.param; }
            set { _timer.param = value; }
        }

        private void Awake()
        {
            _timer = new TimerClass();
            m_eventQueueList = new List<string>();
            _timer.TimeUpEvent += m_TimeUpEventCallBack;
        }

        private void Update()
        {
            if (m_eventQueueList.Count > 0)
            {
                var count = m_eventQueueList.Count;
                for (var idex = 0; idex < count; ++idex)
                {
                    var tag = m_eventQueueList[0];
                    m_eventQueueList.RemoveAt(0);
                    TimeUpEvent?.Invoke(this, tag);
                }
            }
        }

        private void OnApplicationQuit()
        {
            Debug.Log("~quit timer");
            _timer.TimeUpEvent -= m_TimeUpEventCallBack;
            _timer.Stop();
        }

        public event TimerClass.TimeUpDelegate TimeUpEvent;

        /// <summary>
        ///     普通计时器任务
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="spanTime"></param>
        /// <param name="startTime"></param>
        public void AddTask(string tag, long spanTime, long startTime = 0)
        {
            _timer.AddTask(tag, spanTime, startTime);
        }

        /// <summary>
        ///     自定义时间节点计时器
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="loopSpanTimes">间隔时间数组(毫秒)</param>
        public void AddTask(string tag, List<int> loopSpanTimes)
        {
            _timer.AddTask(tag, loopSpanTimes);
        }


        public void StartTimer(long startTimestamp = 0)
        {
            _timer.Start(startTimestamp);
        }

        public void RemoveTask(string taskTag)
        {
            _timer.RemoveTask(taskTag);
        }

        public void Kill()
        {
            _timer.Stop();
        }

        private void m_TimeUpEventCallBack(object sender, string tag)
        {
            m_eventQueueList.Add(tag);
        }

        public static long GetCurTimetamp()
        {
            return TimerClass.GetCurrentTimestamp();
        }

        public static long GetDeltaMilliSecond(long startTicks, long endTicks)
        {
            return TimerClass.GetDeltaMilliSecond(startTicks, endTicks);
        }
    }
}