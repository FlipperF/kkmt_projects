using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Scheduler
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class ShadulerCode : Window
    {
        public string file = "users.xml";//xml файл
        public XmlDocument xDoc = new XmlDocument();//переменная для работы с xml документом
        public XmlElement xRoot;//Получает основной элемент xml,  относительно которого производятся записи

        //Переменные, данные к которым присваиваются при создании задачи, для передачи в xml
        public string numXml = "";
        public string themeXml = "";
        public string textXml = null;
        public string datatimeXml = null;

        //Списки, формирующиеся при считывании xml файла, необходимы для созданя задач на экране
        public static List<string> Numbers = new List<string>();
        public static List<string> Themes = new List<string>();
        public static List<string> Texts = new List<string>();
        public static List<string> DataTimes = new List<string>();

        public int num = 0;//Кол-во "Stiker" в xml 
        public static string timeNOW;//Текущее дата и время компьютера

        public static System.Windows.Forms.NotifyIcon notify = new System.Windows.Forms.NotifyIcon();//Иконка свернутого в трей приложения
        public bool statusApp = true;//Статус приложения(открыто/свернуто)
        public System.Windows.Forms.ContextMenu contextMenu1 = new System.Windows.Forms.ContextMenu();//Контекстное меню при нажатии пкм по иконке в трее

        public static List<Button> buttonsMas = new List<Button>();

        public ShadulerCode()
        {
            InitializeComponent();
            createStyles();
            XmlRead();
            createPanel();
            threadForDate();
            Notifycation();

            App.LanguageChanged += LanguageChanged;//кол-во языков в программе
            CultureInfo currLang = App.Language;//Перевод приложения
            menuLanguage.Items.Clear();  //Отчистка меню смены языка:

            //Заполнение lixtbox с языками
            foreach (var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
              
                menuLang.Margin = new Thickness(0, 0, -120, 0);
                menuLang.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang; //Хранение выбранного языка
                menuLang.IsChecked = lang.Equals(currLang); //Проверка на тот же язык
                menuLang.Click += ChangeLanguageClick;
                menuLanguage.Items.Add(menuLang);//Добавление языка в листбокс
            }
        }

        /// <summary>
        /// Функция обработки события LanguageChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;

            //Отмечаем нужный пункт смены языка как выбранный язык
            foreach (MenuItem i in menuLanguage.Items)
            {
                CultureInfo ci = i.Tag as CultureInfo;
                i.IsChecked = ci != null && ci.Equals(currLang);
            }
        }

        /// <summary>
        /// Функция смены языка
        /// </summary>
        /// <param name="sender"> объект, на который кликнули</param>
        /// <param name="e">событие</param>
        private void ChangeLanguageClick(Object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                CultureInfo lang = mi.Tag as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }
            LangGrid.Visibility = Visibility.Hidden; //После выбора языка, меня скрывается

        }

        /// <createPanel>
        /// Функция создания заметок на экране
        /// </createPanel>
        private void createPanel()
        {
            for (int i = 0; i < Themes.Count; i++)
            {
                Expander expander = new Expander(); //экспандер, на котором все элементы и будут расположены
                StackPanel stackPanel1 = new StackPanel();//Выезжающее меню
                TextBlock dataTime_tb = new TextBlock();//Дата и время на панели
                TextBlock text_tb = new TextBlock();//Содержание на панели
                Grid inExpander = new Grid();//грид, для размещения кнопки и темы
                TextBlock toHeadEx = new TextBlock();//Тема на панели, которая видна изначально
               
                dataTime_tb.Style = Application.Current.FindResource("MaterialDesignBody") as Style; //Стиль даты/время из Material design
                string noSec = DataTimes[i];//Переменная для среза секунд
                //Условие для времяни 0:00 и 10:00
                if (noSec.Length == 18)
                    dataTime_tb.Text = noSec.Substring(0, 15); 
                else
                    dataTime_tb.Text = noSec.Substring(0, 16);

                //Настройка вида даты и время на панели
                dataTime_tb.FontFamily = new FontFamily("Comic Sans MS");
                dataTime_tb.FontSize = 12;
                dataTime_tb.Background = new SolidColorBrush(Colors.Black) { Opacity = 0.5 };//Цвет фона 
                dataTime_tb.Foreground = new SolidColorBrush(Colors.White);//Белый шрифт
                dataTime_tb.Margin = new Thickness(0);//нет отступов
                dataTime_tb.Padding = new Thickness(20, 10, 20, 10);//рамка

                //настройка вида панели с содержанием
                text_tb.Style = Application.Current.FindResource("MaterialDesignBody") as Style; //Стиль содержания из Material design
                text_tb.Opacity = 68;//Прозрачность
                text_tb.Text = Texts[i].ToString();//Значение из массива

                //Настройка шрифта
                text_tb.FontFamily = new FontFamily("Comic Sans MS");
                text_tb.FontSize = 12;

                text_tb.TextWrapping = TextWrapping.Wrap;
                text_tb.Background = new SolidColorBrush(Colors.Black) { Opacity = 0.5 };//цвет фона
                text_tb.Foreground = new SolidColorBrush(Colors.White);//Белый шрифт
                text_tb.Margin = new Thickness(0);//нет отступов
                text_tb.Padding = new Thickness(20, 2, 20, 0);//Рамка

                //Настройка шрифта
                toHeadEx.FontFamily = new FontFamily("Comic Sans MS");
                toHeadEx.FontSize = 18;

                toHeadEx.Text = Themes[i].ToString();// значение из массива
                toHeadEx.Margin = new Thickness(0, 0, 54, 0);//Отступы

                buttonsMas[i].Name = "btn" + i.ToString();

                buttonsMas[i].Margin = new Thickness(350,0,0,0);//Унопка прижата к левому краю
                buttonsMas[i].HorizontalAlignment = HorizontalAlignment.Left;

                buttonsMas[i].BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));//Убирает рамки кнопки
                ImageBrush myBrush = new ImageBrush();//Переменная изображения
                myBrush.ImageSource = new BitmapImage(new Uri("Resources/delete1.png",UriKind.Relative)); //Получение изображения из ресурса

                //Действие при клике на кнопку
                buttonsMas[i].Click += btn_Click;
                buttonsMas[i].Background = myBrush;
                //Добавление в грид экспандера кнопки и текста с темой
                inExpander.Children.Add(toHeadEx);
                inExpander.Children.Add(buttonsMas[i]);

                //Выезжающая вниз панель
                stackPanel1.Orientation = Orientation.Vertical;
                stackPanel1.Margin = new Thickness(0);

                //Настроки вида экспандера
                expander.HorizontalAlignment = HorizontalAlignment.Stretch; //Растянут до заполнения свободного места
                expander.Header = inExpander; //Помещение грида с кнопкой и темой в видимую часть
                expander.Margin = new Thickness(5, 0, 5, 0);//Отступы слева и справа по 5
                expander.Background = new SolidColorBrush(Colors.Black) { Opacity = 0.5 }; //черный бекграунд
                expander.Foreground = new SolidColorBrush(Colors.White);//Белый шрифт

                //В выезжающий стакпанел добавляется Дата\время и содежрание
                stackPanel1.Children.Add(dataTime_tb);
                stackPanel1.Children.Add(text_tb);
                
                // помещение выдвижной панели в экспандер
                expander.Content = stackPanel1;
                
                //Экспандр помещается в созданный в MainWindow.xaml элемент 
                listPanel.Children.Add(expander);
            }

        }
        /// <summary>
        /// Функция удаления запись с экрана и xml
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            //Отчистка всех массивов с данными
            Themes.Clear();
            Texts.Clear();
            DataTimes.Clear();
            buttonsMas.Clear();

            System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(file);//Открытие xml файла

            //Поиск совпадений по атрибуту num
            foreach (System.Xml.Linq.XElement xnode in xdoc.Root.Nodes())
            {
                string st = (sender as Button).Name;//Получение названия кнопки по передаваемому объекту
                string numBut = "";//Номер кнопки
                for (int i = 0; i < st.Length; i++)//Поиск номера кнопки в названии
                {
                    if (st[i] == '1' || st[i] == '2' || st[i] == '3' || st[i] == '4' || st[i] == '5' || st[i] == '6' || st[i] == '7' || st[i] == '8' || st[i] == '9' || st[i] == '0')
                        numBut += st[i];
                }
                //Удаления соответствующего узла из xml
                if (xnode.Attribute("Num").Value == numBut.ToString())
                {
                    xnode.Remove();
                }
            }
            //Заполнение атрибутов узла заного
            int x = 0;
                foreach (System.Xml.Linq.XElement xnode in xdoc.Root.Nodes())
                {
                    xnode.Attribute("Num").Value = x.ToString();
                    x++;
                }
            
            //Обработка исключения
            //При быстром удалении нескольких элементов файл не успевает закрыться и снова открыться
            try
            {
                xdoc.Save(file);
            }
            catch
            {
                MessageBox.Show("Повторите попытку через несколько секунд", "Слиокшом быстро");
            }
            
            //Пересоздание заметок на экране
            listPanel.Children.Clear();
            XmlRead();
            createPanel();

        }

        /// <summary>
        /// Функция отображения меню с добавдением записи
        /// Скрывает все прочие элементы, отображает грид добавления записи и кнопку назад
        /// </summary>
        /// <param name="sender">Нажатый объект</param>
        /// <param name="e">Событие</param>
        private void addSet_Click(object sender, RoutedEventArgs e)
        {
            AddSetGrid.Visibility = Visibility.Visible;
            AddSetBut.Visibility = Visibility.Hidden;
            backBut.Visibility = Visibility.Visible;
            listPanel.Visibility = Visibility.Hidden;
            LangGrid.Visibility = Visibility.Hidden;

        }
        /// <summary>
        /// функция отображения главного меню
        /// Скрывает все прочие элементы, отображает грид с меню и кнопку Добавить
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void backClick(object sender, RoutedEventArgs e)
        {
            backBut.Visibility = Visibility.Hidden;
            AddSetBut.Visibility = Visibility.Visible;
            AddSetGrid.Visibility = Visibility.Hidden;
            themesGrid.Visibility = Visibility.Hidden;
            listPanel.Visibility = Visibility.Visible;
            LangGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Функция сохранения добавленной записи на экран и в xml
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void saveClick(object sender, RoutedEventArgs e)
        {
            //Очистка массивов с данными для записи
            Themes.Clear();
            Texts.Clear();
            DataTimes.Clear();

            //Присваивание введенных данных соответствующим переменным для записи в xml
            themeXml = temaBox.Text;
            textXml = new TextRange(textToEl.Document.ContentStart, textToEl.Document.ContentEnd).Text;
            datatimeXml = dataEl.Text + " " + timeEl.Text + ":01";
            //Если что один из текстбоксов не заполнен, то выведет сообщение 
            if (themeXml == "" || textXml == "" || datatimeXml == "" || dataEl.Text == null || timeEl.Text == null)
            {
                MessageBox.Show("Присутствуют незаполненные поля");
            }
            else
            {
                //Отображение главного меню и скрывание грида добавления записей
                backBut.Visibility = Visibility.Hidden;
                AddSetBut.Visibility = Visibility.Visible;
                AddSetGrid.Visibility = Visibility.Hidden;
                listPanel.Visibility = Visibility.Visible;

                xmlAdd();//запись данных в xml
                listPanel.Children.Clear();//Отчистка Записей на экраане для перезаписи
                XmlRead();//Чтение файла
                createPanel();//Создание записей в главно меню

                //Очистка всех элементов создания записи
                temaBox.Text = "";
                textToEl.Text = null;
                dataEl.Text = null;
                timeEl.Text = null;
            }

        }

        /// <summary>
        /// Функция нажатия по кнопке Смена темы
        /// Скрывает все прочие элементы, отображает грид с темами и кнопку назад
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void themes_Click(object sender, RoutedEventArgs e)
        {
            themesGrid.Visibility = Visibility.Visible;
            AddSetBut.Visibility = Visibility.Hidden;
            backBut.Visibility = Visibility.Visible;
            AddSetGrid.Visibility = Visibility.Hidden;
            listPanel.Visibility = Visibility.Hidden;
            LangGrid.Visibility = Visibility.Hidden;

        }
        /// <summary>
        /// Функция по нажатию на Смену языка
        /// Скрывает все прочие элементы, отображает грид смены языка
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Событие</param>
        private void languages_click(object sender, RoutedEventArgs e)
        {
            AddSetGrid.Visibility = Visibility.Hidden;
            themesGrid.Visibility = Visibility.Hidden;
            LangGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Функция заполнения грида смены тем
        /// </summary>
        private void createStyles()
        {
            Button[,] listBut = new Button[3, 3];//матрица кнопок
            string[,] themesName = { { "Gradient", "Rain", "Relax" }, { "Sea", "Cat", "Nature" }, { "Girl", "Dark", "Fun" } }; //матрица названий
            //Заполнение сетки
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    listBut[i, j] = new Button(); //Создание кнопки
                    listBut[i, j].Content = themesName[i, j].ToString();//название из матрицы

                    //Условие для черного шрифта
                    if (themesName[i, j].ToString() == "Cat" || themesName[i, j].ToString() == "Girl")
                    {
                        listBut[i, j].Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    }
                    listBut[i, j].BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));//отсутствие границ

                    ImageBrush myBrush = new ImageBrush();//Переменная изображения
                    myBrush.ImageSource = new BitmapImage(new Uri("Resources/" + i.ToString() + j.ToString() + ".jpg", UriKind.Relative)); //Получение изображения из ресурса

                    listBut[i, j].Background = myBrush; //изображение на кнопку
                    //Размеры кнопки
                    listBut[i, j].Width = 140;
                    listBut[i, j].Height = 100;

                    listBut[i, j].Margin = new Thickness(5, 5, 5, 5);//отступы от границ ячейки
                    //РАсположение, относительно ячейки
                    listBut[i, j].VerticalAlignment = VerticalAlignment.Center;
                    listBut[i, j].HorizontalAlignment = HorizontalAlignment.Left;
                    listBut[i, j].FontSize = 24;//Шрифт
                    //listBut[i, j].HorizontalAlignment = HorizontalAlignment.Center;

                    //Клик по кнопке
                    listBut[i, j].Click += delegate
                    {
                        UpLine.Background = new SolidColorBrush(Color.FromArgb(80, 115, 120, 236));//Цвет верхней полосы программы
                        menuLanguage.Background = new SolidColorBrush(Color.FromArgb(80, 115, 120, 236));//Цвет окна выбора языка
                        mainGrid.Background = myBrush;//Цвет главного грида окна программы
                    };

                    //Добавление кнопки в ячейку на гриде
                    PicturesGrid.Children.Add(listBut[i, j]);
                    Grid.SetRow(listBut[i, j], i);
                    Grid.SetColumn(listBut[i, j], j);
                }
        }


        /// <summary>
        /// Функция записи в XML файл
        /// </summary>
        private void xmlAdd()
        {
            xDoc.Load(file);//Загрузка файла
            xRoot = xDoc.DocumentElement; //Получает корневой объект

            //Создание нового элемента Striker с атрибутом Num
            XmlElement Element = xDoc.CreateElement("Stiker");
            XmlAttribute numEl = xDoc.CreateAttribute("Num");

            //Создание остальных атрибутов
            XmlElement theme = xDoc.CreateElement("Theme");
            XmlElement text = xDoc.CreateElement("Content");
            XmlElement dataTime = xDoc.CreateElement("DataTime");

            //создание текстовых значений для элементов и атрибута
            XmlText numStick = xDoc.CreateTextNode(numXml);
            XmlText themeElem = xDoc.CreateTextNode(themeXml);
            XmlText content = xDoc.CreateTextNode(textXml);
            XmlText dataTimeElem = xDoc.CreateTextNode(datatimeXml);

            //добавляем узлы
            //К атрибутам привязывыем строки, соответствующие созданной записи
            numEl.AppendChild(numStick);
            theme.AppendChild(themeElem);
            text.AppendChild(content);
            dataTime.AppendChild(dataTimeElem);

            //Связь основного элемента с зависимыми
            Element.Attributes.Append(numEl);
            Element.AppendChild(theme);
            Element.AppendChild(text);
            Element.AppendChild(dataTime);
            num = 0;//Обнуление кол-ва "Stiker"
            xRoot.AppendChild(Element);//Добавление созданного узла к корню
            xDoc.Save(file);//Сохранение файла
        }

        /// <summary>
        /// Функция чтения xml файла
        /// </summary>
        private void XmlRead()
        {
            buttonsMas.Clear();
            listPanel.Children.Clear();
            XmlTextReader reader = new XmlTextReader(file); //Считываемый файл

            //Поиск узлов по названиям и запись их в массивы
            while (reader.Read())
            {
                if (reader.Name == "Stiker")
                    num++;
                if (reader.Name == "Theme")
                {
                    Themes.Add(reader.ReadInnerXml().ToString());

                }
                if (reader.Name == "Content")
                {
                    Texts.Add(reader.ReadInnerXml().ToString());

                }
                if (reader.Name == "DataTime")
                {
                    DataTimes.Add(reader.ReadInnerXml().ToString());
                }

            }
            for (int i = 0;i < Themes.Count; i++)
            {
                Button btn = new Button();
                buttonsMas.Add(btn);
            }
            numXml = (num / 2).ToString();//т.к считывается еще и закрывающий объект
        }

        /// <summary>
        /// Оповещение при наступлении времени из записи
        /// </summary>
        public void threadForDate()
        {
            //Запуск нового потока
            System.Threading.Tasks.Task.Run(() =>
            {
              while (true)
              {
                    // Получение текущего времени компьютера
                    timeNOW = System.DateTime.Now.ToShortDateString() +" "+ System.DateTime.Now.ToString("HH:mm:ss");
                    
                    //Если оно совпадает с одним из элементов массива DataTimes
                    if (DataTimes.Contains(timeNOW))
                    {
                        notify.Visible = true;//Отображает значек в трее
                        int index = DataTimes.IndexOf(timeNOW);//Получение индекса совпавшей записи
                        notify.BalloonTipTitle = "Планировщик";//Заголовок оповещения
                        notify.BalloonTipText = Themes[index] + "\n" + Texts[index];//Содержание оповещения

                        //Звук оповещения
                        MediaPlayer mpBgr = new MediaPlayer();//Переменная для звуков
                        mpBgr.Open(new Uri("Resources/Sound.wav", UriKind.Relative));//Загрузить звук
                        mpBgr.Play();//Проиграть звук

                        notify.ShowBalloonTip(10);//Время отображения оповещения(10 сек)
                    }
                    System.Threading.Thread.Sleep(1000);//Остановка работы потока на секунду
              }
            });
        }

        /// <summary>
        /// Создание значка в трее
        /// </summary>
        public void Notifycation()
        {
            notify.Icon = System.Drawing.SystemIcons.Application;//Иконка значка
            notify.Visible = false;//Изначально он скрыт, т.к приложение открыто
            createMenu();//Контекстное меню выход\открыть

            //Даблклик по значку откроет приложение и скроет значек
            notify.DoubleClick += delegate(object sender, EventArgs args)
            {
                if (statusApp == false)
                {
                    notify.Visible = false;
                    this.Show();
                }
            }; 
        }

        /// <summary>
        /// Сворачивание приложения в трей
        /// Срабатывает на клик по крестику
        /// </summary>
        /// <param name="sender">Оьъект</param>
        /// <param name="e">Событие</param>
        private void AppInTray(object sender, System.ComponentModel.CancelEventArgs e)
        {
            notify.Visible = true;//Отобразить значек в трее
            e.Cancel = true;//Отменяет закрытие приложения
            statusApp = false;// приложение скрыто
            notify.BalloonTipText = "Окно было свернуто";//Оповещение о сворачивании
            notify.ShowBalloonTip(5);//Время работы оповещения
            this.Hide();//Свернуть приложение
        }

        /// <summary>
        /// Создание контекстного меню при клике пкм по значку в трее
        /// </summary>
        public void createMenu()
        {
            // Создание элементов контекстного меню
            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem();
            open.Text = "Открыть";
            open.Click += delegate //Клик по элементу скроет значек и откроет приложение
            {
                notify.Visible = false; 
                this.Show();
            };
            contextMenu1.MenuItems.Add(open);
            System.Windows.Forms.MenuItem close = new System.Windows.Forms.MenuItem();
            close.Text = "Закрыть";
            close.Click += delegate //Клик по элементу закроет приложение
            {
                Environment.Exit(0);
            };
            contextMenu1.MenuItems.Add(close);//Добавление элементов в контекстное меню
            notify.ContextMenu = contextMenu1;//связь контекстного меню и иконки в трее
        }
    }
}
