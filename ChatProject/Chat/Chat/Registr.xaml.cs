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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Chat
{
    /// <summary>
    /// Логика взаимодействия для Registr.xaml
    /// </summary>
    public partial class Registr : Window
    {

        public Registr()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Функция клика по кнопке Зарегестрироваться
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void regBut_Click(object sender, RoutedEventArgs e)
        {
            //Проверка на незаполненные поля
            if(lnameTb.Text == "" || fnameTb.Text == "" || loginTb.Text == "" || passwordTb.Text == "")
            {
                MessageBox.Show("Присутствуют незаполненные поля");
            }
            else
            {
                string selectUser = ""; //Строка запроса на поиск полльзователя по логину
                string userStatus = "";//Результат поиска
                dataBase.connect.Open();
                selectUser = "select login from users where login = '" + loginTb.Text.ToString()+"'";//Запрос поиска юзера

                MySqlCommand command = new MySqlCommand(selectUser, dataBase.connect);//Выполнение запроса
                MySqlDataReader reader = command.ExecuteReader();//Ридер для чтения столбца результата
                while (reader.Read())
                {
                    userStatus = reader[0].ToString();
                   
                }

                //При удачном запросе уведомляет пользователя о наличии такого профиля
                if (userStatus != "")
                {
                    MessageBox.Show("Пользователь с таким Логином уже существует");
                    dataBase.connect.Close();
                    return;
                }
                //При неудачном запросе добавляет нового пользователя в базу и закрывает окно
                else
                {
                    dataBase.connect.Close();
                    dataBase.connect.Open();
                    string addUser = @"INSERT INTO users(fname, lname, login, password) VALUES ('" + fnameTb.Text+ "', '" +
                                                                                                     lnameTb.Text + "', '" +
                                                                                                     loginTb.Text + "', '" +
                                                                                                     passwordTb.Text + "')";

                    dataBase.command = new MySqlCommand(addUser, dataBase.connect);
                    dataBase.command.ExecuteNonQuery();//Применить команду на добавление
                    dataBase.connect.Close();

                    this.Hide();
                    Login log = new Login();
                    log.Show();
                }
            }
        }

        /// <summary>
        /// На крестик сфорачивает окно, и отображает окно логина
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Login log = new Login();
            log.Show();
        }
    }
}
