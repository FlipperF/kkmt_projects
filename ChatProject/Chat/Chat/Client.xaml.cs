using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
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

        public static string pathToFile;
        public static byte[] imageData;
        public static string findImage;
        public static byte[] data;
        public static BitmapImage mitmapFromDB;
        public static bool statusImage;

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
                profileBut.Content = Login.fname + "\n" + Login.lname + "\n" + "@" + Login.loginStat;
            }
            else
                profileBut.Content = Login.fname + " " + Login.lname + "\n" + "@" + Login.loginStat;

            userName =Login.fname; //имя для сообщения

            logProf.Content = "@"+Login.loginStat;//Логин
            nameBlock.Text = Login.fname;
            lnameBlock.Text = Login.lname;
            nameBox.Text = Login.fname;
            lnameBox.Text = Login.lname;

            SearchImage();
            if (findImage == null || findImage == "")
            {
                statusImage = false;

                Image img = new Image();
                img.Source = new BitmapImage(new Uri("Resources/noImage.png",UriKind.Relative));
                img.UseLayoutRounding = true;
                img.SnapsToDevicePixels = true;
                img.Stretch = Stretch.Fill;
                img.Width = 32;
                img.Height = 32;
                profileBut.Icon = img;

                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri("Resources/noImage.png", UriKind.Relative));
                imgProfile.Fill = ib;

            }
            else
            {
                Image img = new Image();
                img.Source = ImageOutDB(Login.loginStat);
                img.UseLayoutRounding = true;
                img.SnapsToDevicePixels = true;
                img.Stretch = Stretch.Fill;
                img.Width = 32;
                img.Height = 32;
                profileBut.Icon = img;
                statusImage = true;

                ImageBrush ib = new ImageBrush();
                ib.ImageSource = ImageOutDB(Login.loginStat);
                imgProfile.Fill = ib;
            }

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
                    print(String.Format("{0}: {1} {2}", messages[i].Split('~')[0], messages[i].Split('~')[1], messages[i].Split('~')[2]));//отображение сообщения
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
                string data = writeMes.Text + "~"+ Login.loginStat;//Получение текста из поля ввода
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
                    profileBut.Content = nameBlock.Text + "\n" + lnameBlock.Text + "\n" + "@" + Login.loginStat;
                }
                else
                    profileBut.Content = nameBlock.Text + " " + lnameBlock.Text + "\n" + "@" + Login.loginStat;
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
        public void imgProfileBtn_Click(object sender, RoutedEventArgs e)
        {


            OpenFileDialog openF = new OpenFileDialog();
            openF.Filter = "Text files(*.jpg, *.png)|*.jpg; *.png";
            if (openF.ShowDialog() == true)
            {
                pathToFile = openF.FileName;
            }


            using (System.IO.FileStream fs = new System.IO.FileStream(pathToFile, System.IO.FileMode.Open))
            {
                imageData = new byte[fs.Length];
                fs.Read(imageData, 0, imageData.Length);
            }


            SearchImage();
            ProfileImage.ImageToDB(imageData);

            

            Image img = new Image();
            img.Source = ImageOutDB(Login.loginStat);
            img.UseLayoutRounding = true;
            img.SnapsToDevicePixels = true;
            img.Stretch = Stretch.Fill;
            img.Width = 32;
            img.Height = 32;
            profileBut.Icon = img;

            ImageBrush bit = new ImageBrush();
            bit.ImageSource = ImageOutDB(Login.loginStat);
            imgProfile.Fill = bit ;
            this.Show();

        }

        /// <summary>
        /// Функция создания поля сообщений
        /// </summary>
        /// <param name="msg">Сообщение</param>
        private void createMesBoard(string msg)
        {
            //MessageBox.Show(msg);
            string name = msg.Substring(0, msg.IndexOf(' '));//Первая часть соообщения - имя
            string loginName = msg.Substring(msg.LastIndexOf(' ') + 1);

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
            messageTB.Text = msg.Substring(name.Length, msg.Length - loginName.Length - name.Length);
            messageTB.TextWrapping = TextWrapping.Wrap;
            messageTB.Margin = new Thickness(5, 0, 0, 0);
            messageTB.FontFamily = new FontFamily("Comic Sans MS");
            messageTB.FontSize = 18;

            //MessageBox.Show(lnameBox.Text);
            //MessageBox.Show(name);
            if (loginName == Login.loginStat)
            {
                UserNameTB.HorizontalAlignment = HorizontalAlignment.Right;
                messageTB.HorizontalAlignment = HorizontalAlignment.Right;
                messageSP.HorizontalAlignment = HorizontalAlignment.Right;
                messageSP.Margin = new Thickness(250, 0, 5, 0);
                UserNameTB.Margin = new Thickness(5, 0, 5, 0);
            }
            else
            {
                UserNameTB.HorizontalAlignment = HorizontalAlignment.Left;
                messageTB.HorizontalAlignment = HorizontalAlignment.Left;
                messageSP.HorizontalAlignment = HorizontalAlignment.Left;
                messageSP.Margin = new Thickness(5, 0, 250, 0);
                UserNameTB.Margin = new Thickness(5, 0, 5, 0);
            }
            MaterialDesignThemes.Wpf.Chip btn = new MaterialDesignThemes.Wpf.Chip();

            Image img = new Image();
            if(statusImage == false)
                img.Source = new BitmapImage(new Uri("Resources/noImage.png", UriKind.Relative));
            else
            {
                img.Source = ImageOutDB(loginName);
            }
            
            img.UseLayoutRounding = true;
            img.SnapsToDevicePixels = true;
            img.Stretch = Stretch.Fill;
            img.Width = 32;
            img.Height = 32;
            btn.Icon = img;
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.Height = double.NaN;
            btn.Margin = new Thickness(5,5,0,5);
            btn.Content = messageTB;
            
            //Добавление блока сообщения и имени пользователя в стак панель, а ее в основную доску сообщений
            messageSP.Children.Add(btn);
            //messageSP.Children.Add(messageTB);
            mesBoard.Children.Add(messageSP);
        }

        public BitmapImage ImageOutDB(string log)
        {
            dataBase.command = new MySql.Data.MySqlClient.MySqlCommand("select image from users where login = '" + log + "'", dataBase.connect);

            dataBase.connect.Open();
            dataBase.reader = dataBase.command.ExecuteReader();


            while (dataBase.reader.Read())
            {
                data = (byte[])dataBase.reader[0];
            }
            dataBase.connect.Close();
            using (var ms = new MemoryStream(data))
            {
                mitmapFromDB = new BitmapImage();
                ms.Position = 0;
                mitmapFromDB.BeginInit();
                mitmapFromDB.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                mitmapFromDB.CacheOption = BitmapCacheOption.OnLoad;
                mitmapFromDB.UriSource = null;
                mitmapFromDB.StreamSource = ms;
                mitmapFromDB.EndInit();
            }
            mitmapFromDB.Freeze();

            return mitmapFromDB;
        }

        public void SearchImage()
        {
            dataBase.command = new MySql.Data.MySqlClient.MySqlCommand("select image from users where login = '" + Login.loginStat + "'", dataBase
    .connect);
            dataBase.connect.Open();
            dataBase.reader = dataBase.command.ExecuteReader();
            while (dataBase.reader.Read())
            {
                findImage = dataBase.reader[0].ToString();
            }
            dataBase.connect.Close();
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement me = sender as MediaElement;
            me.Position = TimeSpan.FromMilliseconds(1);
        }

        private void binomoBut_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://binomo.com/ru");
        }

        private void vaBankBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://vabank.casino/");
        }

        private void gaminatorBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://online.geiminators.com/");
        }


        private void advertstringCheck_Click(object sender, RoutedEventArgs e)
        {
            if (advertstringCheck.IsChecked == true)
            {
                advertstring.Visibility = Visibility.Visible;
            }
            else
                advertstring.Visibility = Visibility.Hidden;

        }
    }
}
