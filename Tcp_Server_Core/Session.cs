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

        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        bool _pending = false;
        object _lock = new object();

        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();

        public void Start(Socket socket)
        {
            _socket = socket;
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            // recvArgs.AcceptSocket(socket); 리스너 전용 
            recvArgs.SetBuffer(new byte[1024], 0, 1024);


            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);


            RegisterRecv(recvArgs);

        }

        public void Send(byte[] sendBuff)
        {
            lock(_lock)  //동시에 Send를 호출 했을때, 큐에 넣는 작업을 동기화 시킴
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pending == false) // 이부분에서 보내는 도중에 들어올 수 있음 
                    RegisterSend(); //Recv와 다르게, Send는 무엇을 보낼지를 정해놓고 그 시점에 호출함. 미리 예약을 걸지 않음 
            }
        }
    

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)  //끊긴 상태 확인, 멀티스레드 상황에서의 충돌방지
                return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신

        void RegisterSend()
        { 

            //RegisterSend는 Send를 통해 Lock을 걸어놓아, 따로 lock처리가 필요없음.
            _pending = true;   //예약 
            byte[] buff = _sendQueue.Dequeue();
            _sendArgs.SetBuffer(buff, 0, buff.Length);

            bool pending = _socket.SendAsync(_sendArgs); //재사용 가능한 현태 
            if (pending == false)
                OnSendCompleted(null, _sendArgs); 
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            //Callback방식으로 다른 스레드에서 호출 될 수 있기 때문에, lock을 걸어줌
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        
                        if(_sendQueue.Count > 0) // 보내는도중, 큐에 데이터가 들어오면 다시 Send를 호출
                            RegisterSend();
                        else
                            _pending = false; // queue에 있는것을 보내고 나서 다시 pending을 false로 만들어줌}
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e.Message}");
                    }
                }
                else
                {
                    Disconnect();
                }

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
