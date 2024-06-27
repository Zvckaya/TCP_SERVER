using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tcp_Server.Session;
using Tcp_Server_Core;

namespace Tcp_Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();
        JobQueue _jobQueue = new JobQueue();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }


        public void BroadCast(ClientSession session,string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = chat + $" I am {packet.playerId}";
            ArraySegment<byte> segment = packet.Write();

            
                foreach(ClientSession s in _sessions)
                {
                    s.Send(segment);
                }
            

        }

        public void Enter(ClientSession session)
        {
         
                _sessions.Add(session);
                session.Room = this;
            

        }

        public void Leave(ClientSession session)
        {
          
                _sessions.Remove(session);
            
        }

       
    }
}
