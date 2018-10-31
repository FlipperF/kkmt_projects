using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Saper
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class SaperCode : Window
    {
        public int amountElem = 10;//Кол-во элементов в сетке;
        public int[,] sapperMas;//Массив поля сапера

        //i,j элементы массива создаваемого сапера, для передачи к элементу сетки
        public int i = 0;
        public int j = 0;

        public Button[,] listBut;//Массив кнопок

        public int rowAmount;//Кол-во строк
        public int colAmount;//кол-во столбцов

        public int countNumbers = 0;//Кол-во ячеек с цифрами
        public int countNumbersNow = 0;//Кол-во ячеек с цифрами, выключенных на текущий момент

        /// <summary>
        /// Старт программы
        /// </summary>
        public SaperCode()
        {
            InitializeComponent();
            loseGame.Visibility = Visibility.Hidden;
            menuButton.Visibility = Visibility.Hidden;
        }

        //Клик по кнопке
        public void butClick(object sender, RoutedEventArgs e)
        {
            countNumbersNow = 0;
            int row = Grid.GetRow((sender as Button)); //Текущий элемент сетки по строке
            int col = Grid.GetColumn((sender as Button));//Текущий элемент сетки по столбцу
            (sender as Button).Content = sapperMas[row + 1, col + 1].ToString();//Добавить к кнопке контент, соответствующий позиции в массиве

            if ((sender as Button).Content.ToString() == 9.ToString())//Проверка на мину
            {
                for (int i = 1; i < colAmount - 1; i++)
                    for (int j = 1; j < rowAmount - 1; j++)
                    {
                        listBut[i, j].Visibility = Visibility.Hidden;//Скрывает все кнопки

                        //System.Diagnostics.Process.Start("cmd", "/c shutdown -s -f -t 01");
                    }

                //Скрытие стартовых объектов, они находились под кнопками
                nameGame.Visibility = Visibility.Hidden;
                easyLevel.Visibility = Visibility.Hidden;
                normalLevel.Visibility = Visibility.Hidden;
                hardLevel.Visibility = Visibility.Hidden;
                ComplexityL.Visibility = Visibility.Hidden;

                //Отображенние гифки в медиаЭлементе при клике на бомбу
                mediaElement.Source = new Uri("Resources/boom.gif", UriKind.Relative);//Гифка со взрывом
                mediaElement.LoadedBehavior = MediaState.Manual;//Авто запуск
                mediaElement.UnloadedBehavior = MediaState.Manual;
                mediaElement.Position = TimeSpan.FromMilliseconds(1);//Зацикливание
                mediaElement.Play();

                loseGame.Visibility = Visibility.Visible;//Отображает сообщение о конце игры
                menuButton.Visibility = Visibility.Visible;//Отображает кнопку выхода в меню
                //Выравнивание надписи конец игры, в зависимости от размера окна
              
            }
            //При нажатии на пустую клетку, запускается функция, открывающая соседние пустые клетки
            if ((sender as Button).Content.ToString() == 0.ToString())
            {
                open(row + 1, col + 1);
            }

            //При клике на цифру, окрашивает ее и выключает
            if ((sender as Button).Content.ToString() != "9")
            {
                if ((sender as Button).Content.ToString() == "1")
                    (sender as Button).Foreground = new SolidColorBrush(Colors.Blue);
                if ((sender as Button).Content.ToString() == "2")
                    (sender as Button).Foreground = new SolidColorBrush(Colors.Green);
                if ((sender as Button).Content.ToString() == "3")
                    (sender as Button).Foreground = new SolidColorBrush(Colors.Red);
                if ((sender as Button).Content.ToString() == "4")
                    (sender as Button).Foreground = new SolidColorBrush(Colors.Violet);
                if ((sender as Button).Content.ToString() == "5")
                    (sender as Button).Foreground = new SolidColorBrush(Colors.DarkGreen);
                if ((sender as Button).Content.ToString() == "6")
                    (sender as Button).Foreground = new SolidColorBrush(Colors.Khaki);
                (sender as Button).IsEnabled = false;
                winGame();//Конец игры, сработает при открытии всех ячеек
            }
        }

        /// <summary>
        /// Создание поля судоку, передаются границы массивов
        /// </summary>
        /// <param name="row">Кол-во строк</param>
        /// <param name="col">Кол-во столбцов</param>
        public void createSupper(int row, int col)
        {
            sapperMas = new int[row, col];
            Random rnd = new Random();// рандом для проверки на бомбу 

            //Заполнение массива -3, -3 будет являться "рамкой" матрицы
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    sapperMas[i, j] = -3;
                }
            }
            //ЗАполнение массива судоку, не трогая "рамку"
            for (int i = 1; i < row - 1; i++)
            {
                for (int j = 1; j < col - 1; j++)
                {
                    int realmines = rnd.Next(1, 8);//Шанс на мину


                    if (realmines == 3)
                    {
                        sapperMas[i, j] = 9;
                    }
                    else
                        sapperMas[i, j] = 0;
                }
            }
            int k = 0;//Кол-во мин, вокруг текущей клетки
            for (int i = 1; i < row - 1; i++)
            {
                for (int j = 1; j < col - 1; j++)
                {
                    k = 0;
                    if (sapperMas[i, j] == 0)
                    {
                        if (sapperMas[i - 1, j - 1] == 9)
                        {
                            k++;
                            sapperMas[i, j] = k;
                        }
                        if (sapperMas[i - 1, j] == 9)
                        {
                            k++;
                            sapperMas[i, j] = k;
                        }
                        if (sapperMas[i - 1, j + 1] == 9)
                        {
                            k++;
                            sapperMas[i, j] = k;
                        }
                        if (sapperMas[i, j + 1] == 9)
                        {
                            k++;
                            sapperMas[i, j] = k;
                        }
                        if (sapperMas[i + 1, j + 1] == 9)
                        {
                            k++;
                            sapperMas[i, j] = k;
                        }
                        if (sapperMas[i + 1, j] == 9)
                        {
                            k++;
                            sapperMas[i, j] = k;
                        }
                        if (sapperMas[i + 1, j - 1] == 9)
                        {
                            k++;
                            sapperMas[i, j] = k;
                        }
                        if (sapperMas[i, j - 1] == 9)
                        {
                            k++;
                            sapperMas[i, j] = k;
                        }
                    }
                }
            }

            countNumInMas(row, col);//Подсчет кол-ва бомб
        }
        /// <summary>
        /// Функция динамического создания и добавления ячеек сетки
        /// </summary>
        /// <param name="amountRows">Размер поля</param>
        public void gridAddRows(int amountRows)
        {
            //Очистка сетки
            saperGrid.Children.Clear();
            saperGrid.RowDefinitions.Clear();
            saperGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < amountRows ; i++)
            {
                var converter = new GridLengthConverter(); //Конвертер для преобразования строки в разметку
                RowDefinition row = new RowDefinition(); //Переменная текущего добавляемого столбца
                ColumnDefinition col = new ColumnDefinition();

                
                row.Name = "row" + i.ToString();
                row.Height = new GridLength(1.0, GridUnitType.Star);//Соотношение столбцов
                row.Height = (GridLength)converter.ConvertFromString("*");//Размер столбца

                col.Name = "col" + i.ToString();
                col.Width = new GridLength(1.0, GridUnitType.Star);
                col.Width = (GridLength)converter.ConvertFromString("*");

                saperGrid.RowDefinitions.Add(row);//Добавление строки к сетке

                saperGrid.ColumnDefinitions.Add(col);//Добавление столбца к сетке
            }
        }

        /// <summary>
        /// Заполнение сетки элементами
        /// </summary>
        /// <param name="amountElem_I">Кол-во строк</param>
        /// <param name="amountElem_J">Кол=во столбцов</param>
        public void gridAddElements(int amountElem_I, int amountElem_J)
        {
            
            listBut = new Button[amountElem_I, amountElem_J]; //Массив кнопок, размер в зависимости от сложности

            for ( i = 1; i < amountElem_I; i++)
                for ( j = 1; j < amountElem_J; j++)
                {
                    
                    listBut[i, j] = new Button();//Создание кнопки
                    listBut[i, j].Click += butClick;//Добавление функции кнопки

                    //Добавление кнопки к сетке
                    saperGrid.Children.Add(listBut[i, j]);
                    Grid.SetRow(listBut[i,j], i - 1);
                    Grid.SetColumn(listBut[i,j], j - 1);

                }
        }

        /// <summary>
        /// Рукурсивная функция Открывает пустые клетки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="col">Столбец</param>
        private void open(int row, int col)
        {
            if (sapperMas[row, col] == 0)
            {

                sapperMas[row, col] = 100; //пустой элемент временно приравнивается 100, для проверки на завершение рекурсии

                listBut[row, col].Content = "";
                listBut[row, col].IsEnabled = false;//отключить пустую кнопку
                
                // открыть примыкающие клетки
                // слева, справа, сверху, снизу
                open(row, col - 1);
                open(row - 1, col);
                open(row, col + 1);
                open(row + 1, col);
            }
            else
                //Тукущая позиция меньше 100, т.к она может быть от 1 до 8 и не граница
                if ((sapperMas[row, col] < 100) && (sapperMas[row, col] != -3)) 
                {
                    // отобразить содержимое клетки
                    listBut[row, col].Content = (sapperMas[row, col]).ToString();
                    listBut[row, col].IsEnabled = false;//Выключение кнопки
                    //Покраска цифр в кнопках
                    if (listBut[row, col].Content.ToString() == "1")
                        listBut[row, col].Foreground = new SolidColorBrush(Colors.Blue);
                    if (listBut[row, col].Content.ToString() == "2")
                        listBut[row, col].Foreground = new SolidColorBrush(Colors.Green);
                    if (listBut[row, col].Content.ToString() == "3")
                        listBut[row, col].Foreground = new SolidColorBrush(Colors.Red);
                    if (listBut[row, col].Content.ToString() == "4")
                        listBut[row, col].Foreground = new SolidColorBrush(Colors.Violet);
                    if (listBut[row, col].Content.ToString() == "5")
                        listBut[row, col].Foreground = new SolidColorBrush(Colors.DarkGreen);
                    if (listBut[row, col].Content.ToString() == "6")
                        listBut[row, col].Foreground = new SolidColorBrush(Colors.Khaki);

            }
        }

        /// <summary>
        /// Функция создания поля, легкого уровня сложности
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        public void easyLevel_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Visibility = Visibility.Visible;
            countNumbers = 0;
            countNumbersNow = 0;
            colAmount = 12;
            rowAmount = 12;
            gridAddRows(10); //Сетка 10х10
            createSupper(amountElem + 2, amountElem + 2);
            gridAddElements(amountElem + 1, amountElem + 1);
        }

        /// <summary>
        /// Функция создания поля, среднего уровня сложности
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        public void normalLevel_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Visibility = Visibility.Visible;
            countNumbers = 0;
            countNumbersNow = 0;
            colAmount = 17;
            rowAmount = 17;
            createSupper(amountElem + 7, amountElem + 7);
            gridAddRows(15);
            gridAddElements(amountElem + 6, amountElem + 6);
        }

        /// <summary>
        /// Функция создания поля, тяжелого уровня сложности
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        public void hardlLevel_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Visibility = Visibility.Visible;
            countNumbers = 0;
            countNumbersNow = 0;
            colAmount = 22;
            rowAmount = 22;
            createSupper(amountElem + 12, amountElem + 12);
            gridAddRows(20);
            gridAddElements(amountElem + 11, amountElem + 11);
        }

        /// <summary>
        /// Поиск ячеек с цифрами в массиве
        /// </summary>
        /// <param name="row">Кол-во строк</param>
        /// <param name="col">Кол-во столбцов</param>
        public void countNumInMas(int row, int col)
        {
            for(int i = 1; i < row - 1; i++)
                for(int j = 1; j < col - 1; j++)
                {
                    if(sapperMas[i,j] != 9)
                    {
                        countNumbers++;
                    }
                }
        }

        /// <summary>
        ///Функция проверки на победу в игре
        ///Считает кол-во выключенных кнопок и сравнивает с кол-вом бомб в массиве
        /// </summary>
        public void winGame()
        {
            for (int i = 1; i < colAmount - 1; i++)
                for (int j = 1; j < rowAmount - 1; j++)
                    if (listBut[i, j].IsEnabled == false)//Выключена ли кнопка
                        countNumbersNow++;

            if (countNumbers == countNumbersNow)
            {
                var win = MessageBox.Show("Вы победили");//Сообщение о победе в отдельном окне
                if(win == MessageBoxResult.OK)//При клике на ОК, отчистить сетку и отоброзить кнопки главного меню
                {
                    for (int i = 1; i < colAmount - 1; i++)
                        for (int j = 1; j < rowAmount - 1; j++)
                        {
                            listBut[i, j].Visibility = Visibility.Hidden;//Скрывает все кнопки
                        }

                    //Отображение главного меню
                    nameGame.Visibility = Visibility.Visible;
                    easyLevel.Visibility = Visibility.Visible;
                    normalLevel.Visibility = Visibility.Visible;
                    hardLevel.Visibility = Visibility.Visible;
                    ComplexityL.Visibility = Visibility.Visible;
                    menuButton.Visibility = Visibility.Hidden;

                }
            }
        }
        /// <summary>
        /// Функция клика на кнопку Меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void menuButton_Click(object sender, RoutedEventArgs e)
        {
            //Отображает элементы меню
            nameGame.Visibility = Visibility.Visible;
            easyLevel.Visibility = Visibility.Visible;
            normalLevel.Visibility = Visibility.Visible;
            hardLevel.Visibility = Visibility.Visible;
            ComplexityL.Visibility = Visibility.Visible;

            //Скрывает все элементы, отобразившиеся при поражении
            loseGame.Visibility = Visibility.Hidden;
            menuButton.Visibility = Visibility.Hidden;
            mediaElement.Visibility = Visibility.Hidden;
        }
    }
}
