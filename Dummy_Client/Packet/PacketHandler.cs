using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tcp_Server_Core;


class PacketHandler
{
    public static void S_TestHandler(PacketSession session, IPacket packet)
    {
        S_Test p = packet as S_Test;

        Console.WriteLine($"PlayerInfoReq: {p.testInt} ");


    }
}

