using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tcp_Server_Core;

namespace Dummy_Client
{  //세션은 대리자의 개념이다.

    public abstract class Packet  //패킷 헤더
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    public struct SkillInfo
    {
        public int id;
        public short level;
        public float duration;

        public bool Write(Span<byte> s, ref ushort count)
        {
            bool success = true;
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
            count += sizeof(int);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
            count += sizeof(short);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
            count += sizeof(float);

            return true;
        }

        public void Read(ReadOnlySpan<byte> s, ref ushort count)
        {
            id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
            count += sizeof(int);
            level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
            count += sizeof(short);
            duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
            count += sizeof(float);
        }


    }


    public class PlayerInfoReq : Packet
    {
        public long playerId;
        public string name;

      

        // public List<int> skills = new List<int>(); // 

        public List<SkillInfo> skills = new List<SkillInfo>();
        
        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {

            ushort count = 0;

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            count += sizeof(ushort);
            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += sizeof(long);

            //string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            this.name =  Encoding.Unicode.GetString(s.Slice(count,nameLen));
            count += nameLen;

            //skill list
            skills.Clear();
            ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));  //스킬이 몇개?
            count += sizeof(ushort);
            for(int i=0; i < skillLen; i++)
            {
                SkillInfo skill = new SkillInfo();
                skill.Read(s, ref count);
                skills.Add(skill);
            }
        }
         
        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096); //사이즈 예약
            bool success = true;
            ushort count = 0;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            //count 

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count,s.Length - count), this.packetId); // 공간이 모자르면 실패 
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId); // 공간이 모자르면 실패 
            count += sizeof(long);

            // string 의 len 알아내기 -> byte[]로 변환해 직렬화

            //ushort nameLen =  (ushort)Encoding.Unicode.GetByteCount(this.name);  
            //success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen); //byte 배열의 정확한 크기로 buffer에 삽입
            //count += sizeof(ushort);
            //Array.Copy(Encoding.Unicode.GetBytes(this.name), 0, segment.Array,count,nameLen); // 
            //count += nameLen;

            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count+sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);
            count += nameLen;

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
            count += sizeof(ushort);
            foreach(SkillInfo skill in skills)
            {
                success &= skill.Write(s, ref count);
            }


            //최종 카운트 기입
            success &= BitConverter.TryWriteBytes(s, count); // 사이즈 적어주기

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

    class ServerSession : Session
    {



        public override void OnConnected(EndPoint endPoint)
        {

            Console.WriteLine($"On Connected :{endPoint}");
         
            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001,name="ABCD" };
            packet.skills.Add(new SkillInfo() { id = 101, level = 1, duration = 3.0f });

            packet.skills.Add(new SkillInfo() { id = 102, level = 2, duration = 3.0f });

            packet.skills.Add(new SkillInfo() { id = 103, level = 3, duration = 3.0f });

            packet.skills.Add(new SkillInfo() { id = 104, level = 4, duration = 3.0f });

            ArraySegment<byte> s=  packet.Write();

            if (s != null)
                Send(s);

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
