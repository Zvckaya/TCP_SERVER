using System.Net;
using System.Net.Sockets;
using System.Text;
using Tcp_Server_Core;

namespace Tcp_Server
{
    class GameSession : Session
    {
        //왜 구현하는가 ? 엔진과 컨텐츠를 분리하기 위함 
        //컨텐츠측에서는 세션이아닌 게임 세션을 만들어서 사용 
        public override void OnConnected(EndPoint endPoint)
        {

            Console.WriteLine($"On Connected :{endPoint}");
            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to Server");
            Send(sendBuff);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"On Disconnected :{endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }

    class Program
    {
        static Listener _listener = new Listener();



        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening...");


            while (true)
            {
                ;
            }


        }
    }
}