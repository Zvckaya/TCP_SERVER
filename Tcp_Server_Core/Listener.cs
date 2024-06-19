﻿using System;
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
        Action<Socket> _onAcceptHandler;

        public void Init(IPEndPoint endPoint,Action<Socket> onAcceptHandler)
        {

            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler;

            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(10);  //10개의 리스너 생성제한으로 pending false만 나올수가 없음.
             
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);   
            RegisterAccept(args);   
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);
            if (pending==false)
            {
                OnAcceptCompleted(null, args);
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
          
                if (args.SocketError == SocketError.Success)  // AcceptAsync를 사용하여 자동으로 별도의 스레
                {
                    _onAcceptHandler.Invoke(args.AcceptSocket);
                }
                else
                    Console.WriteLine(args.SocketError.ToString());

                RegisterAccept(args); 
           
        }

        public Socket Accept()
        {
             return _listenSocket.Accept();
        }

    }
}
