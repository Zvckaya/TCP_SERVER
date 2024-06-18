using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tcp_Server_Core
{
    class Listener
    {
        Socket _listenSocket;

        public void Init(IPEndPoint endPoint)
        {

            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(10);

        }

        public Socket Accept()
        {
            return _listenSocket.Accept();
        }

    }
}
