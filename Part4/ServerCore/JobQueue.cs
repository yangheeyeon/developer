using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    //어떤 능력을 상속 받는 개념
    public interface IJobQueue
    {
        void Push(Action job);
    }
    public class JobQueue : IJobQueue
    {
        Queue<Action> jobQueue = new Queue<Action>();
        object _lock = new object();
        bool _flush = false;

        public void Push(Action job)
        {

            bool flush = false;

            lock (_lock)
            {
                jobQueue.Enqueue(job);

                if (_flush == false)
                    _flush = flush = true;
                
            }
            //하나의 쓰레드만 아는 정보
            if (flush)
                Flush();

        }
        void Flush()
        {
            while(true)
            {
                Action _job = Pop();
                if (_job == null)
                    return;
                _job.Invoke();
            }

        }

        Action Pop()
        {
            lock (_lock)
            {
                if (jobQueue.Count == 0)
                {
                    _flush = false;
                    return null;
                }
                   
                return jobQueue.Dequeue();
            }
        }
    }
}
