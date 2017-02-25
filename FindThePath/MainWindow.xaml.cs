// Нахождение пути на карте
// http://www.firststeps.ru/theory/karta.html

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
using System.Data;

namespace FindThePath
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int AREA_WIDTH = 50, AREA_HEIGHT = 30;
        private static DataTable _data = new DataTable();
        /// <summary>
        /// Start path point
        /// </summary>
        private ObjectPoint A;
        /// <summary>
        /// End path point
        /// </summary>
        private ObjectPoint B;
        //private Style startPointStyle, endPointStyle, blockStyle, blankStyle;

        {
        }

        {
        }


        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            InitNewPath(ref _data);
        }

        private void ReplaceOldPointToNew(ref DataTable table, ref ObjectPoint replacingPoint, ObjectPoint newPoint, string pointName)
        {
            if (replacingPoint.X != -1 && replacingPoint.Y != -1)
            {
                Console.WriteLine("Old point '{1}': {0}", replacingPoint.ToString(), pointName);
                replacingPoint.Obj = ObjectType.None;
                table.Rows[replacingPoint.X][replacingPoint.Y] = "";
                Console.WriteLine("Deleted old '{0}' point from grid", pointName);
            }
            replacingPoint.X = newPoint.X;
            replacingPoint.Y = newPoint.Y;
            replacingPoint.Obj = newPoint.Obj;
            Console.WriteLine("New point {0}: {1}", pointName, replacingPoint.ToString());
        }

        private void _grid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string newText = (e.EditingElement as TextBox).Text;
            switch (newText)
            {
                case "a":
                case "A":
                    ReplaceOldPointToNew(ref _data, ref A,
                                        new ObjectPoint(e.Row.GetIndex(), e.Column.DisplayIndex, 
                                                        ObjectType.StartPoint),
                                        "A");
                    break;
                case "b":
                case "B":
                    ReplaceOldPointToNew(ref _data, ref B,
                                        new ObjectPoint(e.Row.GetIndex(), e.Column.DisplayIndex,
                                                        ObjectType.EndPoint),
                                        "B");
                    break;
                case "x":
                case "X":
                    // block (barrier)
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

        public MainWindow()
        {
            InitializeComponent();

            this.Title += String.Format(" [{0} x {1}]", AREA_WIDTH, AREA_HEIGHT);


            InitNewPath(ref _data);

            _grid.DataContext = _data.DefaultView;
            _grid.HeadersVisibility = DataGridHeadersVisibility.None;
            _grid.UpdateLayout();

            //_data.Rows[0][5] = 5;
        }
    }
}
