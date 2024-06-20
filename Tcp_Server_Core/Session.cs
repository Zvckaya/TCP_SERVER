using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tcp_Server_Core
{
    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;



        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        object _lock = new object();


        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>(); // List로 만들어서 여러개의 버퍼를 보낼 수 있도록 함
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Start(Socket socket)
        {
            _socket = socket;
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _recvArgs.SetBuffer(new byte[1024], 0, 1024);

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            RegisterRecv();

        }

        public void Send(byte[] sendBuff)
        {
            lock (_lock)  //동시에 Send를 호출 했을때, 큐에 넣는 작업을 동기화 시킴
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0) //보내는중이 아니라면, 보내기 시작
                    RegisterSend();

            }
        }


        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)  //끊긴 상태 확인, 멀티스레드 상황에서의 충돌방지
                return;

            OnDisconnected(_socket.RemoteEndPoint);

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신

        void RegisterSend()
        {
            while (_sendQueue.Count > 0)
            {
                byte[] buff = _sendQueue.Dequeue();
                _pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length)); //List에 추가
            }
            _sendArgs.BufferList = _pendingList;

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
                        _sendArgs.BufferList = null; //보낸 데이터를 비워줌
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);

                        if (_sendQueue.Count > 0) // 보내는도중, 큐에 데이터가 들어오면 다시 Send를 호출
                            RegisterSend();

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

        void RegisterRecv()
        {
            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)
                OnRecvCompleted(null, _recvArgs);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));

                    RegisterRecv(); //다시 재등록 
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
