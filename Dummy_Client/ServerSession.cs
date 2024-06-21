using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tcp_Server_Core;

namespace Dummy_Client
{  //세션은 대리자의 개념이다.

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

    class ServerSession : Session
    {



        public override void OnConnected(EndPoint endPoint)
        {

            Console.WriteLine($"On Connected :{endPoint}");
         
            PlayerInfoReq packet = new PlayerInfoReq() {  packetId = (ushort)PacketID.PlayerInfoReq, playerId = 1001 };

            ArraySegment<byte> s = SendBufferHelper.Open(4096); //사이즈 예약
            bool success = true;
            ushort count = 0;

            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset+count, s.Count-count), packet.packetId); // 공간이 모자르면 실패 
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset+count, s.Count-count), packet.playerId); // 공간이 모자르면 실패 
            count += 8;

            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count); // 사이즈 적어주기

            //미리 만들어진 버퍼에 복사하지 않고 직접 수정

            ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);


            if (success)
            {
                Send(sendBuff);
            }
            else
            {
                Console.WriteLine("전송 실패.");
            }
         


        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"On Disconnected :{endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }

}
