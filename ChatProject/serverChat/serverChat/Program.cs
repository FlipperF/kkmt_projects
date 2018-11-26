using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace serverChat
{
    class Program
    {
        private const string ip= "127.0.0.1";
        private const int port = 9933;
        private static Thread serverThread;

        static void Main(string[] args)
        {
            // Создаём и запускаем фоновый поток
            serverThread = new Thread(startServer);
            serverThread.IsBackground = true;
            serverThread.Start();
            while (true)
                handlerCommands(Console.ReadLine());
        }


        /// <summary>
        /// Функция получения списка пользователей
        /// </summary>
        /// <param name="cmd"></param>
        private static void handlerCommands(string cmd)
        {
            cmd = cmd.ToLower();
            if (cmd.Contains("/getusers"))
            {
                int countUsers = Server.Clients.Count;
                for (int i = 0; i < countUsers; i++)
                {
                    Console.WriteLine("[{0}]: {1}",i,Server.Clients[i].UserName);
                }
            }
        }

        /// <summary>
        /// Функия запуска сервера
        /// </summary>
        private static void startServer()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(ip);
            IPAddress ipAddress = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(new IPEndPoint(IPAddress.Any, port));

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remote = (EndPoint)(sender);
            socket.Listen(0);
            Console.WriteLine("Сервер запущен на IP: {0}.", ipEndPoint);
            while(true)
            {
                try
                {
                    Socket user = socket.Accept();
                    Server.NewClient(user);
                }
                catch (Exception exp) { Console.WriteLine("Ошибка: {0}",exp.Message); }
            }

        }
    }
}
