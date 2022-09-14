using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    //[size(2)][packetId(2)][...] [size(2)][packetId(2)][...]
    //패킷 파싱
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;

        //뭉탱이로 온 배열 파싱
        public sealed override int OnRecv(ArraySegment<byte> buff)//ArraySegment struct-type
        {
            int processLen = 0;

            while (true)
            {

                //최소한 Header는 읽을 수 있는지
                if (buff.Count < HeaderSize)
                    break;

                //패킷이 완전체로 도착 했는지
                ushort dataSize = BitConverter.ToUInt16(buff.Array, buff.Offset);
                if (buff.Count < dataSize)
                    break;

                //여기 까지 왔으면 패킷 조립 가능
                OnRecvPacket(new ArraySegment<byte>(buff.Array, buff.Offset, dataSize));


                //버퍼 업데이트
                processLen += dataSize;
                buff = new ArraySegment<byte>(buff.Array, buff.Offset + dataSize, buff.Count - dataSize);

                break;

            }

            return processLen;

        }
        public abstract void OnRecvPacket(ArraySegment<byte> buff);
    }
    public abstract class Session
    {
        RecvBuffer recvBuffer = new RecvBuffer(1024);

        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        object _lock = new object();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        int _disconnected = 0;
        Socket _clientSocket;
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        //컨텐츠 서버와 엔진 서버 분리
        public abstract void OnConnected(EndPoint endpoint);
        public abstract void OnDisconnected(EndPoint endpoint);
        public abstract int OnRecv(ArraySegment<byte> buff);
        public abstract void OnSend(int numOfBytes);

        public void Init(Socket clientSocket)
        {
            
            // 휴대폰
            _clientSocket = clientSocket;
            
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }
        
        public void Send(ArraySegment<byte> sendBuff)
        {
            //다른 차원(세계)
            lock (_lock)
            {
                //일단 대기
                _sendQueue.Enqueue(sendBuff);//ok
                if (_pendingList.Count == 0)
                    RegisterSend();
                

            }
            

        }
        public void Disconnect()
        {
            //멀티 쓰레드 상황
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;
            //컨텐츠 서버
            OnDisconnected(_clientSocket.RemoteEndPoint);

            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }
        #region 네트워크 통신
        public void RegisterSend()
        {
            //멀티 쓰레드 환경
            if (_disconnected == 1)
                return;
            //다른 차원의 낚시대
            while(_sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendingList.Add(buff);
            }
            sendArgs.BufferList = _pendingList;
          

            bool pending = _clientSocket.SendAsync(sendArgs);
            if (pending == false)
                OnSendCompleted(null, sendArgs);
        }
        public void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
                {
                    try
                    {
                        //보내고난 뒤
                        sendArgs.BufferList = null;
                        _pendingList.Clear();

                        OnSend(sendArgs.BytesTransferred);
                        
                        if (_sendQueue.Count > 0)
                            RegisterSend();
                          

                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
                //완료
                
            }
            

        }
        public void RegisterRecv()
        {
            
            recvBuffer.Clean();
            ArraySegment<byte> segment = recvBuffer.WriteSegment;
            recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
            
            bool pending = _clientSocket.ReceiveAsync(recvArgs);
            if (pending == false)
                OnRecvCompleted(null, recvArgs);
        }
        public void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    //write커서 옮기기
                    if (recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }
                    //컨텐츠 쪽에 데이터 넘겨주고 어디까지 읽었는지 확인(readPos)
                    int processLen = OnRecv(recvBuffer.ReadSegment);
                    if (processLen == 0)
                        Console.WriteLine("why processLen is 0?");
                    if (processLen < 0|| processLen > recvBuffer.DataSize)
                    {
                        Disconnect();
                        return;
                    }
                    //read커서 옮기기
                    if (recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }
                    
                    RegisterRecv();
                }
                catch(Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
                
            }
            else
            {
                Disconnect();
            }

        }
        #endregion
    }
}
