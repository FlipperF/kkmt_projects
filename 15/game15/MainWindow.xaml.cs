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
using System.Media;
namespace game15
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        int st = 1; /// Счетчик шагов
        public string[] data = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", " " };
        public int answ = 0;
        /// <summary>
        /// Старт игры
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //Музыка на фоне
            SoundPlayer sp = new SoundPlayer("Resources/8bit.wav");
            sp.Play();

            //массив пятнашек
            

            //Перемешивание массива 
            var random = new Random(DateTime.Now.Millisecond);
            data = data.OrderBy(x => random.Next()).ToArray();
            

            //Кнопки скрыты, пока не будет нажата кнопка "Начать"   
            for (int i = 0; i< data.Length;i++)
            {
                Button ButHide = (Button)this.FindName("button" + Convert.ToString(i + 1));
                ButHide.Visibility = Visibility.Hidden;
            }
            
            //Все объекты скрыты, пока не будет нажата кнопка старт
            label.Visibility = Visibility.Hidden;
            kolvo.Visibility = Visibility.Hidden;
            label1.Visibility = Visibility.Hidden;

            ButVis(data);//Пустая ячейка

            // Запись в кнопки перемешаного массивa
            for (int i = 0; i < data.Length; i++)
            {
                Button btn = (Button)this.FindName("button" + Convert.ToString(i + 1));
                btn.Content = data[i];
            }
            
        }
        
        /// <summary>
        /// Свап ячеек
        /// </summary>
        /// <param name="a">Кнопка1</param>
        /// <param name="b">Кнопка2</param>
        public void swap(Button a, Button b) 
        {
            string temp;
            temp = Convert.ToString(a.Content);
            a.Content = Convert.ToString(b.Content);
            b.Content = Convert.ToString(temp);

            //Если кнопка пустая то скрыть ее
            if (Convert.ToString(a.Content) == " ")
            {
                a.Visibility = Visibility.Hidden;
            }
            else a.Visibility = Visibility.Visible;
            if (Convert.ToString(b.Content) == " ")
            {
                b.Visibility = Visibility.Hidden;
            }
            else b.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Сдвиги ячеек для каждой из 16ти кнопок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button2.Content) == " ")
            {
                swap(button1, button2);
            }
            else if (Convert.ToString(button3.Content) == " ")
            {
                swap(button2, button3);
                swap(button1, button2);
            }
            else if (Convert.ToString(button4.Content) == " ")
            {
                swap(button3, button4);
                swap(button2, button3);
                swap(button1, button2);
            }
            else if (Convert.ToString(button5.Content) == " ")
            {
                swap(button1, button5);
            }
            else if (Convert.ToString(button9.Content) == " ")
            {
                swap(button9, button5);
                swap(button5, button1);
            }
            else if (Convert.ToString(button13.Content) == " ")
            {
                swap(button13, button9);
                swap(button9, button5);
                swap(button5, button1);
            }
            label1.Content = Convert.ToString(st++);
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button1.Content) == " ")
            {
                swap(button2, button1);
            }
            else if (Convert.ToString(button3.Content) == " ")
            {
                swap(button2, button3);
            }
            else if (Convert.ToString(button6.Content) == " ")
            {
                swap(button2, button6);
            }
            else if (Convert.ToString(button4.Content) == " ")
            {
                swap(button4, button3);
                swap(button3, button2);
            }
            else if (Convert.ToString(button10.Content) == " ")
            {
                swap(button10, button6);
                swap(button6, button2);
            }
            else if (Convert.ToString(button14.Content) == " ")
            {
                swap(button14, button10);
                swap(button10, button6);
                swap(button6, button2);
            }
            label1.Content = Convert.ToString(st++);;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button2.Content) == " ")
            {
                swap(button3, button2);
            }
            else if (Convert.ToString(button4.Content) == " ")
            {
                swap(button3, button4);
            }
            else if (Convert.ToString(button7.Content) == " ")
            {
                swap(button3, button7);
            }
            if (Convert.ToString(button1.Content) == " ")
            {
                swap(button1, button2);
                swap(button2, button3);
            }
            if (Convert.ToString(button11.Content) == " ")
            {
                swap(button11, button7);
                swap(button7, button3);
            }
            if (Convert.ToString(button15.Content) == " ")
            {
                swap(button15, button11);
                swap(button11, button7);
                swap(button7, button3);
            }
            label1.Content = Convert.ToString(st++);;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button3.Content) == " ")
            {
                swap(button4, button3);
            }
            else if (Convert.ToString(button8.Content) == " ")
            {
                swap(button4, button8);
            }
            else if (Convert.ToString(button2.Content) == " ")
            {
                swap(button2, button3);
                swap(button3, button4);
            }
            else if (Convert.ToString(button1.Content) == " ")
            {
                swap(button1, button2);
                swap(button2, button3);
                swap(button3, button4);
            }
            else if (Convert.ToString(button12.Content) == " ")
            {
                swap(button12, button8);
                swap(button8, button4);
            }
            else if (Convert.ToString(button16.Content) == " ")
            {
                swap(button16, button12);
                swap(button12, button8);
                swap(button8, button4);
            }
            label1.Content = Convert.ToString(st++);;
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button1.Content) == " ")
            {
                swap(button5, button1);
            }
            else if (Convert.ToString(button6.Content) == " ")
            {
                swap(button5, button6);
            }
            else if (Convert.ToString(button9.Content) == " ")
            {
                swap(button5, button9);
            }
            else if (Convert.ToString(button13.Content) == " ")
            {
                swap(button13, button9);
                swap(button9, button5);
            }
            else if (Convert.ToString(button7.Content) == " ")
            {
                swap(button7, button6);
                swap(button6, button5);
            }
            else if (Convert.ToString(button8.Content) == " ")
            {
                swap(button8, button7);
                swap(button7, button6);
                swap(button6, button5);
            }
            label1.Content = Convert.ToString(st++);;
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button2.Content) == " ")
            {
                swap(button6, button2);
            }
            else if (Convert.ToString(button7.Content) == " ")
            {
                swap(button6, button7);
            }
            else if (Convert.ToString(button10.Content) == " ")
            {
                swap(button6, button10);
            }
            else if(Convert.ToString(button5.Content) == " ")
            {
                swap(button6, button5);
            }
            else if (Convert.ToString(button8.Content) == " ")
            {
                swap(button8, button7);
                swap(button7, button6);
            }
            else if (Convert.ToString(button14.Content) == " ")
            {
                swap(button14, button10);
                swap(button10, button6);
            }
            label1.Content = Convert.ToString(st++);;
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button3.Content) == " ")
            {
                swap(button7, button3);
            }
            else if (Convert.ToString(button8.Content) == " ")
            {
                swap(button7, button8);
            }
            else if (Convert.ToString(button11.Content) == " ")
            {
                swap(button7, button11);
            }
            else if (Convert.ToString(button6.Content) == " ")
            {
                swap(button7, button6);
            }
            else if (Convert.ToString(button5.Content) == " ")
            {
                swap(button5, button6);
                swap(button6, button7);
            }
            else if (Convert.ToString(button15.Content) == " ")
            {
                swap(button15, button11);
                swap(button11, button7);
            }
            label1.Content = Convert.ToString(st++);;
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button4.Content) == " ")
            {
                swap(button8, button4);
            }
            else if (Convert.ToString(button7.Content) == " ")
            {
                swap(button8, button7);
            }
            else if (Convert.ToString(button12.Content) == " ")
            {
                swap(button8, button12);
            }
            else if (Convert.ToString(button16.Content) == " ")
            {
                swap(button16, button12);
                swap(button12, button8);
            }
            else if (Convert.ToString(button6.Content) == " ")
            {
                swap(button6, button7);
                swap(button7, button8);
            }
            else if (Convert.ToString(button5.Content) == " ")
            {
                swap(button5, button6);
                swap(button6, button7);
                swap(button7, button8);
            }
            label1.Content = Convert.ToString(st++);;
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button5.Content) == " ")
            {
                swap(button9, button5);
            }
            else if (Convert.ToString(button10.Content) == " ")
            {
                swap(button9, button10);
            }
            else if (Convert.ToString(button13.Content) == " ")
            {
                swap(button9, button13);
            }
            else if (Convert.ToString(button1.Content) == " ")
            {
                swap(button1, button5);
                swap(button5, button9);
            }
            else if (Convert.ToString(button11.Content) == " ")
            {
                swap(button11, button10);
                swap(button10, button9);
            }
            else if (Convert.ToString(button12.Content) == " ")
            {
                swap(button12, button11);
                swap(button11, button10);
                swap(button10, button9);
            }
            label1.Content = Convert.ToString(st++);;

        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button6.Content) == " ")
            {
                swap(button10, button6);
            }
            else if (Convert.ToString(button11.Content) == " ")
            {
                swap(button10, button11);
            }
            else if (Convert.ToString(button14.Content) == " ")
            {
                swap(button10, button14);
            }
            else if (Convert.ToString(button9.Content) == " ")
            {
                swap(button10, button9);
            }
            else if (Convert.ToString(button2.Content) == " ")
            {
                swap(button2, button6);
                swap(button6, button10);
            }
            else if (Convert.ToString(button12.Content) == " ")
            {
                swap(button12, button11);
                swap(button11, button10);
            }
            label1.Content = Convert.ToString(st++);;
        }

        private void button11_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button7.Content) == " ")
            {
                swap(button11, button7);
            }
            else if (Convert.ToString(button12.Content) == " ")
            {
                swap(button11, button12);
            }
            else if (Convert.ToString(button15.Content) == " ")
            {
                swap(button11, button15);
            }
            else if (Convert.ToString(button10.Content) == " ")
            {
                swap(button11, button10);
            }
            else if (Convert.ToString(button9.Content) == " ")
            {
                swap(button9, button10);
                swap(button10, button11);
            }
            else if (Convert.ToString(button3.Content) == " ")
            {
                swap(button3, button7);
                swap(button7, button11);
            }
            label1.Content = Convert.ToString(st++);;
        }

        private void button12_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button8.Content) == " ")
            {
                swap(button12, button8);
            }
            else if (Convert.ToString(button11.Content) == " ")
            {
                swap(button12, button11);
            }
            else if (Convert.ToString(button16.Content) == " ")
            {
                swap(button12, button16);
            }
            else if (Convert.ToString(button4.Content) == " ")
            {
                swap(button4, button8);
                swap(button8, button12);
            }
            else if (Convert.ToString(button9.Content) == " ")
            {
                swap(button9, button10);
                swap(button10, button11);
                swap(button11, button12);
            }
            else if (Convert.ToString(button10.Content) == " ")
            {
                swap(button10, button11);
                swap(button11, button12);
            }
            label1.Content = Convert.ToString(st++);;
        }

        private void button13_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button9.Content) == " ")
            {
                swap(button13, button9);
            }
            else if (Convert.ToString(button14.Content) == " ")
            {
                swap(button13, button14);
            }
            else if (Convert.ToString(button5.Content) == " ")
            {
                swap(button5, button9);
                swap(button9, button13);
            }
            else if (Convert.ToString(button1.Content) == " ")
            {
                swap(button1, button5);
                swap(button5, button9);
                swap(button9, button13);
            }
            else if (Convert.ToString(button15.Content) == " ")
            {
                swap(button15, button14);
                swap(button14, button13);
            }
            else if (Convert.ToString(button16.Content) == " ")
            {
                swap(button16, button15);
                swap(button15, button14);
                swap(button14, button13);
            }
            label1.Content = Convert.ToString(st++);;
        }

        private void button14_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button13.Content) == " ")
            {
                swap(button14, button13);
            }
            else if (Convert.ToString(button10.Content) == " ")
            {
                swap(button14, button10);
            }
            else if (Convert.ToString(button15.Content) == " ")
            {
                swap(button14, button15);
            }
            else if (Convert.ToString(button2.Content) == " ")
            {
                swap(button2, button6);
                swap(button6, button10);
                swap(button10, button14);
            }
            else if (Convert.ToString(button6.Content) == " ")
            {
                swap(button6, button10);
                swap(button10, button14);
            }
            else if (Convert.ToString(button16.Content) == " ")
            {
                swap(button16, button15);
                swap(button15, button14);
            }
            label1.Content = Convert.ToString(st++);;
        }
        private void button15_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button14.Content) == " ")
            {
                swap(button15, button14);
            }
            else if (Convert.ToString(button11.Content) == " ")
            {
                swap(button15, button11);
            }
            else if (Convert.ToString(button16.Content) == " ")
            {
                swap(button15, button16);
            }
            else if (Convert.ToString(button3.Content) == " ")
            {
                swap(button3, button7);
                swap(button7, button11);
                swap(button11, button15);
            }
            else if (Convert.ToString(button7.Content) == " ")
            {
                swap(button7, button11);
                swap(button11, button15);
            }
            else if (Convert.ToString(button13.Content) == " ")
            {
                swap(button13, button14);
                swap(button14, button15);
            }
            label1.Content = Convert.ToString(st++);;
        }

        private void button16_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(button12.Content) == " ")
            {
                swap(button16, button12);
            }
            else if (Convert.ToString(button15.Content) == " ")
            {
                swap(button16, button15);
            }
            else if (Convert.ToString(button8.Content) == " ")
            {
                swap(button8, button12);
                swap(button12, button16);
            }
            else if (Convert.ToString(button14.Content) == " ")
            {
                swap(button14, button15);
                swap(button15, button16);
            }
            else if (Convert.ToString(button13.Content) == " ")
            {
                swap(button13, button14);
                swap(button14, button15);
                swap(button15, button16);
            }
            else if (Convert.ToString(button4.Content) == " ")
            {
                swap(button4, button8);
                swap(button8, button12);
                swap(button12, button16);
            }
            label1.Content = Convert.ToString(st++);;
        }

        /// <summary>
        /// Функция скрывает пустую кнопку
        /// </summary>
        /// <param name="data">Массив контента кнопок</param>
        private void ButVis(string[] data)
        {
            int num = Array.IndexOf(data, " ") - 1;
            Button button = (Button)this.FindName("button" + Convert.ToString(num));
            button.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Поиск пустой кнопки и ее скрытие
        /// </summary>
        private void SearchZ()
        {

            for (int i = 1; i < 17; i++)
            {
                Button btn = (Button)this.FindName("button" + Convert.ToString(i));
                if (Convert.ToString(btn.Content) == " ")
                    btn.Visibility = Visibility.Hidden;
                else
                    btn.Visibility = Visibility.Visible;
            }
            

        }

        private void endGame()
        {
            for(int i = 0; i < 16; i ++)
            {
                Button btn = new Button();
                btn.Name = "button" + i + 1.ToString();
                if(btn.Content.ToString() == data[i].ToString())
                {
                    answ += 1;
                }
            }
            if(answ == 15)
            {
                MessageBox.Show("Победа");
            }
        }

        /// <summary>
        /// Клик по кнопке старт
        /// Отображает поле кнопок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Start(object sender, RoutedEventArgs e)
        {

            SearchZ();

            label.Visibility = Visibility.Visible;
            kolvo.Visibility = Visibility.Visible;
            label1.Visibility = Visibility.Visible;
            SB.Visibility = Visibility.Hidden;
            StartLabel.Visibility = Visibility.Hidden;
           
        }

      
    }

}
