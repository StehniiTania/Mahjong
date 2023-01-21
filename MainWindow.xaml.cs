using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;


namespace Madzong
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ArrayButtonGame arrayCard = new ArrayButtonGame();
        Grid GridButtons = new Grid();
        Button b1 = new Button();
        Button b2 = new Button();
        Button but1 = new Button();
        Button but2 = new Button();
        bool flagHelp = false;

        //public UIElement GridButtons { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
                      
            
            arrayCard.CreateArrayCard();
            Create_Buttons();            
        }       

        //создание кнопок на основании массива карт
        public void Create_Buttons()
        {            
            GridCenter.Children.Add(GridButtons);

            for (int j = 0; j < arrayCard.row; j++)
            {
                GridButtons.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(600/arrayCard.column) });
            }

            for (int j = 0; j < arrayCard.column; j++)
            {
                GridButtons.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(600 / arrayCard.column) });
            }
                        
            for (int i = 0; i < arrayCard.row * arrayCard.column; i++)
            {
                //создаем ячейку Grid
                Grid grid = new Grid();

                grid.Name = "Grid" + i;
                GridButtons.Children.Add(grid);
                Grid.SetRow(grid, i / arrayCard.column);
                Grid.SetColumn(grid, i % arrayCard.column);

                
                Brush color_button = arrayCard.buttons[i].brush;
                string text = arrayCard.buttons[i].content;
                if (text != null)
                {
                    //Border border = new Border
                    //{
                    //    Background = color_button,
                    //    Width = 48,
                    //    Height = 48,
                    //    CornerRadius = new CornerRadius(10)
                    //};

                    //grid.Children.Add(border);
                    

                    //создаем кнопку
                    Button button = new Button
                    {
                        Name = "Button" + i,
                        Content = text,
                        Background = color_button,
                        FontSize = 312/arrayCard.column,
                        BorderBrush = color_button,
                        Width = 480/arrayCard.column,
                        Height = 480 / arrayCard.column
                    };

                    grid.Children.Add(button);                    
                    button.Click += button_Click;                    
                }
            }
        }
           
        //обработка клика кнопки
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Opacity == 0.2)
            {
                (sender as Button).Opacity = 1;               
            }
            else
            {
                if ((string)b1.Content == null)
                {
                    b1 = (sender as Button);
                    b1.Opacity = 0.2;                                    
                }
                else
                {
                    b2 = (sender as Button);
                    b2.Opacity = 0.2;
                }

                //если кнопки одинаковые, то запускается ф-ю, проверяющ. возможность их удаления
                if (b1.Content == b2.Content && b1.Background == b2.Background)
                {
                    //поиск координат сравниваемых точек в матрице
                    //через координаты Grid-а, в котором находится нажатая кнопка 
                    Grid grid1 = (Grid)(b1).Parent;
                    int N1 = int.Parse(grid1.Name.Substring(4));
                    int i1 = N1 / arrayCard.column + 1;
                    int j1 = N1 % arrayCard.column + 1;

                    Grid grid2 = (Grid)(b2).Parent;
                    int N2 = int.Parse(grid2.Name.Substring(4));
                    int i2 = N2 / arrayCard.column + 1;
                    int j2 = N2 % arrayCard.column + 1;

                    if (arrayCard.Check_path(i1, j1, i2, j2) == true)
                    {
                        deleteButtoms(b1, b2);
                        b1 = new Button();
                        b2 = new Button();
                    }
                        
                    //иначе возвращаем кнопкам первоначальную прозрачность
                    else if (b1.Content != null && b2.Content != null)
                    {
                        b1.Opacity = 1;
                        b2.Opacity = 1;
                        b1 = new Button();
                        b2 = new Button();
                    }
                }

                //иначе возвращаем кнопкам первоначальную прозрачность
                else if (b1.Content != null && b2.Content != null )
                {
                    b1.Opacity = 1;
                    b2.Opacity = 1;
                    b1 = new Button();
                    b2 = new Button();
                }                
            }     
        }        

        //удаление пары кнопок
        private void deleteButtoms(Button b1, Button b2)
        {
            
            //удаление пары кнопок
            Grid grid1 = (Grid)(b1).Parent;
            grid1.Children.Clear();

            Grid grid2 = (Grid)(b2).Parent;
            grid2.Children.Clear();

            int N1 = int.Parse(grid1.Name.Substring(4));
            int N2 = int.Parse(grid2.Name.Substring(4));

            //замена удаляемых карт из массива карт на карту Бланк
            //(просто удалять нельзя, т.к. потеряется очередность)
            Card blank = new Card();
            arrayCard.buttons[N1] = blank;
            arrayCard.buttons[N2] = blank;

            //обнуление временных переменных b1, b2          
            b1 = new Button();
            b2 = new Button();


            //проверяем, все ли карты убраны
            int n = 0;           
            foreach (Card card in arrayCard.buttons)
            {
                if (card.content == "" || card.content == null)
                    n++;
            }
            if (n == arrayCard.row * arrayCard.column)
                //если все карты убраны, игра окончена
                Finish();
        }

        //когда убраны все карты, становится видимым блок Финиш в блоке GridBottom
        private void Finish()
        {
            GridBottom.Opacity = 1;  
        }

        //обработка кнопок да/нет
        private void button_Click_YesNo(object sender, RoutedEventArgs e)
        {            
            if ((sender as Button).Name == "buttonYes1")
            {  //карты перемешиываются и игра продолжается              
                GridTop.Opacity = 0;
                arrayCard.MixCards();
                GridButtons.Children.Clear();
                Create_Buttons();
            }
            else if ((sender as Button).Name == "buttonYes")           
            {   //игра начинается заново
                GridBottom.Opacity = 0;                
                arrayCard.CreateArrayCard();
                GridButtons = new Grid();
                Create_Buttons();
            }
            else //окончание игры
                Close();
        }
                
        //обработка кнопки "помощь"
        private void Button_Click_Help(object sender, RoutedEventArgs e)
        {
            if (flagHelp == true)
            {
                but1.Opacity = 1;
                but2.Opacity = 1;
                flagHelp = false;
                return;
            }
            //ищем возможные пары одинаковых карт
            for (int i = 0; i < arrayCard.row * arrayCard.column; i++)
                for (int j = i + 1; j < arrayCard.row * arrayCard.column; j++)
                {  
                    //если карта "не пустая"
                    if (arrayCard.buttons[i].content != null && arrayCard.buttons[i].content != "")
                    {
                        if (arrayCard.buttons[i].brush == arrayCard.buttons[j].brush
                        && arrayCard.buttons[i].content == arrayCard.buttons[j].content)
                        {
                            //найдена пара одинаковых карт, ищем их координаты в матрице
                            //и проверяем на возможность их убрать с игрового стола
                            int x1, y1, x2, y2;
                            x1 = i / arrayCard.column + 1;
                            y1 = i % arrayCard.column + 1;
                            x2 = j / arrayCard.column + 1;
                            y2 = j % arrayCard.column + 1;
                            //если такая возможность есть
                            if (arrayCard.Check_path(x1, y1, x2, y2) == true)
                            {
                                Button button1 = new Button { Name = "Button" + i };
                                Button button2 = new Button { Name = "Button" + j };

                                foreach (UIElement it in GridButtons.Children)
                                {
                                    if (it is Grid)
                                    {
                                        foreach (var item in (it as Grid).Children)
                                        {
                                            if (item is Button)
                                            {
                                                if ((item as Button).Name == button1.Name || (item as Button).Name == button2.Name)
                                                {
                                                    //(item as Button).Opacity = 0.2;
                                                    if (but1.Opacity == 1)
                                                    {
                                                        but1 = item as Button;
                                                        but1.Opacity = 0.2;
                                                    }
                                                    else 
                                                    {
                                                        but2 = item as Button;
                                                        but2.Opacity = 0.2;
                                                        flagHelp = true;
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }                                
                            }
                        }
                    }
                    
                }
            //если промотрены все варианты и ход не найден
            //делаем видимым блок, где предлагается перемешать карты
            GridTop.Opacity = 1;
        }

        //обработка кнопки перемешивание карт
        private void Button_Click_Mix(object sender, RoutedEventArgs e)
        {
            arrayCard.MixCards();
            GridButtons.Children.Clear();
            GridButtons = new Grid();
            Create_Buttons();
        }

        //кнопки выбора количества карт
        private void Button_Click_Select(object sender, RoutedEventArgs e)
        {    
            if ((sender as Button).Name == "Game18")
                arrayCard = new ArrayButtonGame(18);

            if ((sender as Button).Name == "Game72")
                arrayCard = new ArrayButtonGame(72);

            if ((sender as Button).Name == "Game162")
                arrayCard = new ArrayButtonGame(162);

            arrayCard.CreateArrayCard();

            GridCenter.Children.Clear();
            GridButtons = new Grid();
            Create_Buttons();

        }
    }
}