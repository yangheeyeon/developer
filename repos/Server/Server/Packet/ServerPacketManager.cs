

using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
    {
        //싱글톤 패턴(클래스명으로 접근)
        static PacketManager _instance = new PacketManager();
        public static PacketManager Instance { 
            get 
            {
                if (_instance == null)
                    _instance = new PacketManager();
                return _instance; 
            }
        }
        //프로토콜 아이디 -> 알맞은 함수 실행
        Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
        //패킷 핸들러 연결
        Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

        //딕셔너리에 한번만 등록
        public void Register()
        {
            
_onRecv.Add((ushort)PacketID.C_PlayerInfoReq, MakePacket<C_PlayerInfoReq>);
_handler.Add((ushort)PacketID.C_PlayerInfoReq, PacketHandler.C_PlayerInfoReqHandler);

        }
        public void OnRecvPacket(PacketSession session, ArraySegment<byte> buff)
        {
          
            ushort count = 0;
            //패킷 아이디 -> 함수<패킷 타입> 실행
            ushort size = BitConverter.ToUInt16(buff.Array, buff.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buff.Array, buff.Offset + 2);
            count += 2;
            //함수 탐색
            Action<PacketSession, ArraySegment<byte>> _action = null;
            if(_onRecv.TryGetValue(id , out _action))
            {
                //함수 실행
                _action.Invoke(session, buff);
            }
            

        }
       //패킷타입 핸들러 실행
        void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T: IPacket , new()
        {
            T pkt = new T();
            pkt.Read(buffer);

            Action<PacketSession, IPacket> action = null;

            if (_handler.TryGetValue(pkt.Protocol, out action))
                action.Invoke(session,pkt);
            
        }
    }
