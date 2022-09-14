using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class SendBuffHelper
    {
        //threadLocal Storage for specified data type <T>
        public static ThreadLocal<SendBuff> currentBuff = new ThreadLocal<SendBuff>(() => { return null; });

        public static int ChunkSize { get; set; } = 4096;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (currentBuff.Value == null)
                currentBuff.Value = new SendBuff(ChunkSize);

            //버퍼가 부족하면 다시 할당
            if(reserveSize > currentBuff.Value.FreeSize)
                currentBuff.Value = new SendBuff(ChunkSize);

            return currentBuff.Value.Open(reserveSize);
            

        }
        public static ArraySegment<byte> Close(int usedSize)
        {
            return currentBuff.Value.Close(usedSize);
        }

    }
    public class SendBuff
    {
        byte[] _buffer;
        int _usedSize = 0;
        public int FreeSize { get { return _buffer.Length - _usedSize; } }
        public SendBuff(int chunckSize)
        {
            _buffer = new byte[chunckSize];
        }
        public ArraySegment<byte> Open(int reserveSize)
        {
            //예약 버퍼
            if (reserveSize > FreeSize)
                return null;

            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
            return segment;
        }
        public ArraySegment<byte> Close(int usedSize)
        {
            
            //실제 쓴 버퍼
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;
            return segment;
            
        }
    }
}
