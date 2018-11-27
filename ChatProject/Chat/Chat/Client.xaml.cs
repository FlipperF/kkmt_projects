using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Chat
{
    /// <summary>
    /// Логика взаимодействия для Client.xaml
    /// </summary>
    public partial class Client : Window
    {
        private Socket serverSocket; // Сокет для подключения к удаленному узлу
        private Thread clientThread; // поток получения сообщений
        public static string myHost; //Имя узла
        //public static string ip = "rooddie.ddns.net";
        public static string ip = "127.0.0.1";//Сервер
        public static int port = 100; //Порт сервера
        public static string userName; //Имя,отображаемое в сообщении

        /// <summary>
        /// Старт программы
        /// </summary>
        public Client()
        {
            InitializeComponent();

            myHost = System.Net.Dns.GetHostName();//Получение имени узла

            //ip = System.Net.Dns.GetHostByName(myHost).AddressList[0].ToString();
            chatList.Items.Add("Открыть диалог");
            
            //Вид имени Фамилии профиля в зависимости от его длины
            if((Login.fname + " " + Login.lname).Length >= 16)
            {
                profileBut.Content = Login.fname + "\n" + Login.lname;
            }
            else
                profileBut.Content = Login.fname + " " + Login.lname;

            userName = Login.fname; //имя для сообщения

            logProfile.Content = "@"+Login.loginStat;//Логин

            connect();// Подключение к серверу
            clientThread = new Thread(listner);//Поток прослушки
            clientThread.IsBackground = true;//Работает в фоне
            clientThread.Start();//Запуск потока
        }

        /// <summary>
        /// Получение сообщений
        /// </summary>
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

        /// <summary>
        /// Функция подключения к серверу
        /// </summary>
        private void connect()
        {
            try
            {
                //Процесс получения доступу к серверу 
                IPHostEntry ipHost = Dns.GetHostEntry(ip);
                IPAddress ipAddress = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

                //определение сетевого адреса
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remote = (EndPoint)(sender); 

                // Создание подключения
                serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Connect(ipEndPoint);
            }
            catch { mesBoard.Text = ("Сервер недоступен!"); }
        }

        /// <summary>
        /// Функция очистки чатборда
        /// </summary>
        private void clearChat()
        {
            // Получение доступа к элементу в другом потоке
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                mesBoard.Text = string.Empty;
            }));
        }

        /// <summary>
        /// Функция обновления чата (получение всех сообщений из буфера сервера
        /// </summary>
        /// <param name="data">Сообщение</param>
        private void UpdateChat(string data)
        {
            clearChat();//очищает чат
            string[] messages = data.Split('&')[1].Split('|');
            int countMessages = messages.Length;//кол-во сообщений
            if (countMessages <= 0) return;
            for (int i = 0; i < countMessages; i++)
            {
                try
                {
                    if (string.IsNullOrEmpty(messages[i])) continue;
                    print(String.Format("{0}: {1}", messages[i].Split('~')[0], messages[i].Split('~')[1]));//отображение сообщения
                }
                catch { continue; }
            }
        }

        /// <summary>
        /// Функция отправки сообщений
        /// </summary>
        /// <param name="data">Сообщение</param>
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

        /// <summary>
        /// Функция отображения сообщения в меседжборде
        /// </summary>
        /// <param name="msg">Сообщение</param>
        private void print(string msg)
        {
            // Получение доступа к элементу в другом потоке
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                if (mesBoard.Text.Length == 0)
                    mesBoard.Text += msg;
                else
                {
                    mesBoard.Text += (Environment.NewLine + msg);
                    scroll.ScrollToEnd();//опустить скрол вниз
                }
            }));
        }

        /// <summary>
        /// Закрывает окно на крестик
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Environment.Exit(0);
        }

        /// <summary>
        /// Отображение окна чата по клику на listbox
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void chatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string chatClick = chatList.SelectedItem.ToString();
            if(chatClick == "Открыть диалог")
            {
                //string Name = userName; //Присваивает имя аккаунта
                if (string.IsNullOrEmpty(userName)) return;
                // Отправляем серверу комманду с именем клиента
                send("#setname&" + userName);
                chatGrid.Visibility = Visibility.Visible;
                startGrid.Visibility = Visibility.Hidden;

            }
        }

        /// <summary>
        /// Функция отправки сообщения
        /// </summary>
        private void sendMessage()
        {
            try
            {
                string data = writeMes.Text;//Получение текста из поля ввода
                if (string.IsNullOrEmpty(data)) return;
                // Отправляем серверу комманду с сообщением
                send("#newmsg&" + data);
                writeMes.Text = string.Empty;
            }
            catch { MessageBox.Show("Ошибка при отправке сообщения!"); return; }
        }

        /// <summary>
        /// Функция отправки сообщения при клике на Enter
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private  void writeMes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sendMessage();
            }
        }

        /// <summary>
        /// Функция отправки сообщения по кнопке на жкране
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void sendMesBtn_Click(object sender, RoutedEventArgs e)
        {
            sendMessage();
        }
    }
}
