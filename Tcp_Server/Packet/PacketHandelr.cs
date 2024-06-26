using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tcp_Server_Core;


class PacketHandelr
{
    public static void PlayerInfoReqHandelr(PacketSession session, IPacket packet)
    {
        PlayerInfoReq p = packet as PlayerInfoReq;

        Console.WriteLine($"PlayerInfoReq: {p.playerId} {p.name}");

        foreach(PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"SKill {skill.id} {skill.level} {skill.duration}");
        }
    }
}


