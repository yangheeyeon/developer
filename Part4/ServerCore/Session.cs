using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;

            //buffer == WriteSegment
            while (true)
            {
                //최소한 헤더는 파싱할 수 있는지 확인
                if (buffer.Count < HeaderSize)
                    break;
                //완전체 패킷만 받기
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;
                //패킷 조립
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                //처리한 데이터 사이즈 반환

                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);

            }
            return processLen;
        }
        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }
    public abstract class Session
    {
        //다형성!
        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);

        object _lock = new object();

        Socket _socket;
        int disconnected = 0;
        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();

        Queue<ArraySegment<byte>> sendQueue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        //배열 segment

        RecvBuffer recvBuffer = new RecvBuffer(65535);
        public void Start(Socket socket)
        {
            //요청하는측 소켓
            _socket = socket;

            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();

          
        }
     
        public void Send(ArraySegment<byte> sendBuff)
        {
            lock (_lock)
            {
                sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
           
        }
        public void Send(List<ArraySegment<byte>> sendBuffList)
        {
            //sendBuffList 전부 보내기!

            if (sendBuffList == null)
                return;

            lock (_lock)
            {
               
                foreach(ArraySegment<byte> sendBuff in sendBuffList)
                {
                    sendQueue.Enqueue(sendBuff);
                }

              
                
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
            
        }
        public void Clear()
        {
            sendQueue.Clear();
            _pendingList.Clear();
        }
        #region
        public void RegisterSend()
        {
            if (_socket == null || disconnected == 1)
                return;

            while (sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = sendQueue.Dequeue();
                _pendingList.Add(buff);
            }
            sendArgs.BufferList = _pendingList;

           //콜백
            try
            {
                bool pending = _socket.SendAsync(sendArgs);
                if (pending == false)
                    OnSendCompleted(null, sendArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterSend Failed{e}");
            }
            
            
            
        }
        public void OnSendCompleted(object sender,SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                //잘 전송 되었는지 확인!
                if (sendArgs.BytesTransferred > 0 && sendArgs.SocketError == SocketError.Success)
                {
                    try
                    {
                        _pendingList.Clear();
                        sendArgs.BufferList = null;
                        OnSend(args.BytesTransferred);

                        if (sendQueue.Count > 0)
                            RegisterSend();
                        
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed{e}");
                    }
                    

                }
                else
                {
                    
                    //쫒아내기!
                    Disconnect();
                }
            }
          
            

        }
        public void RegisterRecv()
        {

            if (disconnected == 1)
                return;

            recvBuffer.Clean();
            ArraySegment<byte> segment = recvBuffer.WriteSegment;
            recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(recvArgs);
                if (pending == false)
                    OnRecvCompleted(null, recvArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterRecv Failed {e}");
            }
            
        }
        public void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            //받는다
            if(args.BytesTransferred >0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    //write커서 이동
                    if(recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }
                    // 컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다
                    int processLen = OnRecv(recvBuffer.ReadSegment);
                    if (processLen <0 || processLen > recvBuffer.DataSize)
                    {
                        Disconnect();
                        return;
                    }
                    //read커서 이동
                    if (recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }
                    
                    RegisterRecv();
                }
                catch(Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed{e}");
                }
                
            }
            else
            {
                Disconnect();
            }
            
               
            
        }
        public void Disconnect()
        {
            lock (_lock)
            {
                if (Interlocked.Exchange(ref disconnected, 1) == 1)
                    return;

                {
                    //다형성 실현
                    OnDisconnected(_socket.RemoteEndPoint);

                    //쫒아낸다(나가 임마!)
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    Clear();
                }
            }
           
            
        }
        #endregion
    }
}
