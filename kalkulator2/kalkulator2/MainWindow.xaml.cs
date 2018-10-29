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

namespace kalkulator2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Клик на кнопку
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие </param>
        private void answButton_Click(object sender, RoutedEventArgs e)
        {
            string stroka = dataStr.Text + " ";//Получение данных из textBox
            string chisla = "";//Строка с числами
            List<string> masData = new List<string>(); // массив данных их строки
            string deistv = ""; // переменная для действия между числами
           
            stroka.Replace('.', ',');//Изменение точек в вещественных чисел на запятые(Вот так вот в c# )

            //Разбитие строки на отдельные числа и действия
            for (int i = 0; i < stroka.Length; i++)
            {
                if (stroka[i] == '*' || stroka[i] == '/' || stroka[i] == '+' || stroka[i] == '-' || stroka[i] == ' ')
                {
                    masData.Add(Convert.ToString(chisla));//Запись в массив
                    deistv = Convert.ToString(stroka[i]);
                    masData.Add(deistv);
                    chisla = "";
                }
                else chisla += Convert.ToString(stroka[i]);
                masData.Remove(" ");

            }
            //Цикл рассчета данных в массиве
            for (int k = 0; k < masData.Count; k++)
            {
                //Так как в конце рассчетов должено остаться одно число - результат, то выводится нулевой элемент
                if (masData.Count == 1)
                {
                    label.Content = Convert.ToString(masData[0]);
                }
                //Сначала проверяется наличие умножения и деления, для их рассчета в приоритете
                if (masData.Contains("*") == true || masData.Contains("/") == true)
                {
                    if (masData[k] == "*") //ВЫполнить соотвествующее действие 
                    {
                        masData[k - 1] = Convert.ToString(Convert.ToDouble(masData[k - 1]) * Convert.ToDouble(masData[k + 1]));//предыдущий элемент равен результату
                        //Удаление двух прочих элементов
                        masData.RemoveAt(k + 1);
                        masData.RemoveAt(k);

                        k = 0;//ЗАпуск цикла заного
                        label.Content = Convert.ToString(masData[0]);

                    }
                    else if (masData[k] == "/")
                    {
                        masData[k - 1] = Convert.ToString(Convert.ToDouble(masData[k - 1]) / Convert.ToDouble(masData[k + 1]));
                        masData.RemoveAt(k + 1);
                        masData.RemoveAt(k);
                        k = 0;
                        label.Content = Convert.ToString(masData[0]);

                    }
                }
                //Рассчет сложения и вычитания
                else
                {
                    if (masData[k] == "+")
                    {
                        masData[k - 1] = Convert.ToString(Convert.ToDouble(masData[k - 1]) + Convert.ToDouble(masData[k + 1]));
                        masData.RemoveAt(k + 1);
                        masData.RemoveAt(k);
                        k = 0;
                        label.Content = Convert.ToString(masData[0]);

                    }
                    else if (masData[k] == "-")
                    {
                        masData[k - 1] = Convert.ToString(Convert.ToDouble(masData[k - 1]) - Convert.ToDouble(masData[k + 1]));
                        masData.RemoveAt(k + 1);
                        masData.RemoveAt(k);
                        k = 0;
                        label.Content = Convert.ToString(masData[0]);
                    }

                }

            }
        }
    }
}
