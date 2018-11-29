using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        //public static string ip = "mikazuki.ddns.net";
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

            userName =Login.fname; //имя для сообщения

            logProfile.Content = "@"+Login.loginStat;//Логин
            nameBlock.Text = Login.fname;
            lnameBlock.Text = Login.lname;
            logProf.Content = "@" + Login.loginStat;
            nameBox.Text = Login.fname;
            lnameBox.Text = Login.lname;

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
                byte[] buffer = new byte[1048576];
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
            catch { MessageBox.Show("Сервер недоступен!"); }
        }

        /// <summary>
        /// Функция очистки чатборда
        /// </summary>
        private void clearChat()
        {
            // Получение доступа к элементу в другом потоке
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                mesBoard.Children.Clear();
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
            catch { MessageBox.Show(("Связь с сервером прервалась...")); }
        }

        /// <summary>
        /// Функция отображения сообщения в меседжборде
        /// </summary>
        /// <param name="msg">Сообщение</param>
        private void print(string msg)
        {
            // Получение доступа к элементу в другом потоке
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                if (mesBoard.Children.Count == 0)
                     createMesBoard(msg);
                else
                {
                    createMesBoard(msg);
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

        /// <summary>
        /// Функция клика по профилю
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void profileBut_Click(object sender, RoutedEventArgs e)
        {
            setBord.Visibility = Visibility.Visible;//Отобразить грид 
        }

        /// <summary>
        /// Клик по кнопке принять в изменении профиля
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void accept_Click(object sender, RoutedEventArgs e)
        {
            //Если при мзменении одно из полей полностью пустое, то изменение не сохраняется
            if(lnameBox.Text == "" || nameBox.Text == "")
            {
                MessageBox.Show("Присутствуют незаполненные поля. Старые данные остаются без изменений");
                nameBox.Text = nameBlock.Text;
                lnameBox.Text = lnameBlock.Text;
            }
            else
            {
                nameBlock.Text = nameBox.Text;
                lnameBlock.Text = lnameBox.Text;
                //Добавить измененные данные в базу
                string newDataUser = @"UPDATE `P2_15_Pervoi`.`users` SET `fname` = '"+nameBox.Text.ToString()+"', `lname` = '" + lnameBox.Text.ToString()+ "'" +
                    "  WHERE (`login` = '" + Login.loginStat+"');";
                //MessageBox.Show(newDataUser);

                dataBase.connect.Open();
                dataBase.command = new MySql.Data.MySqlClient.MySqlCommand(newDataUser, dataBase.connect);
                dataBase.command.ExecuteNonQuery();
                dataBase.connect.Close();

                //Изменение имени в кнопке профиля
                if ((nameBlock.Text + " " + lnameBlock.Text).Length >= 16)
                {
                    profileBut.Content = nameBlock.Text + "\n" + lnameBlock.Text;
                }
                else
                    profileBut.Content = nameBlock.Text + " " + lnameBlock.Text;
            }
            
            //Закрыть текущее окно
            setBord.Visibility = Visibility.Hidden;
            nameBlock.Visibility = Visibility.Visible;
            lnameBlock.Visibility = Visibility.Visible;
            nameBox.Visibility = Visibility.Hidden;
            lnameBox.Visibility = Visibility.Hidden;

            
        }

        /// <summary>
        /// Клик по кнопке регистрации
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void redBut_Click(object sender, RoutedEventArgs e)
        {
            nameBlock.Visibility = Visibility.Hidden;
            lnameBlock.Visibility = Visibility.Hidden;
            nameBox.Visibility = Visibility.Visible;
            lnameBox.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Клик по кнопке профиля
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void imgProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush myBrush = new ImageBrush();//Переменная изображения
            myBrush.ImageSource = new BitmapImage(new Uri("Resources/1.jpg", UriKind.Relative));
            imgProfileBtn.Background = myBrush;
            //profileBut.Icon = img;

        }

        /// <summary>
        /// Функция создания поля сообщений
        /// </summary>
        /// <param name="msg">Сообщение</param>
        private void createMesBoard(string msg)
        {
            
            string name = msg.Substring(0, msg.IndexOf(' '));//Первая часть соообщения - имя
            name = name.Trim();//Удаление мусора
            
            StackPanel messageSP = new StackPanel();//Стак панель, хранящая в себе имя и сообщение
            messageSP.Margin = new Thickness(0, 0, 0, 20);
            messageSP.VerticalAlignment = VerticalAlignment.Bottom;//Отображение сообщений внизу
            messageSP.Height = double.NaN;//растянуть по всей высоте Height = "auto"

            //Блок имени пользователя
            TextBlock UserNameTB = new TextBlock();
            UserNameTB.FontWeight = FontWeights.Bold;
            UserNameTB.FontSize = 20;
            UserNameTB.Margin = new Thickness(5, 0, 0, 0);
            UserNameTB.Text = name;
            UserNameTB.Foreground = Brushes.DeepSkyBlue;
            UserNameTB.FontFamily = new FontFamily("Comic Sans MS");

            //Блок сообщения
            TextBlock messageTB = new TextBlock();
            messageTB.Text = msg.Substring(msg.IndexOf(' '));
            messageTB.TextWrapping = TextWrapping.Wrap;
            messageTB.Margin = new Thickness(5, 0, 0, 0);
            messageTB.FontFamily = new FontFamily("Comic Sans MS");
            messageTB.FontSize = 18;

            //MessageBox.Show(lnameBox.Text);
            //MessageBox.Show(name);
            if (name == nameBox.Text+":")
            {
                UserNameTB.HorizontalAlignment = HorizontalAlignment.Left;
                messageTB.HorizontalAlignment = HorizontalAlignment.Left;
                messageSP.HorizontalAlignment = HorizontalAlignment.Left;
                messageSP.Margin = new Thickness(5, 0, 300, 0);
                UserNameTB.Margin = new Thickness(5, 0, 5, 0);
            }
            else
            {
                UserNameTB.HorizontalAlignment = HorizontalAlignment.Right;
                messageTB.HorizontalAlignment = HorizontalAlignment.Right;
                messageSP.HorizontalAlignment = HorizontalAlignment.Right;
                messageSP.Margin = new Thickness(300, 0, 5, 0);
                UserNameTB.Margin = new Thickness(5, 0, 5, 0);
            }
            //Добавление блока сообщения и имени пользователя в стак панель, а ее в основную доску сообщений
            messageSP.Children.Add(UserNameTB);
            messageSP.Children.Add(messageTB);
            mesBoard.Children.Add(messageSP);
        }
    }
}
