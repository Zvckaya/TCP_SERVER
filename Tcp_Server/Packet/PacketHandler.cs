using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tcp_Server;
using Tcp_Server.Session;
using Tcp_Server_Core;


class PacketHandler
{
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        C_Chat chatPacket = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        //jobqueue이용 
        clientSession.Room.Push(
            () => { clientSession.Room.BroadCast(clientSession, chatPacket.chat); });


    }
}


