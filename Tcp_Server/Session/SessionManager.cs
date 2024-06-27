using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tcp_Server.Session;

namespace Tcp_Server
{
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager instance { get { return _session; } }

        int _sessionId = 0;
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        object _sessionLock = new object();

        public ClientSession Generate()
        {
            lock (_sessionLock)
            {
                int sessionId = _sessionId++;

                ClientSession session = new ClientSession();
                session.SessionId = _sessionId;
                _sessionId++;
                _sessions.Add(sessionId, session);

                Console.WriteLine($"Connected: {sessionId}");

                return session;
            }

        }

        public ClientSession Find(int id)
        {
            ClientSession session = null;
            _sessions.TryGetValue(id, out session);
            return session;
        }

        public void Remove(ClientSession session)
        {
            lock (_sessionLock)
            {
                _sessions.Remove(session.SessionId);
            }
        }


    }
}
