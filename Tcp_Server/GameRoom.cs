using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tcp_Server.Session;

namespace Tcp_Server
{
    class GameRoom
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();

        public void BroadCast(ClientSession session,string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = chat;
            ArraySegment<byte> segment = packet.Write();

            lock (_lock)  // 공유하는 _sessions 를 다룰 때는 lock
            {
                foreach(ClientSession s in _sessions)
                {
                    s.Send(segment);
                }
            }

        }

        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Add(session);
                session.Room = this;
            }
  

        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
            }
        }


    }
}
