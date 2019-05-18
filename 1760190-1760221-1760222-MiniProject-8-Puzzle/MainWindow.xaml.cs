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
        Image[,] _images = new Image[3, 3];
        int commonindex = 8;

        List<Image> _listImg = new List<Image>(9);

        // create steps
        const int MoveUp = 1;
        const int MoveDown = -1;
        const int MoveRight = -3;
        const int MoveLeft = 3;

        // create continue game
        bool IsContinueGame = true;

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
                            Tag = new Tuple<int, int, int>(i, j, i*3 + j)
                        };
                        
                        images.Add(img);
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
                            var img = images[indices[index]];


                            Choi.Children.Add(img);
                            

                            Canvas.SetLeft(img, i * (100 + 5)+30);
                            Canvas.SetTop(img, j * (newHeight + 5)+30);

                            _listImg.Add(img);

                            docao = newHeight;

                            indices.RemoveAt(index);
                        }
                        else
                        {
                            int index = rng.Next(indices.Count);
                            var img = images[indices[index]];

                            _listImg.Add(img);
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
        int _selectedIndex = -1;
        int _swapIndex = -1;
        double GetTop_selectedIndex;
        double GetLeft_selectedIndex;
    
        bool _isDragging = false;  

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastPos = e.GetPosition(this);

            for (int i = 0; i < _listImg.Count; i++)
            {
                // Get all bound of image
                var TopBound = Canvas.GetTop(_listImg[i]);
                var LeftBound = Canvas.GetLeft(_listImg[i]);
                var BottomBound = Canvas.GetTop(_listImg[i]) + _listImg[i].Height;
                var RightBound = Canvas.GetLeft(_listImg[i]) + _listImg[i].Width;

                //Get position image
                if ((_lastPos.X >= LeftBound && _lastPos.X <= RightBound) && (_lastPos.Y >= TopBound && _lastPos.Y <= BottomBound))
                {
                    _selectedIndex = i;
                }
            }
            
            // Checking _selectedIndex if _selectedIndex != -1 have to right active Mouse_Move
            if(_selectedIndex != -1)
            {
                _isDragging = true;
                GetTop_selectedIndex = Canvas.GetTop(_listImg[_selectedIndex]);
                GetLeft_selectedIndex = Canvas.GetLeft(_listImg[_selectedIndex]);
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;

            var curPos = e.GetPosition(this);

            for(int i = 0; i < _listImg.Count;i++)
            {
                // Get all bound of image swap
                var TopBound = Canvas.GetTop(_listImg[i]);
                var LeftBound = Canvas.GetLeft(_listImg[i]);
                var BottomBound = Canvas.GetTop(_listImg[i]) + _listImg[i].Height;
                var RightBound = Canvas.GetLeft(_listImg[i]) + _listImg[i].Width;

                //Get position image swap
                if ((curPos.X >= LeftBound && curPos.X <= RightBound) && (curPos.Y >= TopBound && curPos.Y <= BottomBound) && i != commonindex)
                {
                    _swapIndex = i;
                }
            }

            if (_swapIndex == _selectedIndex)
            {
                _swapIndex = commonindex;
            }

            Canvas.SetTop(_listImg[_selectedIndex], GetTop_selectedIndex);
            Canvas.SetLeft(_listImg[_selectedIndex], GetLeft_selectedIndex);

            // Check swap two images
            if (_swapIndex == commonindex && _selectedIndex == _swapIndex + MoveDown )
            {
                sendKey(Key.Down);

                GetTop_selectedIndex = Canvas.GetTop(_listImg[_selectedIndex]);
                GetLeft_selectedIndex = Canvas.GetLeft(_listImg[_selectedIndex]);

                if (CheckWin())
                {
                    MessageBox.Show("You Win!");
                    IsContinueGame = false;
                }
            }
            else if(_swapIndex == commonindex && _selectedIndex == _swapIndex + MoveUp )
            {
                sendKey(Key.Up);

                GetTop_selectedIndex = Canvas.GetTop(_listImg[_selectedIndex]);
                GetLeft_selectedIndex = Canvas.GetLeft(_listImg[_selectedIndex]);

                if (CheckWin())
                {
                    MessageBox.Show("You Win!");
                    IsContinueGame = false;
                }
            }
            else if(_swapIndex == commonindex && _selectedIndex == _swapIndex + MoveRight)
            {
                sendKey(Key.Right);
                if (CheckWin())
                {
                    MessageBox.Show("You Win!");
                    IsContinueGame = false;
                }
            }
            else if(_swapIndex == commonindex && _selectedIndex == _swapIndex + MoveLeft)
            {
                sendKey(Key.Left);

                GetTop_selectedIndex = Canvas.GetTop(_listImg[_selectedIndex]);
                GetLeft_selectedIndex = Canvas.GetLeft(_listImg[_selectedIndex]);

                if (CheckWin())
                {
                    MessageBox.Show("You Win!");
                    IsContinueGame = false;
                }
            }
            else
            {
                Canvas.SetTop(_listImg[_selectedIndex], GetTop_selectedIndex);
                Canvas.SetLeft(_listImg[_selectedIndex], GetLeft_selectedIndex);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var curPos = e.GetPosition(this);

                var dx = curPos.X - _lastPos.X + GetLeft_selectedIndex;
                var dy = curPos.Y - _lastPos.Y + GetTop_selectedIndex;

                Canvas.SetLeft(_listImg[_selectedIndex], dx);    
                Canvas.SetTop(_listImg[_selectedIndex], dy);
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
            if(IsContinueGame)
                sendKey(Key.Up);
        }

        private void ButtomButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsContinueGame)
                sendKey(Key.Down);
        }
       
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsContinueGame)
                sendKey(Key.Left);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsContinueGame)
                sendKey(Key.Right);
        }

        private void sendKey(Key key)
        {
            var e1 = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key) { RoutedEvent = Keyboard.KeyDownEvent };
            InputManager.Current.ProcessInput(e1);
        }

        private void SwichImg(int location)
        {
            // GetTop and GetLeft
            var GetTop = Canvas.GetTop(_listImg[commonindex + location]);
            var GetLeft = Canvas.GetLeft(_listImg[commonindex + location]);

            // Set location 
            if (location == -1)
                Canvas.SetTop(_listImg[commonindex + location], GetTop + _listImg[commonindex + location].Height + 5);
            else if (location == -3)
                Canvas.SetLeft(_listImg[commonindex + location], GetLeft + _listImg[commonindex + location].Width + 5);
            else if (location == 3)
                Canvas.SetLeft(_listImg[commonindex + location], GetLeft - _listImg[commonindex + location].Width - 5);
            else
                Canvas.SetTop(_listImg[commonindex + location], GetTop - _listImg[commonindex + location].Height - 5);

            //Swap two images
            var temp_img = new Image();
            temp_img = _listImg[commonindex];
            _listImg[commonindex] = _listImg[commonindex + location];
            _listImg[commonindex + location] = temp_img;

            // update location control
            commonindex += location;
        }
        
        private void Board_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key.ToString())
            {
                case "Down":
                    {
                        if (commonindex + MoveDown >= 0 && commonindex + MoveDown <= 8 && commonindex != 6 && commonindex != 3 && commonindex !=0)
                            SwichImg(MoveDown);
                    } break;
                case "Up":
                    {
                        if (commonindex + MoveUp >= 0 && commonindex + MoveUp <= 8 && commonindex != 2 && commonindex != 5 && commonindex != 8)
                            SwichImg(MoveUp);
                    } break;
                case "Left":
                    {
                        if (commonindex + MoveLeft >= 0 && commonindex + MoveLeft <= 8)
                            SwichImg(MoveLeft);
                    } break;
                case "Right":
                    {
                        if (commonindex + MoveRight >= 0 && commonindex + MoveRight <= 8)
                            SwichImg(MoveRight);
                    } break;
            }
            if (CheckWin())
            {
                MessageBox.Show("You Win!");
                IsContinueGame = false;
            }
        }

        int CheckWin_game = 0;

        private bool CheckWin()
        {
            for(int i = 0; i < 9;i++)
            {
                var tag = _listImg[i].Tag as Tuple<int, int, int>;

                if (tag.Item3 == i)
                {
                    CheckWin_game++;
                }
                else
                {
                    CheckWin_game = 0;
                    break;
                }
            }
            if (CheckWin_game == 9)
                return true;
            else
                return false;
        }
    }
}
