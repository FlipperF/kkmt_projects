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

        private void regBut_Click(object sender, RoutedEventArgs e)
        {
            if(lnameTb.Text == "" || fnameTb.Text == "" || loginTb.Text == "" || passwordTb.Text == "")
            {
                MessageBox.Show("Присутствуют незаполненные поля");
            }
            else
            {
                string selectUser = "";
                string userStatus = "";
                dataBase.connect.Open();
                selectUser = "select login from users where login = '" + loginTb.Text.ToString()+"'";
                MySqlCommand command = new MySqlCommand(selectUser, dataBase.connect);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    userStatus = reader[0].ToString();
                   
                }
                if (userStatus != "")
                {
                    MessageBox.Show("Пользователь с таким Логином уже существует");
                    dataBase.connect.Close();
                    return;
                }
                else
                {
                    dataBase.connect.Close();
                    dataBase.connect.Open();
                    string addUser = @"INSERT INTO users(fname, lname, login, password) VALUES ('" + fnameTb.Text+ "', '" +
                                                                                                     lnameTb.Text + "', '" +
                                                                                                     loginTb.Text + "', '" +
                                                                                                     passwordTb.Text + "')";
                    MessageBox.Show(addUser);
                    dataBase.command = new MySqlCommand(addUser, dataBase.connect);
                    dataBase.command.ExecuteNonQuery();
                    dataBase.connect.Close();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Login log = new Login();
            log.Show();
        }
    }
}
