using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    class Program
    {
        public static TcpListener listener = new TcpListener(IPAddress.Any, 64095);
        public static List<connectedClient> clients = new List<connectedClient>();

        static void Main(string[] args)
        {
            listener.Start();
            while (true)
            {
                var client = listener.AcceptTcpClient();

                Task.Factory.StartNew(() =>
                {
                    var sr = new StreamReader(client.GetStream());
                    while (client.Connected)
                    {
                        var line = sr.ReadLine();
                        if (line.Contains("Name: ") && !string.IsNullOrWhiteSpace(line.Replace("Name: ", "")))
                        {
                            var nick = line.Replace("Name: ", "");

                            if (clients.FirstOrDefault(s => s.Name == nick) == null)
                            {
                                clients.Add(new connectedClient(client, nick));
                                Console.WriteLine($"New connection: {nick}");
                                break;
                            }
                            else
                            {
                                var sw = new StreamWriter(client.GetStream());
                                sw.AutoFlush = true;

                                sw.WriteLine("Пользователь с таким именем уже есть в чате");
                                client.Client.Disconnect(false);

                            }
                        }
                    }
                    while (client.Connected)
                    {
                        try
                        {
                            sr = new StreamReader(client.GetStream());
                            var line = sr.ReadLine();
                            SendToAllClient(line);

                            Console.WriteLine(line);

                        }
                        catch { }
                    }
                });
            }
        }

        static async void SendToAllClient(string message)
        {
            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    try
                    {
                        var sw = new StreamWriter(clients[i].Client.GetStream());
                        sw.AutoFlush = true;

                        sw.WriteLine(message);

                    }
                    catch { }

                }
            });
        }
    }
}
