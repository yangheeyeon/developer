using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
    //재귀적 락을 허용? -> yes->
    //writelock-> readlock(o), writelock -> writelock (o), readlock->writelock(x)
    //스핀락 (5000 -> Yield)
    
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_FLAG = 0x7FFF0000;
        const int READ_FLAG = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;
        int flag = EMPTY_FLAG;
        int _writeCount = 0;
        //[Unused(1)][WriteThreadId(15)][ReadCount(16)]
        public void WriteLock()
        {
            //같은 쓰레드가 또 writelock 
            if (Thread.CurrentThread.ManagedThreadId == (flag & WRITE_FLAG))
                _writeCount++;


            //아무도 writeLock또는 readLock을 가지지 않을 때, 경합해서 writeLock 획득
            while (true)
            {
                int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_FLAG;
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    //WriteThreadId(15)
                    if (Interlocked.CompareExchange(ref flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                        

                }
                Thread.Yield();
            }

            

        }
        public void WriteUnlock()
        {
            _writeCount--;
            if(_writeCount == 0)
                Interlocked.Exchange(ref flag, EMPTY_FLAG);

        }
        public void ReadLock()
        {
            //아무도 writelock을 갖고 있지 않으면 readCount++
            while (true)
            {
                for(int i=0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = flag & READ_FLAG;
                    if (Interlocked.CompareExchange(ref flag, expected + 1, expected) == expected)
                        return;
                }

                Thread.Yield();
            }
            
        }
        public void ReadUnlock()
        {
            Interlocked.Decrement(ref flag);
        }
    }
}
