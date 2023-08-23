using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoTaSapXepNoiBot
{
    public partial class MainWindow : Window
    {
        List<int> _numList = new List<int>();
        List<Rectangle> _rectList = new List<Rectangle>();
        Random _rand = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            // 1,2,5
            _numList.Clear();
            string[] spliter = txtNumList.Text.Split(',');
            foreach (string s in spliter)
            {
                _numList.Add(int.Parse(s));
            }

            cvMain.Children.Clear();

            Thread t = new Thread(() =>
            {
                run();
            });
            t.IsBackground = true;
            t.Start();
        }

        private void run()
        {
            // Vẽ rect angle
            for (int i = 0; i < _numList.Count; i++)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Color.FromRgb((byte)_rand.Next(1, 255), (byte)_rand.Next(1, 255), (byte)_rand.Next(1, 255)));
                    rect.Width = _numList[i] * 5;
                    rect.Height = 25;
                    rect.ToolTip = _numList[i];
                    rect.Tag = i;
                    rect.HorizontalAlignment = HorizontalAlignment.Left;
                    rect.VerticalAlignment = VerticalAlignment.Bottom;
                    rect.Margin = new Thickness(0, i * 35, 0, 0);

                    _rectList.Add(rect);
                    cvMain.Children.Add(rect);
                });
            }

            // Mô tả thuật toán
            for (int i = 0; i < _numList.Count; i++)
            {
                lblLanLap.Dispatcher.Invoke(() => lblLanLap.Content = "LẦN LẶP " + (i + 1));
                lblStatus.Dispatcher.Invoke(() => lblStatus.Content = $"Index đang set : {i}, giá trị hiện tại: {_numList[i]}");
                setActiveRect(i, i+1);
                Thread.Sleep(1000);

                for (int j = i + 1; j < _numList.Count; j++)
                {
                    lblStatus.Dispatcher.Invoke(() => lblStatus.Content = $"Index đang set : {i}, giá trị hiện tại: {_numList[i]}. Đang so sánh {i} với {j}");
                    setActiveRect(i, j);

                    if (_numList[i] > _numList[j])
                    {
                        int tmp = _numList[j];
                        _numList[j] = _numList[i];
                        _numList[i] = tmp;

                        // Trên giao diện, xử lý i
                        Rectangle rect1 = findRectByIndex(i);
                        int newTop1 = j * 35;
                        Rectangle rect2 = findRectByIndex(j);
                        int newTop2 = i * 35;

                        changeRectIndex(rect1, rect2, i, j);

                        Thread t1 = new Thread(() => changeLeftEffect(rect1, newTop1));
                        Thread t2 = new Thread(() => changeLeftEffect(rect2, newTop2));

                        t1.Start();
                        t2.Start();
                    }
                    Thread.Sleep(1000);
                }

                Thread.Sleep(1000);
            }
        }

        private void changeLeftEffect(Rectangle rect, int newTop)
        {
            this.Dispatcher.Invoke(() =>
            {
                ThicknessAnimation animation = new ThicknessAnimation(new Thickness(0, rect.Margin.Top, 0, 0), new Thickness(0, newTop, 0, 0), new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                rect.BeginAnimation(Rectangle.MarginProperty, animation);
            });


        }

        private Rectangle findRectByIndex(int id)
        {
            foreach (Rectangle rect in _rectList)
            {
                int tag = 0;
                this.Dispatcher.Invoke(() =>
                {
                    tag = Convert.ToInt32(rect.Tag);
                });
                if (tag == id)
                    return rect;
            }

            return null;
        }

        private void changeRectIndex(Rectangle rect1, Rectangle rect2, int index1 , int index2)
        {
            this.Dispatcher.Invoke(() =>
            {
                rect1.Tag = index2;
                rect2.Tag = index1;
            });
        }

        private void setActiveRect(int index1, int index2)
        {
            this.Dispatcher.Invoke(() =>
            {
                // Bỏ hết hiệu ứng
                foreach (Rectangle rect in _rectList)
                {
                    rect.StrokeThickness = 0;
                }

                // Bật hiệu ứng
                foreach (Rectangle rect in _rectList)
                {
                    int tag = 0;
                    this.Dispatcher.Invoke(() =>
                    {
                        tag = Convert.ToInt32(rect.Tag);
                    });
                    if (tag == index1)
                    {
                        rect.StrokeThickness = 2;
                        rect.Stroke = new SolidColorBrush(Colors.Red);
                    }
                    if (tag == index2)
                    {
                        rect.StrokeThickness = 2;
                        rect.Stroke = new SolidColorBrush(Colors.Yellow);
                    }

                }
            });
            
        }
    }
}
