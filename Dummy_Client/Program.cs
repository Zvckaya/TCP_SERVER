using System.Net;
using System.Net.Sockets;
using System.Text;
using Tcp_Server_Core;

namespace Dummy_Client
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {

            Console.WriteLine($"On Connected :{endPoint}");
       
            for (int i = 0; i < 5; i++)
            {
                byte[] sendBuff = Encoding.UTF8.GetBytes($"Connect Server {i}");
                Send(sendBuff);
            }

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

    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return new GameSession(); });

            while (true)
            {
              
                try
                {

                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                }
                Thread.Sleep(100);
            }
        }
    }
}