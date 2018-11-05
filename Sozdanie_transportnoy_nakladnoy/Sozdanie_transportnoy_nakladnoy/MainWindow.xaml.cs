using System.Windows;
using MySql.Data.MySqlClient;

namespace Sozdanie_transportnoy_nakladnoy
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string tryLogin = "";
        public static string tryPassword = "";
        public static MySqlConnection connect = new MySqlConnection(@"server=den1.mysql4.gear.host;
                                                                port=3306;
                                                                user=gruzoperevozki;
                                                                password=*87654321;
                                                                database=gruzoperevozki");

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                connect.Open();
            }
            catch
            {
                MessageBox.Show("Connection False");
            }
        }

        private void registerWInOpen (object sender, RoutedEventArgs e)
        {
            Registr regWindow = new Registr();
            regWindow.Show();
        }

        private void login_but_Click(object sender, RoutedEventArgs e)
        {
            string loginNow = login_tb.Text;
            string passwordNow = password_tb.Text;

            string selectLogin = "select email, password from users where email = '" +loginNow+"' and password =  '"+passwordNow+"';";
            MySqlCommand checkCommand = new MySqlCommand(selectLogin,connect);
            MySqlDataReader readLogin = checkCommand.ExecuteReader();
            while (readLogin.Read())
            {
                tryLogin = readLogin[0].ToString();
                tryPassword = readLogin[1].ToString();
            }

            if(tryPassword != "" && tryLogin != "")
            {
                this.Hide();
                startInterface start = new startInterface();
                start.Show();

                connect.Close();
            }
            else
            {
                MessageBox.Show("Такого пользователя не существует");
                return;
            }
        }
    }
}
