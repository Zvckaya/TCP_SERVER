using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tcp_Server_Core;


class PacketHandler
{
    public static void PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        PlayerInfoReq p = packet as PlayerInfoReq;

        Console.WriteLine($"Player InfoReq: {p.playerId} {p.name}");

        foreach (PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"Skill({skill.id}) ({skill.level}) ({skill.duration})");
        }
    }
}

