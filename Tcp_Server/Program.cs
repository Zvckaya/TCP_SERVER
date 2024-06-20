using System.Net;
using System.Net.Sockets;
using System.Text;
using Tcp_Server_Core;

namespace Tcp_Server
{
    class Packet
    {
        public ushort size;
        public ushort packetId;

    }

    class GameSession : PacketSession
    {
        //왜 구현하는가 ? 엔진과 컨텐츠를 분리하기 위함 
        //컨텐츠측에서는 세션이아닌 게임 세션을 만들어서 사용 
        public override void OnConnected(EndPoint endPoint)
        {

            Console.WriteLine($"OnConnected : {endPoint}");

//            Packet packet = new Packet() { size = 100, packetId = 10 };
//;
//            ArraySegment<byte> openSegment =  SendBufferHelper.Open(4096);
//            byte[] buffer = BitConverter.GetBytes(packet.size);
//            byte[] buffer2 = BitConverter.GetBytes(packet.packetId);

//            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
//            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
//            ArraySegment<byte> sendBuff =  SendBufferHelper.Close(buffer.Length + buffer2.Length);

//            Send(sendBuff);

            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"On Disconnected :{endPoint}");
        }

        //public override int OnRecv(ArraySegment<byte> buffer)
        //{
        //    string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        //    Console.WriteLine($"[From Client] {recvData}");
        //    return buffer.Count;
        //}

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"Recv Size {size} RecvId {packetId}");
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