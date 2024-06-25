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

   


    class ServerSession : Session
    {



        public override void OnConnected(EndPoint endPoint)
        {

            Console.WriteLine($"On Connected :{endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001, name = "ABCD" };
         
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 101, level = 1, duration = 3.0f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 102, level = 2, duration = 3.0f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 103, level = 3, duration = 3.0f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 104, level = 4, duration = 3.0f });

            ArraySegment<byte> s = packet.Write();

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
