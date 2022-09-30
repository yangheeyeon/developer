using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{

    public class SendBuffHelper
    {
        //CurrentBuff.Value == null
        public static ThreadLocal<SendBuff> CurrentBuff = new ThreadLocal<SendBuff>(()=> { return null; });
        public static int ChunckSize { get { return 65535 * 100; } }
        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuff.Value == null)
                CurrentBuff.Value = new SendBuff(ChunckSize);

            if (CurrentBuff.Value.FreeSize < reserveSize)
                CurrentBuff.Value = new SendBuff(ChunckSize);

            return CurrentBuff.Value.Open(reserveSize);

        }
        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuff.Value.Close(usedSize);

        }
    }
    public class SendBuff
    {

        byte[] _buffer;
        int _usedSize = 0;

        public int FreeSize { get { return _buffer.Length - _usedSize; } }

        public SendBuff(int ChunkSize)
        {
            _buffer = new byte[ChunkSize];
        }
        public ArraySegment<byte> Open(int reserveSize)
        {
            if (reserveSize > FreeSize)
                return null;
            return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
        }
        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;

            return segment;
        }

    }
}
