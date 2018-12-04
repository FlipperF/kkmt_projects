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

        public static string pathToFile = "";
        public static byte[] imageData;//Закодированный файл картинки
        public static string findImage;
        public static byte[] data;//Зашифрованные данные изоюражения
        public static BitmapImage bitmapFromDB;
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
            //Заполнение элементов в настройке профиля
            nameBlock.Text = Login.fname;
            lnameBlock.Text = Login.lname;
            nameBox.Text = Login.fname;
            lnameBox.Text = Login.lname;

            SearchImage(Login.loginStat);//Поиск картинки в базе

            //Условие при отсутствии совпадений в базе
            if (findImage == null || findImage == "")
            {
                statusImage = false;//Отрицательный результат поиска
                
                //Миниатюра профиля получает стандартное изоюражение
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("Resources/noImage.png",UriKind.Relative));
                img.UseLayoutRounding = true;
                img.SnapsToDevicePixels = true;
                img.Stretch = Stretch.Fill;
                img.Width = 32;
                img.Height = 32;
                profileBut.Icon = img;

                //Изображение профиля получает стадартное изображение
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri("Resources/noImage.png", UriKind.Relative));
                imgProfile.Fill = ib;

            }
            else//Действие при совпадении
            {
                statusImage = true;//положительный результат поиска

                //Миниатюра профиля получает изображение из базы
                Image img = new Image();
                img.Source = ImageOutDB(Login.loginStat);
                img.UseLayoutRounding = true;
                img.SnapsToDevicePixels = true;
                img.Stretch = Stretch.Fill;
                img.Width = 32;
                img.Height = 32;
                profileBut.Icon = img;

                //Изображение профиля получает изображение из базы
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
                    //Сообщение делится на 3 части: имя, сообщение, логин
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
                writeMes.Text = string.Empty;//отчистка поля
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

            //Открывается файлдиалог с фильтром по киртанкам
            OpenFileDialog openF = new OpenFileDialog();
            openF.Filter = "Text files(*.jpg, *.png)|*.jpg; *.png";
            if (openF.ShowDialog() == true)
            {
                pathToFile = openF.FileName;//Получание пути выбранного файла
            }

            if (pathToFile != "")
            {
                //Закодирование нейденного файла
                using (System.IO.FileStream fs = new System.IO.FileStream(pathToFile, System.IO.FileMode.Open))
                {
                    imageData = new byte[fs.Length];
                    fs.Read(imageData, 0, imageData.Length);
                }

                SearchImage(Login.loginStat);//Поиск изображения
                ProfileImage.ImageToDB(imageData);//Запись изображения в базу

                //Применение нового изображения к миниатюре и основному элементу
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
                imgProfile.Fill = bit;
                statusImage = true;
            }
            else
                return;

        }

        /// <summary>
        /// Функция создания поля сообщений
        /// </summary>
        /// <param name="msg">Сообщение</param>
        private void createMesBoard(string msg)
        {
            //MessageBox.Show(msg);
            string name = msg.Substring(0, msg.IndexOf(' '));//Первая часть соообщения - имя
            string loginName = msg.Substring(msg.LastIndexOf(' ') + 1);//Получение логина из сообщения

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

            //Отправленное сообщение справа
            if (loginName == Login.loginStat)
            {
                UserNameTB.HorizontalAlignment = HorizontalAlignment.Right;
                messageTB.HorizontalAlignment = HorizontalAlignment.Right;
                messageSP.HorizontalAlignment = HorizontalAlignment.Right;
                messageSP.Margin = new Thickness(250, 0, 5, 0);
                UserNameTB.Margin = new Thickness(5, 0, 5, 0);
            }
            else//Полученное слева
            {
                UserNameTB.HorizontalAlignment = HorizontalAlignment.Left;
                messageTB.HorizontalAlignment = HorizontalAlignment.Left;
                messageSP.HorizontalAlignment = HorizontalAlignment.Left;
                messageSP.Margin = new Thickness(5, 0, 250, 0);
                UserNameTB.Margin = new Thickness(5, 0, 5, 0);
            }

            MaterialDesignThemes.Wpf.Chip message = new MaterialDesignThemes.Wpf.Chip();//Элемент сообщения

            Image img = new Image();
            //Изображение в элементе сообщения

            //Сообщение текущего пользователя с картинкой профиля
            if (Login.loginStat == loginName && statusImage == true)
            {
                img.Source = ImageOutDB(loginName);
            }
            //Сообщение текущего пользователя с без картинки профиля
            else if (Login.loginStat == loginName && statusImage != true)
            {
                img.Source = new BitmapImage(new Uri("Resources/noImage.png", UriKind.Relative));
            }
            //Входящее сообщение
            else if (Login.loginStat != loginName)
            {
                SearchImage(loginName);//Поиск картинки
                //Найдена
                if (findImage != "" || findImage != null)
                {
                    img.Source = ImageOutDB(loginName);
                }
                //не найдена
                else
                {
                    img.Source = new BitmapImage(new Uri("Resources/noImage.png", UriKind.Relative));
                }
            }
            
            //настройка изображения в сообщении
            img.UseLayoutRounding = true;
            img.SnapsToDevicePixels = true;
            img.Stretch = Stretch.Fill;
            img.Width = 32;
            img.Height = 32;
            message.Icon = img;
            message.HorizontalAlignment = HorizontalAlignment.Right;
            message.Height = double.NaN;
            message.Margin = new Thickness(5,5,0,5);
            message.Content = messageTB;
            
            //Добавление блока сообщения и имени пользователя в стак панель, а ее в основную доску сообщений
            messageSP.Children.Add(message);
            //messageSP.Children.Add(messageTB);
            mesBoard.Children.Add(messageSP);
        }

        /// <summary>
        /// Выгрузка картинки из базы данных
        /// </summary>
        /// <param name="log">Передаваемый логин</param>
        /// <returns>Возвращает элемент типа BitmapImage, хранящий в себе расшифрованное изображение</returns>
        public BitmapImage ImageOutDB(string log)
        {
            try
            {
                //Писк ячейки с картинкой
                dataBase.command = new MySql.Data.MySqlClient.MySqlCommand("select image from users where login = '" + log + "'", dataBase.connect);

                dataBase.connect.Open();
                dataBase.reader = dataBase.command.ExecuteReader();


                while (dataBase.reader.Read())
                {
                    data = (byte[])dataBase.reader[0];//Получешие зашифрованных данных
                }
                dataBase.connect.Close();

                //Поток расшифровки изображения
                using (var ms = new MemoryStream(data))
                {
                    bitmapFromDB = new BitmapImage();//Возвращаемый элемент
                    ms.Position = 0;
                    bitmapFromDB.BeginInit();
                    bitmapFromDB.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bitmapFromDB.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapFromDB.UriSource = null;
                    bitmapFromDB.StreamSource = ms;
                    bitmapFromDB.EndInit();
                }
                bitmapFromDB.Freeze();

                return bitmapFromDB;
            }
            catch
            {
                dataBase.connect.Close();
                bitmapFromDB = new BitmapImage(new Uri("Resources/noImage.png", UriKind.Relative));
                return bitmapFromDB;
            }
        }

        /// <summary>
        /// Поиск изображения в базе
        /// </summary>
        /// <param name="login"></param>
        public void SearchImage(string login)
        {
            dataBase.command = new MySql.Data.MySqlClient.MySqlCommand("select image from users where login = '" + login + "'", dataBase
    .connect);
            dataBase.connect.Open();
            dataBase.reader = dataBase.command.ExecuteReader();
            while (dataBase.reader.Read())
            {
                findImage = dataBase.reader[0].ToString();
            }
            dataBase.connect.Close();
        }

        /// <summary>
        /// Функция зацикливания гифки
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement me = sender as MediaElement;
            me.Position = TimeSpan.FromMilliseconds(1);
        }

        //Клики по рекламным банерам

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

        /// <summary>
        /// Функция нажатия на кнопку выключения\включения рекламы
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
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
