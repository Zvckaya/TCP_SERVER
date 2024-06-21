using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tcp_Server_Core;

namespace Tcp_Server
{
    class Packet  //패킷 헤더
    {
        public ushort size;
        public ushort packetId;
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;
    }

    class PlayerInfoOk : Packet
    {
        public int hp;
        public int attack;
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2
    }


    class ClientSession : PacketSession
    {
        //왜 구현하는가 ? 엔진과 컨텐츠를 분리하기 위함 
        //컨텐츠측에서는 세션이아닌 게임 세션을 만들어서 사용 
        public override void OnConnected(EndPoint endPoint)
        {

            Console.WriteLine($"OnConnected : {endPoint}");

            //            Packet packet = new Packet() { size = 100, packetId = 10 };
            //;
            //            ArraySegment<byte> openSegment =  SendBufferHelper.Open(4096);
            //            byte[] buffer = BitConverter.GetBytes(packet.size);
            //            byte[] buffer2 = BitConverter.GetBytes(packet.packetId);

            //            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //            ArraySegment<byte> sendBuff =  SendBufferHelper.Close(buffer.Length + buffer2.Length);

            //            Send(sendBuff);

            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"On Disconnected :{endPoint}");
        }

        //public override int OnRecv(ArraySegment<byte> buffer)
        //{
        //    string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        //    Console.WriteLine($"[From Client] {recvData}");
        //    return buffer.Count;
        //}

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        long playerId = BitConverter.ToInt64(buffer.Array, buffer.Offset + count);
                        count += 8;
                        Console.WriteLine($"Player InfoReq: {playerId}");
                    }
                    break;
                case PacketID.PlayerInfoOk:
                    {

                    }
                    break;

            }

            Console.WriteLine($"Recv Size {size} RecvId {id}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }

}
