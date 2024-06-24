﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketGenerator
{
    class PacketFormat
    {
        // {0} 패킷 이름
        // {1} 멤버 변수
        // {2} 멤버변수 Read
        // {3} 멤버변수 Write 

        public static string packetFomat =
@"
public {0}
{{
    {1}

    public  void Read(ArraySegment<byte> segment)
    {{

        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        {2}

    }}
         
    public  ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096); //사이즈 예약
        bool success = true;
        ushort count = 0;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count,s.Length - count), (ushort)PacketID.{0}); 
        count += sizeof(ushort);

        {3}

        success &= BitConverter.TryWriteBytes(s, count); // 사이즈 적어주기

        if (success == false)
            return null;

        //복사배열 생성이 아닌 미리 만들어진 버퍼로 관리
        ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);
        return sendBuff;
    }}
}}
";
        //{0} 변수형식
        //{1} 변수이름

        public static string memberFormat =
@"public {0} {1}";

        //{0} 변수이름
        //{1} To~ 변수형식
        //{2} 변수형식

        public static string readFormat =
@" this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
count += sizeof({2});";

        public static string readStringFormat =
@"
ushort {0}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
this.{0} =  Encoding.Unicode.GetString(s.Slice(count,{0}Len));
count += {0}Len;
";

        //{0} 변수이름
        //{1} 변수형식
        public static string writeFormat =
@"
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.{0); 
count += sizeof({1});
";
        //{0} 변수이름
        //{1} 변수형식

        public static string writeStringFormat =
@"
ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count+sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);
count += sizeof(ushort);
count += {0}Len;
";

    }
}
