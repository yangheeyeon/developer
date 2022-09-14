using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int readPos;
        int writePos;
        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }
        //데이터 유효범위 계산
        public int DataSize { get { return writePos - readPos; } }//헷갈리면 writePos & readPos == 0 일때
        //쓸수 있는 범위
        public int FreeSize { get { return _buffer.Count - writePos; } }
        //읽는 부분
        public ArraySegment<byte> ReadSegment
        { 
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + readPos, DataSize); } 
        }
        //쓰는 부분
        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + writePos, FreeSize); }
        }
        //[ ][ ][ ][ ][r][ ][w][ ] ==> [r][ ][w][ ][ ][ ][ ][ ]
        public void Clean()
        {
            int dataSize = DataSize;
            if(dataSize == 0)
            {
                //읽을 남은 데이터가 없으면 커서 위치만 이동
                readPos = writePos = 0;
            }
            else
            {
                //남은 찌끄레기가 있으면 시작위치로 복사
                Array.Copy(_buffer.Array, _buffer.Offset + readPos, _buffer.Array, _buffer.Offset, DataSize);
                readPos = 0;
                writePos = dataSize;

            }
        }
        //데이터를 읽고/쓰고 난 뒤 
        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;

            readPos += numOfBytes;
            return true;
        }
        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;

            writePos += numOfBytes;
            return true;
        }
    }
}
