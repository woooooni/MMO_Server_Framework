using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            //문지기 교육
            _listenSocket.Bind(endPoint);

            //영업 시작
            //backlog : 최대 대기 수
            _listenSocket.Listen(backlog);


            for (int i = 0; i < register; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(onAcceptCompleted);
                //초기화 하는 시점에 한 번 처리.
                RegisterAccept(args);
            }
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            // *** 중요 ***  
            // 이벤트를 재사용하면, 반드시 Clean 하게 만들어 주어야 함.
            args.AcceptSocket = null;

            // AcceptAsync --> bool 값으로 pending 값을 뱉음.
            // pending == null --> 보류 중이 아니다.
            // good!
            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
                onAcceptCompleted(null, args);
        }


        //이벤트 함수!
        void onAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                //아무런 문제가 없을 경우 실행.
                //TODO
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            //다시 실행!
            RegisterAccept(args);
        }
    }
}