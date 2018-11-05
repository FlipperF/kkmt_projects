using System.Windows;
using MySql.Data.MySqlClient;
namespace Sozdanie_transportnoy_nakladnoy
{
    /// <summary>
    /// Логика взаимодействия для Registr.xaml
    /// </summary>
    public partial class Registr : Window
    {
        public string lnameToDB = "";
        public string fnameToDB = "";
        public string mnameToDB = "";
        public string addressToDB = "";
        public string emailToDB = "";
        public string passwordToDB = "";
        public string phoneNumbToDB = "";

        public string userr = "";
        public string checkUser = ""; 

        public Registr()
        {
            InitializeComponent();
        }

        private void regBut_Click(object sender, RoutedEventArgs e)
        {
            lnameToDB = lastName_tb.Text;
            fnameToDB = firstName_tb.Text;
            mnameToDB = middleName_tb.Text;
            //addressToDB = address_tb.Text;
            emailToDB = email_tb.Text;
            passwordToDB = password_tb.Text;
            phoneNumbToDB = phone_tb.Text.ToString();

            if (lnameToDB == "" || fnameToDB == "" || emailToDB == "" || passwordToDB == "" || phoneNumbToDB == "")
            {
                MessageBox.Show("Присутствуют незаполненные поля");
                return;
            }
            else
            {
                userr = "select email from users where email = '" +emailToDB + "';";
                MySqlCommand checkCommand = new MySqlCommand(userr,MainWindow.connect);
                checkUser = checkCommand.ExecuteScalar().ToString();

                if (checkUser != "")
                {
                    MessageBox.Show("Пользователь с таким Email'ом уже существует");
                    return;
                }
                else
                {
                    string adduser = @"INSERT INTO users(f_name,l_name,m_name,email,password,phone_numb) VALUES ('" + fnameToDB + "','" + lnameToDB + "','"
                                                                                                                   + mnameToDB + "','" + emailToDB + "','"
                                                                                                                   + passwordToDB + "','" + phoneNumbToDB + "')";
                    MySqlCommand commandAdd = new MySqlCommand(adduser, MainWindow.connect);
                    commandAdd.ExecuteNonQuery();
                    MainWindow.connect.Close();

                    this.Hide();
                }
            }
        }
    }
}
