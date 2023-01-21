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
using System.Collections;

namespace Madzong
{
    public class Point
    {
        public int x;
        public int y;

        public Point()
        {
            x = 0;
            y = 0;
        }
        public Point(int X, int Y)
        {
            x = X;
            y = Y;
        }

    }

    public class Card
    {
        public Brush brush { set; get; }
        public string content { set; get; }

        public Card()
        {
            brush = Brushes.Aqua;
            content = null;
        }

        public Card(Brush color, string name)
        {
            brush = color;
            content = name;
        }

        public Card(string name)
        {
            content = name;
        }

        public Card(Brush color)
        {
            brush = color;            
        }
    }

    public class ArrayButtonGame
    {
        public int row { set; get; }
        public int column { set; get; }
        public Card[] buttons;
        public int[,] matrisa;
        Random random = new Random();
        //ArrayList arrayList = new ArrayList();


        string[] content = new string[]
        {
            "\u2743", "\u2661", "\u2745", "\u2746", "\u2739", "\u2749",
            "\u2727", "\u2735", "\u2600", "\u2741", "\u2740"//, null, null
        };
        Brush[] color = new Brush[]
        {
            Brushes.Turquoise, Brushes.Yellow, Brushes.DodgerBlue, Brushes.Lavender,
            Brushes.LightPink, Brushes.LightSeaGreen, Brushes.LightYellow,
            Brushes.Chocolate, Brushes.CornflowerBlue, Brushes.YellowGreen,
            Brushes.Orchid, Brushes.LightSkyBlue, Brushes.LightSalmon, Brushes.DarkGoldenrod
        };

        public ArrayButtonGame()
        {
            row = 6;
            column = 12;
            buttons = new Card[row*column];
            for (int i = 0; i < row * column; i++)
                buttons[i] = new Card();
            matrisa = new int[row + 2, column + 2];
        }

        public ArrayButtonGame(int n)
        {
            switch (n)
            {
                case 18:
                    row = 3; break;
                case 72:
                    row = 6; break;
                case 162:
                    row = 9; break;
                default:
                    row = 6; break;
            }

            column = n / row;
            buttons = new Card[row * column];
            for (int i = 0; i < row * column; i++)
                buttons[i] = new Card();
            matrisa = new int[row + 2, column + 2];
        }

        public void CreateArrayCard()
        {      
            for (int i = 0; i < buttons.Length; i+=2)                
            {
                Brush brush_rand = color[random.Next(0, color.Length - 1)];
                string content_rand = content[random.Next(0, content.Length - 1)];

                buttons[i].brush = brush_rand;
                buttons[i].content = content_rand;
                buttons[i+1].brush = brush_rand;
                buttons[i+1].content = content_rand;
            }
            MixCards();
        }

        public void CreateMatrisa()
        {
            int i, n1, n2;
            
            for (i = 0; i < row + 2; i++)
            {
                matrisa[i, 0] = 0;
                matrisa[i, column + 1] = 0;
            }
            for (i = 0; i < column + 2; i++)
            {
                matrisa[0, i] = 0;
                matrisa[0, row + 1] = 0;
            }

            for (i = 0; i < row*column; i++)
            {
                n1 = i / column + 1;
                n2 = i % column + 1;
                if (buttons[i].content == null)
                    matrisa[n1, n2] = 0;
                else
                    matrisa[n1, n2] = 100;
            }
        }

        //проверка, есть ли свободный путь от точки (i1,j1) до точки (i2,j2)
        public bool Check_path(int i1, int j1, int i2, int j2)
        {
            //создание массива матрица
            CreateMatrisa();

            //количество строк и колонок в матрице
            int R = row + 2;
            int C = column + 2;

            //если обе кнопки находятся на последней линии с любой стороны
            //на одной стороне, то они могут быть удалены 
            if ((i1 == 1 && i2 == 1) || (j1 == 1 && j2 == 1) ||
                (i1 == R - 2 && i2 == R - 2) ||
                (j1 == C - 2 && j2 == C - 2))
                return true;
            //если кнопки расположены рядом
            if ((Math.Abs(i1 - i2) == 1 && j1 == j2) || (Math.Abs(j1 - j2) == 1 && i1 == i2))
                return true;

            //если одна из кнопок заблокирована со всех сторон
            if ((matrisa[i1 + 1, j1] == 100 &&  //первая кнопка
                 matrisa[i1 - 1, j1] == 100 &&
                 matrisa[i1, j1 + 1] == 100 &&
                 matrisa[i1, j1 - 1] == 100)
                 ||
                 (matrisa[i2 + 1, j2] == 100 && //вторая кнопка
                  matrisa[i2 - 1, j2] == 100 &&
                  matrisa[i2, j2 + 1] == 100 &&
                  matrisa[i2, j2 - 1] == 100))
                return false;
            else
            {               
                return FindShortPath(i1, j1, i2, j2);
            }
                
        }

        //поиск самого короткого пути по волновому алгоритму
        private bool FindShortPath(int i1, int j1, int i2, int j2)
        {
            int n = 0;
            //MainWindow mainWindow = new MainWindow();
            //проверяем, есть ли в окружении заданной точки пустые клетки
            List<Point> arrBlank = CheckBlank(i1, j1);
            if (arrBlank.Count == 0) //пустых точек нет, путь не найден
                return false;
            else
            {
                n++;  //индекс 
                for (int i = 0; i < arrBlank.Count; i++)
                {
                    matrisa[arrBlank[i].x, arrBlank[i].y] = n;
                    //проверяем, не находятся ли найденные точки рядом с финишной
                    if (CheckFinish(arrBlank[i].x, arrBlank[i].y, i2, j2) == true)
                        return true;
                }
            }
                        

            //ищем путь до финишной точки используя волновой алгоритм            
            while (n < (column + row))
            {
                for (int i = 0; i < row + 2; i++)
                    for (int j = 0; j < column + 2; j++)
                    {
                        if (matrisa[i,j] == n)
                        {
                            arrBlank = CheckBlank(i, j);
                            if (arrBlank.Count == 0) //пустых точек нет, путь не найден
                                continue;
                            else
                            {                                
                                for (int k = 0; k < arrBlank.Count; k++)
                                {
                                    matrisa[arrBlank[k].x, arrBlank[k].y] = n + 1;
                               
                                    
                                    
                                    //проверяем, не находятся ли найденные точки рядом с финишной
                                    if (CheckFinish(arrBlank[k].x, arrBlank[k].y, i2, j2) == true)
                                        return true;
                                }
                            }                          
                        }    
                    }
                n++;
            }

            return false;

        }

        //ф-я проверяет, есть ли в окружении точки с координатами (i,j)
        //свободные клетки и если такие точки есть, возврат массива таких точек
        private List<Point> CheckBlank(int i, int j)
        {
            List<Point> arrBlank = new List<Point>();     
            if (i + 1 >= 0 && i + 1 <= row + 1) //проверяем, не находится ли точка на краю массива
            {
                if (matrisa[i + 1, j] == 0)
                {
                    arrBlank.Add(new Point(i + 1, j));
                }
            }
            if (i - 1 >= 0 && i - 1 <= row + 1) //проверяем, не находится ли точка на краю массива
            {
                if (matrisa[i - 1, j] == 0)
                {
                    arrBlank.Add(new Point(i - 1, j));
                }
            }
            if (j + 1 >= 0 && j + 1 <= column + 1) //проверяем, не находится ли точка на краю массива
            {
                if (matrisa[i, j + 1] == 0)
                {
                    arrBlank.Add(new Point(i, j + 1));
                }
            }
            if (j - 1 >= 0 && j - 1 <= column + 1) //проверяем, не находится ли точка на краю массива
            {
                if (matrisa[i, j - 1] == 0)
                {
                    arrBlank.Add(new Point(i, j - 1));
                }
            }
                

            return arrBlank;
        }



        //ф-я проверяет, не находится ли в прилегающих клетках
        //финишная точка
        private bool CheckFinish(int i1, int j1, int i2, int j2)
        {
            if ((i1 + 1 == i2 && j1 == j2) ||
                (i1 - 1 == i2 && j1 == j2) ||
                (i1 == i2 && j1 + 1 == j2) ||
                (i1 == i2 && j1 - 1 == j2))                
                return true;
            else
                return false;
        }



        public void MixCards()
        {
            int len = buttons.Length, Num1, Num2;
            Card temp = new Card();
            for (int i = 0; i < len; i++)
            {
                Num1 = random.Next(0, len);
                Num2 = random.Next(0, len);
                temp = buttons[Num1];
                buttons[Num1] = buttons[Num2];
                buttons[Num2] = temp;
            }
        }

        
    }
}