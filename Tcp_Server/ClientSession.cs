using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tcp_Server_Core;


namespace Tcp_Server
{
    public abstract class Packet  //패킷 헤더
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    public class PlayerInfoReq : Packet
    {
        public long playerId;

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> s)
        {


            ushort count = 0;

            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            count += 2;
            //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;
            this.playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));  //안전한 버전(범위검사)

            count += 8;
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> s = SendBufferHelper.Open(4096); //사이즈 예약
            bool success = true;
            ushort count = 0;

            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId); // 공간이 모자르면 실패 
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId); // 공간이 모자르면 실패 
            count += 8;

            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count); // 사이즈 적어주기

            if (success == false)
                return null;

            //복사배열 생성이 아닌 미리 만들어진 버퍼로 관리

            ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);
            return sendBuff;
        }
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
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);
                        Console.WriteLine($"Player InfoReq: {p.playerId}");
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
