
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
    {
        //<id , req패킷을 어떻게 처리 할지>
        static Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
        static Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

        static PacketManager _instance = new PacketManager();

        public static PacketManager Instance
        {
            get
            {
                return _instance;
            }
        }
        PacketManager()
        {
            Register();
        }

        public void Register()
        {
            
        _onRecv.Add((ushort)PacketId.c_chat, MakePacket<c_chat>);
        _handler.Add((ushort)PacketId.c_chat,PacketHandler.c_chatHandler);

        }
        public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
        {
            ushort count = 0;
            count += 2;
            ushort packetid = BitConverter.ToUInt16(buffer.Array, count);
            count += 2;

            Action<PacketSession, ArraySegment<byte>> action;
            if(_onRecv.TryGetValue(packetid ,out action))
            {
                action.Invoke(session, buffer);
            }

        }
        public static void MakePacket<T>(PacketSession session,ArraySegment<byte> buffer)where T :IPacket,new()
        {
            T p = new T();
            p.Read(buffer);

            Action<PacketSession, IPacket> action;
            if(_handler.TryGetValue((ushort)p.Protocol, out action))
            {
                action.Invoke(session,p);
            }

        }


      
    }
