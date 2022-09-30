using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    class RecvBuffer
    {
        //[r][][][][w][][][][][]
        ArraySegment<byte> _buffer;
        int readPos = 0;
        int writePos = 0;
        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);

        }
        public int DataSize { get { return writePos - readPos; } }
        public int FreeSize { get { return _buffer.Count - writePos; } }

        //찝어주기!
        public ArraySegment<byte> ReadSegment {

            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + readPos, DataSize); } 

        }
        public ArraySegment<byte> WriteSegment {

            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + writePos, FreeSize); } 

        }
        //읽기를 완료함
        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;
            readPos += numOfBytes;
            return true;
        }
        //쓰기를 완료함
        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;
            writePos += numOfBytes;
            return true;
        }
        //버퍼 정리하자!
        public void Clean()
        {
            //남은 데이터가 없으면
            int dataSize = DataSize;
            if(dataSize == 0)
            {
                readPos = writePos = 0;

            }
            //남은 찌끄레기 있으면
            else
            {
                Array.Copy(_buffer.Array, _buffer.Offset + readPos, _buffer.Array, _buffer.Offset, DataSize);
                readPos = 0;
                writePos = dataSize;
            }
        }


    }
}
