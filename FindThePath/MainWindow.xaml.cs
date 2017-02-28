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
        private static List<ObjectPoint> lastPath;
             
        /// <summary>
        /// Start path point
        /// </summary>
        private ObjectPoint A;
        /// <summary>
        /// End path point
        /// </summary>
        private ObjectPoint B;
        private static SolidColorBrush DEFAULT_NONE_BACKGROUND = Brushes.Transparent;
        private static SolidColorBrush DEFAULT_NONE_FOREGROUND = Brushes.Black;
        private static SolidColorBrush DEFAULT_PATH_BACKGROUND = Brushes.SandyBrown;
        private static SolidColorBrush DEFAULT_PATH_FOREGROUND = Brushes.Wheat;
        private static SolidColorBrush DEFAULT_KEY_POINT_BACKGROUND = Brushes.Brown;
        private static SolidColorBrush DEFAULT_KEY_POINT_FOREGROUND = Brushes.White;
        private static SolidColorBrush DEFAULT_BLOCK_BACKGROUND = Brushes.Black;
        private static SolidColorBrush DEFAULT_BLOCK_FOREGROUND = Brushes.Black;
        //private Style startPointStyle, endPointStyle, blockStyle, blankStyle;

        private string[,] GetStringArray(DataTable table)
        {
            string[,] result = new string[table.Columns.Count, table.Rows.Count];

            for (int x = 0; x < table.Columns.Count; x++)
            {
                for (int y = 0; y < table.Rows.Count; y++)
                {
                    var dump = table.Rows[y][x];
                    if (dump == null) { dump = ""; }
                    if (dump.ToString().ToLower().Contains('a')) { dump = "a"; }
                    else if (dump.ToString().ToLower().Contains('b')) { dump = "b"; }
                    else if (dump.ToString().ToLower().Contains('x')) { dump = "x"; }
                    else { dump = ""; }
                    result[x, y] = dump.ToString();
                }
            }

            return result;
        }

        private void FillTableFromArray(ref DataTable table, string[,] array)
        {
            string[,] result = new string[table.Columns.Count, table.Rows.Count];

            int arrayHeight = array.GetUpperBound(1) + 1;
            int arrayWidth = array.Length / arrayHeight;

            for (int y = 0; y < arrayHeight; y++)
            {
                for (int x = 0; x < arrayWidth; x++)
                {
                    table.Rows[y][x] = array[x, y];
                }
            }
        }

        private bool IsPathPointNull(ObjectPoint point)
        {
            if (point.X == -1 || point.Y == -1)
            {
                return true;
            }
            return false;
        }
        #region Get Cell
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T ?? GetVisualChild<T>(v);
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static DataGridCell GetCell(DataGrid host, DataGridRow row, int columnIndex)
        {
            if (row == null) return null;

            var presenter = GetVisualChild<System.Windows.Controls.Primitives.DataGridCellsPresenter>(row);
            if (presenter == null) return null;

            // try to get the cell but it may possibly be virtualized
            var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
            if (cell == null)
            {
                // now try to bring into view and retreive the cell
                host.ScrollIntoView(row, host.Columns[columnIndex]);
                cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
            }
            return cell;
        }
        #endregion
        #region Cell color
        public static bool ChangeBackgroundCellColor(DataGrid grid, int cellX, int cellY,
            SolidColorBrush newColor)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(cellY);
            if (row != null)
            {
                var cell = GetCell(grid, row, cellX);
                cell.Background = newColor;
                return true;
            }
            return false;
        }
        public static bool ChangeForegroundCellColor(DataGrid grid, int cellX, int cellY,
            SolidColorBrush newColor)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(cellY);
            if (row != null)
            {
                var cell = GetCell(grid, row, cellX);
                cell.Foreground = newColor;
                return true;
            }
            return false;
        }

        public static void ClearAllCellsColor(DataGrid grid)
        {
            for (int rowCounter = 0; rowCounter < grid.Items.Count; rowCounter++)
            {
                for (int columnCounter = 0; columnCounter < grid.Columns.Count; columnCounter++)
                {
                    ChangeBackgroundCellColor(grid, columnCounter, rowCounter, DEFAULT_NONE_BACKGROUND);
                    ChangeForegroundCellColor(grid, columnCounter, rowCounter, DEFAULT_NONE_FOREGROUND);
                }
            }
        }

        public static void RecolorAllCells(DataGrid grid, DataTable table)
        {
            for (int x = 0; x < table.Columns.Count; x++)
            {
                for (int y = 0; y < table.Rows.Count; y++)
                {
                    var dump = table.Rows[y][x];
                    if (dump == null) { dump = ""; }
                    switch (dump.ToString().ToLower())
                    {
                        case "a":
                        case "b":
                            ChangeBackgroundCellColor(grid, x, y, DEFAULT_KEY_POINT_BACKGROUND);
                            ChangeForegroundCellColor(grid, x, y, DEFAULT_KEY_POINT_FOREGROUND);
                            break;
                        case "x":
                            ChangeBackgroundCellColor(grid, x, y, DEFAULT_BLOCK_BACKGROUND);
                            ChangeForegroundCellColor(grid, x, y, DEFAULT_BLOCK_FOREGROUND);
                            break;
                        default:
                            ChangeBackgroundCellColor(grid, x, y, DEFAULT_NONE_BACKGROUND);
                            ChangeForegroundCellColor(grid, x, y, DEFAULT_NONE_FOREGROUND);
                            break;
                    }
                }
            }
        }
        #endregion

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            if (IsPathPointNull(A) || IsPathPointNull(B))
            {
                Console.WriteLine("One of points was null");
                return;
            }
            if (lastPath != null && lastPath.Count > 1)
            {
                for (int i = 0; i < lastPath.Count - 1; i++)
                {
                    var dump = _data.Rows[lastPath[i].Y][lastPath[i].X];
                    var dumpstr = dump.ToString().ToLower();
                    if (dumpstr != "x" && !dumpstr.Contains("a") && !dumpstr.Contains("b"))
                    {
                        ChangeBackgroundCellColor(_grid, lastPath[i].X, lastPath[i].Y, DEFAULT_NONE_BACKGROUND);
                        ChangeForegroundCellColor(_grid, lastPath[i].X, lastPath[i].Y, DEFAULT_NONE_FOREGROUND);
                    }
                }
            }
            // DataTable to array
            var arr = GetStringArray(_data);
            // Calculate the path
            lastPath = WaveAlgLee.MakeWay(ref arr, A, B);
            // Array to DataTable
            FillTableFromArray(ref _data, arr);

            // Recolor cells
            //RecolorAllCells(_grid, _data);

            Console.WriteLine("=== === ===\nResult path:");
            // Start point
            //ChangeBackgroundCellColor(_grid, A.X, A.Y, DEFAULT_KEY_POINT_BACKGROUND);
            //ChangeForegroundCellColor(_grid, A.X, A.Y, DEFAULT_KEY_POINT_FOREGROUND);
            // Without last(finish) point
            for (int i = 0; i < lastPath.Count - 1; i++)
            {
                Console.WriteLine("{0} point: {1}", i, lastPath[i].ToString());
                ChangeBackgroundCellColor(_grid, lastPath[i].X, lastPath[i].Y, DEFAULT_PATH_BACKGROUND);
                ChangeForegroundCellColor(_grid, lastPath[i].X, lastPath[i].Y, DEFAULT_PATH_FOREGROUND);
            }
            // Last point
            Console.WriteLine("{0} point: {1}", lastPath.Count - 1, lastPath[lastPath.Count - 1].ToString());
            //ChangeBackgroundCellColor(_grid, B.X, B.Y, DEFAULT_KEY_POINT_BACKGROUND);
            //ChangeForegroundCellColor(_grid, B.X, B.Y, DEFAULT_KEY_POINT_FOREGROUND);
            Console.WriteLine("=== === ===");
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            InitNewPath(ref _data);
            ClearAllCellsColor(_grid);
        }

        private void ReplaceOldPointToNew(ref DataTable table, ref ObjectPoint replacingPoint, ObjectPoint newPoint, string pointName)
        {
            if (replacingPoint.X != -1 && replacingPoint.Y != -1)
            {
                Console.WriteLine("Old point '{1}': {0}", replacingPoint.ToString(), pointName);
                replacingPoint.Obj = ObjectType.None;
                table.Rows[replacingPoint.Y][replacingPoint.X] = "";
                ChangeBackgroundCellColor(_grid, replacingPoint.X, replacingPoint.Y, DEFAULT_NONE_BACKGROUND);
                ChangeForegroundCellColor(_grid, replacingPoint.X, replacingPoint.Y, DEFAULT_NONE_FOREGROUND);
                Console.WriteLine("Deleted old '{0}' point from grid", pointName);
            }
            replacingPoint.X = newPoint.X;
            replacingPoint.Y = newPoint.Y;
            replacingPoint.Obj = newPoint.Obj;
            ChangeBackgroundCellColor(_grid, replacingPoint.X, replacingPoint.Y, DEFAULT_KEY_POINT_BACKGROUND);
            ChangeForegroundCellColor(_grid, replacingPoint.X, replacingPoint.Y, DEFAULT_KEY_POINT_FOREGROUND);
            Console.WriteLine("New point {0}: {1}", pointName, replacingPoint.ToString());
        }

        private void _grid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string newText = (e.EditingElement as TextBox).Text;
            int x = e.Column.DisplayIndex;
            int y = e.Row.GetIndex();
            switch (newText)
            {
                case "a":
                case "A":
                    ReplaceOldPointToNew(ref _data, ref A,
                                        new ObjectPoint(x, y, ObjectType.StartPoint),
                                        "A");
                    break;
                case "b":
                case "B":
                    ReplaceOldPointToNew(ref _data, ref B,
                                        new ObjectPoint(x, y, ObjectType.EndPoint),
                                        "B");
                    break;
                case "x":
                case "X":
                    // block (barrier)
                    ChangeBackgroundCellColor(_grid, x, y, DEFAULT_BLOCK_BACKGROUND);
                    ChangeForegroundCellColor(_grid, x, y, DEFAULT_BLOCK_FOREGROUND);
                    break;
                default:
                    Console.WriteLine("New point wasn't A or B: {0}. Deleted input value", newText);
                    (e.EditingElement as TextBox).Text = "";
                    ChangeBackgroundCellColor(_grid, x, y, DEFAULT_NONE_BACKGROUND);
                    ChangeForegroundCellColor(_grid, x, y, DEFAULT_NONE_FOREGROUND);
                    break;
            }
            _grid.UpdateLayout();
        }

        public void InitNewPath(ref DataTable dataTable)
        {
            A.X = A.Y = B.X = B.Y = -1;
            dataTable.Columns.Clear();
            dataTable.Rows.Clear();

            for (int i = 0; i < AREA_WIDTH; i++)
            {
                dataTable.Columns.Add(i.ToString());
            }

            for (int i = 0; i < AREA_HEIGHT; i++)
            {
                dataTable.Rows.Add(new string[AREA_WIDTH]);
            }
        }

        private void _grid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (_grid.CurrentCell.Item != DependencyProperty.UnsetValue)
            {
                var gen = _grid.ItemContainerGenerator;
                var rowIndex = gen.IndexFromContainer(gen.ContainerFromItem(_grid.CurrentCell.Item));
                coordX.Content = String.Format("X: {0}", _grid.CurrentCell.Column.DisplayIndex);
                coordY.Content = String.Format("Y: {0}", rowIndex);
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.Title += String.Format(" [{0} x {1}]", AREA_WIDTH, AREA_HEIGHT);            

            InitNewPath(ref _data);

            _grid.DataContext = _data.DefaultView;
            _grid.UpdateLayout();
        }
    }
}
