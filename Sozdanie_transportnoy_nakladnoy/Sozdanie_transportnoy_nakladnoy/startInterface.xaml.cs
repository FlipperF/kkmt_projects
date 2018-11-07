using System;
using System.Collections.Generic;
using System.Data;
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

namespace Sozdanie_transportnoy_nakladnoy
{
    /// <summary>
    /// Логика взаимодействия для startInterface.xaml
    /// </summary>
    public partial class startInterface : Window
    {
        public startInterface()
        {
            InitializeComponent();
            DataToDataGrid("users", DriverDG);
        }

        public static void DataToDataGrid(string table, DataGrid dataGrid)
        {
            Login.connect.Close();

            string select = String.Format("Select * from {0}",table);
            Login.connect.Open();
            MySqlCommand command1 = new MySqlCommand(select, Login.connect);
            DataTable dt = new DataTable();
            MySqlDataAdapter data = new MySqlDataAdapter(command1);
            data.Fill(dt);
            dataGrid.DataContext = dt;

            Login.connect.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Environment.Exit(0);
        }

    }
}
