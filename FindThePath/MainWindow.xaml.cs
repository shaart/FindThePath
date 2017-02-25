using System;
﻿// Нахождение пути на карте
// http://www.firststeps.ru/theory/karta.html
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
using System.Data;

namespace FindThePath
{
    public struct Point2D
    {
        public int X, Y;

        public Point2D(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", X, Y);
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int AREA_WIDTH = 50, AREA_HEIGHT = 30;
        private static DataTable _data = new DataTable();
        private Point2D A, B;

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            InitNewPath(ref _data);
        }


        private void _grid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string newText = (e.EditingElement as TextBox).Text;
            switch (newText)
            {
                case "a":
                case "A":
                    if ((A.X != -1)&&(A.Y != -1))
                    {
                        Console.WriteLine("Old point 'A': {0}", A.ToString());
                        _data.Rows[A.X][A.Y] = "";
                        Console.WriteLine("Deleted old 'A' point from grid");
                    }
                    A.X = e.Row.GetIndex();
                    A.Y = e.Column.DisplayIndex;
                    Console.WriteLine("New point A: {0}", A.ToString());
                    break;
                case "b":
                case "B":
                    if ((B.X != -1) && (B.Y != -1))
                    {
                        Console.WriteLine("Old point 'B': {0}", A.ToString());
                        _data.Rows[B.X][B.Y] = "";
                        Console.WriteLine("Deleted old 'B' point from grid");
                    }
                    B.X = e.Row.GetIndex();
                    B.Y = e.Column.DisplayIndex;
                    Console.WriteLine("New point B: {0}", B.ToString());
                    break;
                default:
                    Console.WriteLine("New point wasn't A or B: {0}. Deleted input value", newText);
                    (e.EditingElement as TextBox).Text = "";
                    break;
            }
            _grid.UpdateLayout();
        }

        public void InitNewPath(ref DataTable dataTable)
        {
            A.X = A.Y = B.X = B.Y = -1;
            _data.Columns.Clear();
            _data.Rows.Clear();

            for (int i = 0; i < AREA_WIDTH; i++)
            {
                _data.Columns.Add(i.ToString());
            }

            for (int i = 0; i < AREA_HEIGHT; i++)
            {
                _data.Rows.Add(new string[AREA_WIDTH]);
            }
        }


            InitNewPath(ref _data);

            _grid.DataContext = _data.DefaultView;
            _grid.HeadersVisibility = DataGridHeadersVisibility.None;
            _grid.UpdateLayout();

            //_data.Rows[0][5] = 5;
        }
    }
}
