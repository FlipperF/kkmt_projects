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
        public static string fname;
        public static string lname;

        public Login()
        {
            InitializeComponent();
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

        private void login_but_Click(object sender, RoutedEventArgs e)
        {
            string selectUser = "";
            string loginStat = "";
            string passStat = "";
            if (login_tb.Text != null && password_tb.Text != null)
            {
                dataBase.connect.Open();
                selectUser = "select login, password, fname, lname from users where login = '" + login_tb.Text + "' and password = '" + password_tb.Text+"'";
                dataBase.command = new MySqlCommand(selectUser, dataBase.connect);
                dataBase.reader = dataBase.command.ExecuteReader();
                while (dataBase.reader.Read())
                {
                    loginStat = dataBase.reader[0].ToString();
                    passStat = dataBase.reader[1].ToString();
                    fname = dataBase.reader[2].ToString();
                    lname = dataBase.reader[3].ToString();
                }
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

       

        private void registerWInOpen(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Registr reg = new Registr();
            reg.Show();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Environment.Exit(0);
        }
    }
}
