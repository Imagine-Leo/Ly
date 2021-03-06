using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Ly.Tools.Timer
{
    public class TimerClass
    {
        public delegate void TimeUpDelegate(object sender, string tag);

        private long m_curTicks;

        private bool m_quitBool;
        private List<Task> m_removeList = new List<Task>();
        private long m_startTicks;

        //private const string DEBUGFORMAT = "yyyy/MM/dd/HH:mm:ss.ffffff";
        private List<Task> m_taskList = new List<Task>();
        private Thread m_taskThread;

        public int param = 20;

        public event TimeUpDelegate TimeUpEvent;

        //TODO实现暂停功能
        // private bool m_pauseBool = false;

        /// <summary>
        ///     添加循环时间任务
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="spanTime"></param>
        /// <param name="startTime"></param>
        public void AddTask(string tag, long spanTime, long startTime = 0)
        {
            var _task = new Task(tag, spanTime, startTime);
            var temp = m_taskList.Find(tar => { return tar.tag == _task.tag; });
            if (temp != null)
            {
                var newTag = _task.tag + Guid.NewGuid();
                _task = new Task(newTag, spanTime, startTime);
            }

            m_taskList.Add(_task);
        }


        /// <summary>
        ///     添加自定义循环规则任务
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="loopSpanTimes"></param>
        public void AddTask(string tag, List<int> loopSpanTimes)
        {
            var _task = new Task(tag, loopSpanTimes);
            var temp = m_taskList.Find(tar => { return tar.tag == _task.tag; });
            if (temp != null)
            {
                var newTag = _task.tag + Guid.NewGuid();
                _task = new Task(newTag, loopSpanTimes);
            }

            m_taskList.Add(_task);
        }


        /// <summary>
        ///     开启时间计时器
        /// </summary>
        /// <param name="startTimestamp"></param>
        public void Start(long startTimestamp = 0)
        {
            _StartTaskThread(startTimestamp == 0 ? Stopwatch.GetTimestamp() : startTimestamp);
        }

        /// <summary>
        ///     关闭时间计时器
        /// </summary>
        public void Stop()
        {
            m_quitBool = true;
            if (m_taskThread != null)
            {
                Debug.Instance.DllLog("timer thread is abort .", LogType.UnityLogWarning);
                m_taskThread.Abort();
            }
        }

        /// <summary>
        ///     移除已经添加进队列的任务
        /// </summary>
        /// <param name="taskTag"></param>
        public void RemoveTask(string taskTag)
        {
            m_removeList.Add(new Task(taskTag));
        }

        /// <summary>
        ///     开启计时器任务队列
        /// </summary>
        /// <param name="startTimestamp"></param>
        private void _StartTaskThread(long startTimestamp)
        {
            m_startTicks = startTimestamp;
            m_taskThread = new Thread(() =>
            {
                while (true)
                {
                    if (!m_quitBool)
                    {
                        if (m_taskList.Count == 0) continue;

                        if (m_removeList.Count != 0) //TODO:
                            for (var idex = 0; idex < m_removeList.Count; ++idex)
                            {
                                var tempTask = m_taskList.Find(task => { return task.tag == m_removeList[0].tag; });
                                if (tempTask != null)
                                {
                                    Debug.Instance.DllLog("手动移除：" + tempTask.tag);
                                    m_taskList.Remove(tempTask);
                                }
                                else
                                {
                                    Debug.Instance.DllLog("手动移除失败：" + m_removeList[0].tag);
                                }

                                m_removeList.RemoveAt(0);
                            }

                        m_curTicks = Stopwatch.GetTimestamp();
                        var _totalMilliSecond = (m_curTicks - m_startTicks) * 1000 / Stopwatch.Frequency;
                        for (var index = 0; index < m_taskList.Count; ++index)
                            if (_totalMilliSecond + param >= m_taskList[index].nextTimePoint)
                            {
                                TimeUpEvent?.Invoke(this, m_taskList[index].tag);
                                if (!m_taskList[index].CanMoveNextTime())
                                {
                                    Debug.Instance.DllLog("计时器任务结束:" + m_taskList[index].tag);
                                    m_taskList.RemoveAt(index);
                                }
                            }
                    }
                    else
                    {
                        break;
                    }

                    Thread.Sleep(10);
                }
            });
            m_taskThread.IsBackground = true;
            m_taskThread.Start();
        }

        /// <summary>
        ///     用于在高精度计算式获取开启程序时的值Stopwatch.GetTimestamp()
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentTimestamp()
        {
            return Stopwatch.GetTimestamp();
        }

        public static long GetDeltaMilliSecond(long startTicks, long endTicks)
        {
            return (endTicks - startTicks) * 1000 / Stopwatch.Frequency;
        }

        private class Task
        {
            //Custom Loop times
            public readonly List<int> customLoopSpanTimes;
            public readonly bool loop;

            //Loop
            public readonly long spanTime_miliSeconds;
            public readonly string tag;
            private List<int> m_customLoopSpanTimes;

            /// <summary>
            ///     Loop
            /// </summary>
            /// <param name="tag"></param>
            /// <param name="spanTime_miliSeconds"></param>
            /// <param name="startPonit_miliSeconds"></param>
            /// <param name="loop"></param>
            public Task(string tag, long spanTime_miliSeconds, long startPonit_miliSeconds)
            {
                this.tag = tag;
                this.spanTime_miliSeconds = spanTime_miliSeconds;
                nextTimePoint = startPonit_miliSeconds;
                loop = true;
            }

            /// <summary>
            ///     Custom loop times
            /// </summary>
            /// <param name="tag"></param>
            /// <param name="loopSpanTimes"></param>
            /// <param name="loop"></param>
            public Task(string tag, List<int> loopSpanTimes)
            {
                this.tag = tag;
                m_customLoopSpanTimes =
                    customLoopSpanTimes = loopSpanTimes;
                nextTimePoint = 0;
                loop = false;
            }

            public Task(string tag)
            {
                this.tag = tag;
            }

            public long nextTimePoint { get; set; }

            public bool CanMoveNextTime()
            {
                if (loop)
                {
                    nextTimePoint += spanTime_miliSeconds;
                    return true;
                }

                if (m_customLoopSpanTimes.Count > 0)
                {
                    nextTimePoint += m_customLoopSpanTimes[0];
                    m_customLoopSpanTimes.RemoveAt(0);
                    return true;
                }

                Debug.Instance.DllLog("m_customLoopSpanTimes null?", LogType.UnityLogError);
                return false;
            }
        }
    }
}