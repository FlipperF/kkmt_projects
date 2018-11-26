using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace Chat
{
    /// <summary>
    /// Логика взаимодействия для Client.xaml
    /// </summary>
    public partial class Client : Window
    {
        public static TcpClient client;
        public static NetworkStream stream;
        private Socket serverSocket;
        private Thread clientThread;
        public static string myHost;
        public static string ip = "rooddie.ddns.net";
        public static int port = 100;
        public static string message;
        public static string userName;
        public Client()
        {
            InitializeComponent();

            myHost = System.Net.Dns.GetHostName();

            //ip = System.Net.Dns.GetHostByName(myHost).AddressList[0].ToString();
            chatList.Items.Add("Открыть диалог");
            profileBut.Content = Login.fname + " " + Login.lname;
            userName = Login.fname;

            connect();
            clientThread = new Thread(listner);
            clientThread.IsBackground = true;
            clientThread.Start();
        }

        private void listner()
        {
            while (serverSocket.Connected)
            {
                // Создаём буфер для сообщений
                byte[] buffer = new byte[8196];
                // Получение сообщения
                int bytesRec = serverSocket.Receive(buffer);
                // Декодируем сообщение
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                // Если в сообщении есть команда обновляем все сообщения
                if (data.Contains("#updatechat"))
                {
                    UpdateChat(data);
                    continue;
                }
            }
        }

        private void connect()
        {
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(ip);
                IPAddress ipAddress = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remote = (EndPoint)(sender);

                // Создание подключения
                serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Connect(ipEndPoint);
            }
            catch { mesBoard.Text = ("Сервер недоступен!"); }
        }

        private void clearChat()
        {
            // Получение доступа к элементу в другом потоке
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                mesBoard.Text = string.Empty;
            }));
        }

        private void UpdateChat(string data)
        {
            clearChat();
            string[] messages = data.Split('&')[1].Split('|');
            int countMessages = messages.Length;
            if (countMessages <= 0) return;
            for (int i = 0; i < countMessages; i++)
            {
                try
                {
                    if (string.IsNullOrEmpty(messages[i])) continue;
                    print(String.Format("[{0}]: {1}.", messages[i].Split('~')[0], messages[i].Split('~')[1]));
                }
                catch { continue; }
            }
        }

        private void send(string data)
        {
            try
            {
                // Создание закодированного буфера сообщения
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int bytesSent = serverSocket.Send(buffer);
            }
            catch { mesBoard.Text = ("Связь с сервером прервалась..."); }
        }

        private void print(string msg)
        {
            // Получение доступа к элементу в другом потоке
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                if (mesBoard.Text.Length == 0)
                    mesBoard.Text += msg;
                else
                {
                    mesBoard.Text += (Environment.NewLine + msg);
                    //scroll.ScrollToEnd();
                }
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Environment.Exit(0);
        }

        private void chatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string chatClick = chatList.SelectedItem.ToString();
            if(chatClick == "Открыть диалог")
            {
                string Name = userName;
                if (string.IsNullOrEmpty(Name)) return;
                // Отправляем серверу комманду с именем клиента
                send("#setname&" + Name);
                chatGrid.Visibility = Visibility.Visible;
                startGrid.Visibility = Visibility.Hidden;

            }
        }

        private void sendMessage()
        {
            try
            {
                
                string data =" " +writeMes.Text;
                if (string.IsNullOrEmpty(data)) return;
                // Отправляем серверу комманду с сообщением
                send("#newmsg&" + data);
                writeMes.Text = string.Empty;
            }
            catch { MessageBox.Show("Ошибка при отправке сообщения!"); }
        }


        private  void writeMes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sendMessage();
            }
        }
    }
}
