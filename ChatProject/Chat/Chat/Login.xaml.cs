using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        public static string fname;//Имя из бд
        public static string lname;//фамилия из бд
        public static string loginStat = "";//Статус логина при проверке


        public Login()
        {
            InitializeComponent();

            //Проверка на подключение к базе
            try
            {
                dataBase.connect.Open();

            }
            catch
            {
                MessageBox.Show("Connection False");
            }
            dataBase.connect.Close();

        }

        /// <summary>
        /// Клик по кнопке Login
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void login_but_Click(object sender, RoutedEventArgs e)
        {
            string selectUser = ""; //Строка запроса поиска юзера
            string passStat = "";//Статус пароля

            //Не сработает при незаполненном поле
            if (login_tb.Text != null && password_tb.Text != null)
            {
                dataBase.connect.Open();
                //Поиск пользователя
                selectUser = "select login, password, fname, lname from users where login = '" + login_tb.Text + "' and password = '" + password_tb.Text+"'";
                dataBase.command = new MySqlCommand(selectUser, dataBase.connect);//Выполнение запроса
                dataBase.reader = dataBase.command.ExecuteReader();//Ридер для чтения столбцов
                //Получение данных пользователя
                while (dataBase.reader.Read())
                {
                    loginStat = dataBase.reader[0].ToString(); 
                    passStat = dataBase.reader[1].ToString();
                    fname = dataBase.reader[2].ToString();
                    lname = dataBase.reader[3].ToString();
                }
                //При нахождении пользователя Откроет основную программу
                if (loginStat != "" && passStat != "")
                {

                    this.Hide();
                    Client client = new Client();
                    client.Show();
                }
                else
                {
                    MessageBox.Show("Такого пользователя не существует");
                    dataBase.connect.Close();
                    return;
                }
            }

            else
            {
                return;
            }
        }

       
        /// <summary>
        /// Клик по кнопки регистрации
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void registerWInOpen(object sender, RoutedEventArgs e)
        {
            //Скрывает окно логина и отображает окно регистрации
            this.Hide();
            Registr reg = new Registr();
            reg.Show();

        }

        /// <summary>
        /// Закрытие окна на крестик
        /// </summary>
        /// <param name="sender">Обхект</param>
        /// <param name="e">Событие</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Environment.Exit(0);
        }
    }
}
