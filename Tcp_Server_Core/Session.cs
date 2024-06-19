using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tcp_Server_Core
{
    class Session
    {
        Socket _socket;
        int _disconnected = 0;

        public void Start(Socket socket)
        {
            _socket = socket;
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);

            // recvArgs.AcceptSocket(socket); 리스너 전용 
            recvArgs.SetBuffer(new byte[1024], 0, 1024);

            RegisterRecv(recvArgs);

        }

        public void Send(byte[] sendBuff)
        {
            SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();  
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            sendArgs.SetBuffer(sendBuff, 0, sendBuff.Length);

            RegisterSend(sendArgs); //Recv와 다르게, Send는 무엇을 보낼지를 정해놓고 그 시점에 호출함. 미리 예약을 걸지 않음 
        }
    

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)  //끊긴 상태 확인, 멀티스레드 상황에서의 충돌방지
                return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신

        void RegisterSend(SocketAsyncEventArgs args)
        {
            bool pending = _socket.SendAsync(args);
            if (pending == false)
                OnSendCompleted(null, args);
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnSendCompleted Failed {e.Message}");
                }
            }
            else
            {

            }
             
                   
        }

        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);
            if (pending == false)
                OnRecvCompleted(null, args);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {recvData}");
                    RegisterRecv(args); //다시 재등록 
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e.Message}");
                }

            }
            else
            {

            }
        }

        #endregion

    }
}
