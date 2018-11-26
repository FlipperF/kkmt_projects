using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        public static MySqlDataAdapter data,driversData,carsData,trailersData;
        public static DataTable dt,drivers_dt,cars_dt,trailers_dt;
        public static string[] tables = {"drivers","cars","trailer"};

        public startInterface()
        {
            InitializeComponent();

            DataToTable("drivers", DriverDG);
            DataToTable("cars", carDG);
            DataToTable("trailer", trailerDG);

        }

        private void addFormBut_Click(object sender, RoutedEventArgs e)
        {
            if (driverGrid.Visibility == Visibility.Visible)
            {
                //MessageBox.Show("1");
                addDriverGrid.Visibility = Visibility.Visible;
                driverGrid.Visibility = Visibility.Hidden;
                addFormBut.Content = "Назад";
            }
            else if (addDriverGrid.Visibility == Visibility.Visible)
            {
                //MessageBox.Show("2");
                addFormBut.Content = "Добавить";
                addDriverGrid.Visibility = Visibility.Hidden;
                driverGrid.Visibility = Visibility.Visible;
            }

            if(carGrid.Visibility == Visibility.Visible)
            {
                addCarGrid.Visibility = Visibility.Visible;
                carGrid.Visibility = Visibility.Hidden;
                addFormBut.Content = "Назад";
            }
            else if (addCarGrid.Visibility == Visibility.Visible)
            {
                //MessageBox.Show("2");
                addFormBut.Content = "Добавить";
                addCarGrid.Visibility = Visibility.Hidden;
                carGrid.Visibility = Visibility.Visible;
            }
            if (trailerGrid.Visibility == Visibility.Visible)
            {
                addTrailerGrid.Visibility = Visibility.Visible;
                trailerGrid.Visibility = Visibility.Hidden;
                addFormBut.Content = "Назад";
            }
            else if (addTrailerGrid.Visibility == Visibility.Visible)
            {
                //MessageBox.Show("2");
                addFormBut.Content = "Добавить";
                addTrailerGrid.Visibility = Visibility.Hidden;
                trailerGrid.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// ВЫвод таблицы на экран приложения
        /// </summary>
        /// <param name="table">Название таблицы</param>
        /// <param name="dataGrid">Название грида</param>
        public static void DataToTable(string table, DataGrid dataGrid)
        {
            Login.connect.Close();

            if (table == "drivers")
            {
                string select = String.Format(@"SELECT f_name, l_name, m_name, category, passData, gos_numb FROM drivers inner join
                                              rights_category on rights_id = id_category inner join cars on drivers.car_id = cars.car_id");
                Login.connect.Open();
                MySqlCommand command1 = new MySqlCommand(select, Login.connect);
                dt = new DataTable();
                data = new MySqlDataAdapter(command1);
                data.Fill(dt);
                dataGrid.DataContext = dt;

                driversData = data;
                drivers_dt = dt;
            }
            else if(table == "cars")
            {
                string select = String.Format(@"SELECT model,gos_numb,category FROM cars
                                                     inner join rights_category on category_id = id_category");
                Login.connect.Open();
                MySqlCommand command1 = new MySqlCommand(select, Login.connect);
                dt = new DataTable();
                data = new MySqlDataAdapter(command1);
                data.Fill(dt);
                dataGrid.DataContext = dt;

                carsData = data;
                cars_dt = dt;

            }
            else if (table == "trailer")
            {
                string select = String.Format(@"SELECT model,carryng,gos_num from trailer");
                Login.connect.Open();
                MySqlCommand command3 = new MySqlCommand(select, Login.connect);
                dt = new DataTable();
                data = new MySqlDataAdapter(command3);
                data.Fill(dt);
                dataGrid.DataContext = dt;

                trailersData = data;
                trailers_dt = dt;

            }
            Login.connect.Close();
         
        }

        /// <summary>
        /// Закрытие программы на крестик
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Environment.Exit(0);
        }


        private void carsBut_Click(object sender, RoutedEventArgs e)
        {
            driverGrid.Visibility = Visibility.Hidden;
            trailerGrid.Visibility = Visibility.Hidden;
            carGrid.Visibility = Visibility.Visible;

            addCarGrid.Visibility = Visibility.Hidden;
            addDriverGrid.Visibility = Visibility.Hidden;
            addTrailerGrid.Visibility = Visibility.Hidden;
            addFormBut.Content = "Добавить";
        }

        private void trailerBut_Click(object sender, RoutedEventArgs e)
        {
            driverGrid.Visibility = Visibility.Hidden;
            carGrid.Visibility = Visibility.Hidden;
            trailerGrid.Visibility = Visibility.Visible;


            addCarGrid.Visibility = Visibility.Hidden;
            addDriverGrid.Visibility = Visibility.Hidden;
            addTrailerGrid.Visibility = Visibility.Hidden;
            addFormBut.Content = "Добавить";
        }

        private void driversBut_Click(object sender, RoutedEventArgs e)
        {
            carGrid.Visibility = Visibility.Hidden;
            trailerGrid.Visibility = Visibility.Hidden;
            driverGrid.Visibility = Visibility.Visible;


            addCarGrid.Visibility = Visibility.Hidden;
            addDriverGrid.Visibility = Visibility.Hidden;
            addTrailerGrid.Visibility = Visibility.Hidden;
            addFormBut.Content = "Добавить";
        }

        /// <summary>
        /// Обновление таблицы, при изменении
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpgradeBut_Click(object sender, RoutedEventArgs e)
        {
            /*MySqlCommandBuilder mcb = new MySqlCommandBuilder(driversData);
            Login.connect.Open();
            driversData.UpdateCommand = mcb.GetUpdateCommand();
            driversData.Update(drivers_dt);
            Login.connect.Close();
            MySqlCommandBuilder mcb1 = new MySqlCommandBuilder(carsData);
            Login.connect.Open();
            carsData.UpdateCommand = mcb1.GetUpdateCommand();
            carsData.Update(cars_dt);
            Login.connect.Close();
            MySqlCommandBuilder mcb2 = new MySqlCommandBuilder(trailersData);
            Login.connect.Open();
            trailersData.UpdateCommand = mcb2.GetUpdateCommand();
            trailersData.Update(trailers_dt);
            Login.connect.Close();

            DataToTable("drivers", DriverDG);
            DataToTable("cars", carDG);
            DataToTable("trailer", trailerDG);*/

        }

        /*
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rowCount > 0)
            {
                try
                {
                    MessageBox.Show(allCount.ToString());
                    MessageBox.Show(rowCount.ToString());
                    if (allCount < rowCount)
                    {
                        MessageBox.Show((DriverDG.ItemsSource as DataView)[rowCount][0].ToString(),1.ToString());
                        
                        string[] arrParmData = {(DriverDG.ItemsSource as DataView)[rowCount][1].ToString(), (DriverDG.ItemsSource as DataView)[rowCount][2].ToString()
                    ,(DriverDG.ItemsSource as DataView)[rowCount][3].ToString(),(DriverDG.ItemsSource as DataView)[rowCount][4].ToString()
                    ,(DriverDG.ItemsSource as DataView)[rowCount][5].ToString(),(DriverDG.ItemsSource as DataView)[rowCount][6].ToString()};
                        MySqlClass.executeQuery(String.Format("insert into drivers (f_name,l_name, m_name, rights_id, passData,car_id) values ('{0}', '{1}', '{2}', {3}, '{4}', {5})",
                            arrParmData[0], arrParmData[1], arrParmData[2], arrParmData[3], arrParmData[4], arrParmData[5]));
                        allCount++;
                        
                    }
                    else
                    {
                        
                        /*MySqlClass.executeQuery(@"update drivers set 
                        f_name = '" + (DriverDG.ItemsSource as DataView)[rowCount][1].ToString() + @"',
                        l_name = '" + (DriverDG.ItemsSource as DataView)[rowCount][2].ToString() + @"',
                        m_name ='" + (DriverDG.ItemsSource as DataView)[rowCount][3].ToString() + @"',
                        rights_id = '" + (DriverDG.ItemsSource as DataView)[rowCount][4].ToString() + @"',
                        passData = '" + (DriverDG.ItemsSource as DataView)[rowCount][5].ToString() + @"',
                        car_id = '" + (DriverDG.ItemsSource as DataView)[rowCount][6].ToString() + @"' where driver_id = '" + (DriverDG.ItemsSource as DataView)[rowCount][0].ToString() + "';");

                    }
                    rowCount = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Изменение таблицы");
                }
            }
        }
        

        private void DriverDG_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            rowCount = e.Row.GetIndex();
        }
        */
    }
}
