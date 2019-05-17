using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace _1760190_1760221_1760222_MiniProject_8_Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
            
        }
        string filename = "";
        List<Image> images = new List<Image>();
       
        List<int> list_2chieu = new List<int>();
        int docao = 0;
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer _timer;
            TimeSpan _time;
            var screen = new OpenFileDialog();
            if (screen.ShowDialog() == true)
            {
                filename = screen.FileName;
                var originalImage = new BitmapImage(new Uri(filename));
                var image = new Image()
                {
                    Source = originalImage
                };
                Show.Children.Add(image);

                int width = originalImage.PixelWidth / 3;
                int height = originalImage.PixelHeight / 3;


                var newHeight = 100 * height / width;
                var images1 = new List<Image>();
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var cropped = new CroppedBitmap(originalImage,
                            new Int32Rect(i * width, j * height, width, height));

                        var img = new Image()
                        {
                            Source = cropped,
                            Width = 100,
                            Height = newHeight,
                            Tag = new Tuple<int, int, int>(i, j, 0)
                        };
                        


                        images.Add(img);
                        images1.Add(img);
                    }
                }

                var indices = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                var rng = new Random();
               
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (!(i == 2 && j == 2))
                        {
                            int index = rng.Next(indices.Count);
                            int index2 = indices[index];
                            var img = images[indices[index]];
                            Choi.Children.Add(img);

                            var tag = img.Tag as Tuple<int, int, int>;
                            img.Tag = new Tuple<int, int, int>
                                (tag.Item1, tag.Item2, j * 3 + i);

                            Canvas.SetLeft(img, i * (100 + 5)+30);
                            Canvas.SetTop(img, j * (newHeight + 5)+30);


                            docao = newHeight;
                            list_2chieu.Add(index2);

                            indices.RemoveAt(index);
                        }
                    }
                }
            }
            _time = TimeSpan.FromSeconds(180);

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                tbTime.Text = _time.ToString("c");
                if (_time == TimeSpan.Zero)
                {
                    //_timer.Stop();
                }
                _time = _time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);

            _timer.Start();
        }


        Point _lastPos;
    
        bool _isDragging = false;
        int move = -1;
        Image[,] _images = new Image[3, 3];


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastPos = e.GetPosition(this);

            //var left = Canvas.GetLeft(images[0]);
            //var top = Canvas.GetTop(images[0]);
            
            //MessageBox.Show(left.ToString());
            //MessageBox.Show(top.ToString());



            //for (int k = 0; k < images.Count; k++)
            //{


            //    var left = Canvas.GetLeft(images[k]);
            //    var top = Canvas.GetTop(images[k]);
            //    MessageBox.Show(left.ToString());
            //    if ((Left <= _lastPos.X && _lastPos.X <= Left + images[k].Width) &&
            //        (top <= _lastPos.Y && _lastPos.Y <= top + images[k].Height))
            //    {
            //        move = k;
            //        MessageBox.Show("ok");
            //    }
            //}
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //_isDragging = false;

            //var curPos = e.GetPosition(this);
            //var x = ((int)curPos.X) / 40 * 40;
            //var y = ((int)curPos.Y) / 40 * 40;
            //Canvas.SetLeft(_images[_lastI, _lastJ], x);
            //Canvas.SetTop(_images[_lastI, _lastJ], y);

        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (_isDragging)
            {
                var curPos = e.GetPosition(this);

                var dx = curPos.X - _lastPos.X;
                var dy = curPos.Y - _lastPos.Y;

                var imgg = images[move];

                var oldLeft = Canvas.GetLeft(imgg);
                var oldTop = Canvas.GetTop(imgg);

                Canvas.SetLeft(imgg, oldLeft + dx);
                Canvas.SetTop(imgg, oldTop + dy);



                _lastPos = curPos;
            }

         }










        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {



            var writer = new StreamWriter("Save.txt");
            writer.WriteLine(filename);
            
            for (int i = 0; i < list_2chieu.Count; i++)
            {
                writer.WriteLine(list_2chieu[i]);
            }
            writer.Close();
            MessageBox.Show("Da Luu Thanh Cong");
        }

        private void TopButton_Click(object sender, RoutedEventArgs e)
        {


            sendKey(Key.Up);


        }

        private void ButtomButton_Click(object sender, RoutedEventArgs e)
        {

            sendKey(Key.Down);
       

        }
       

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            sendKey(Key.Left);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            sendKey(Key.Right);
        }

        private void sendKey(Key key)
        {
            var e1 = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key) { RoutedEvent = Keyboard.KeyDownEvent };
            InputManager.Current.ProcessInput(e1);
        }

        private void SwichImg(int vitri)
        {

            var imgg = images[vitri];
            
            var oldLeft = Canvas.GetLeft(imgg);
            var oldTop = Canvas.GetTop(imgg);




            Canvas.SetLeft(imgg, oldLeft +10);
            //Canvas.SetTop(imgg, oldTop+(docao+35));

            

        }
        int index;
        private void Board_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key.ToString())
            {
                case "Down":
                    {
                        int temp = index;
                        index = index - 3 >= 0 ? index - 3 : index;
                        //MessageBox.Show("Toi La Down");
                        SwichImg(temp);
                    }
                    break;
                case "Up":
                    {
                        //MessageBox.Show("Toi La Up");

                        int temp = index;
                        index = index + 3 < 9 ? index + 3 : index;
                        SwichImg(temp);
                    }
                    break;
                case "Left":
                    {
                        //MessageBox.Show("Toi La Left");
                        if (index == 8 || index == 5 || index == 2)
                            break;

                        int temp = index;

                        SwichImg(temp);
                    }
                    break;
                case "Right":
                    {
                        if (index == 0 || index == 3 || index == 6)
                            break;

                        int temp = index;
                        index = index - 1 < 9 ? index - 1 : index;
                        //MessageBox.Show("Toi La Right");


                        SwichImg(temp);
                    }
                    break;
            }
        }
    }
}
