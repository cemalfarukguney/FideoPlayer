using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibVLCSharp.WPF;
using LibVLCSharp.Shared;
using Microsoft.Win32;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        LibVLC _libVlc;
        MediaPlayer _mediaPlayer;
        bool _isFullScreen = false;
        string fileName = "";
        public UserControl1()
        {
            InitializeComponent();
            fideoView.Loaded += VideoView_Loaded;
        }

        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            Core.Initialize();
            this.KeyDown += UserControl1_KeyDown;
            this.MouseDown += UserControl1_MouseDown;
            _libVlc = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVlc);
            fideoView.MediaPlayer = _mediaPlayer;  
            _mediaPlayer.Volume = 35;

        }

        private void UserControl1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (timeLabel.Visibility == Visibility.Hidden)
            {
                timeLabel.Content = (_mediaPlayer.Position * _mediaPlayer.Media.Duration / 1000) + "/" + (_mediaPlayer.Media.Duration / 1000);
                timeLabel.Visibility = Visibility.Visible;
            }
            else
                timeLabel.Visibility = Visibility.Hidden;
        }

        private void UserControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _isFullScreen)
            {
                Window.GetWindow(this).WindowState = WindowState.Normal;
                Window.GetWindow(this).WindowStyle = WindowStyle.SingleBorderWindow;
                _isFullScreen = false;
            }
            else if (e.Key == Key.Space)
            {
                if (_mediaPlayer.State == VLCState.Playing)
                    _mediaPlayer?.Pause();
                else
                    _mediaPlayer?.Play();
            }
            else if (e.Key == Key.Right)
            {
                _mediaPlayer.Position += (float)(5.0 / (_mediaPlayer.Media.Duration/1000));
            }
            else if (e.Key == Key.Left)
            {
                _mediaPlayer.Position -= (float)(5.0 / (_mediaPlayer.Media.Duration / 1000));
            }
            else if (e.Key == Key.F)
            {
                Window.GetWindow(this).WindowState = WindowState.Maximized;
                Window.GetWindow(this).WindowStyle = WindowStyle.None;
                _isFullScreen = true;
            }
            else if (e.Key == Key.F5)
            {
                string pos = _mediaPlayer.Position.ToString();
                string fileName = _mediaPlayer.Media.Mrl;
                fileName = Uri.UnescapeDataString(fileName.Substring(8, fileName.Length - 12));

                using (StreamWriter writer = new StreamWriter(fileName + "_pos.txt"))
                {
                    writer.WriteLine(pos);
                }
                _mediaPlayer.Stop();
                selectVideoButton.Visibility = Visibility.Visible;
            }
            else if (e.Key == Key.Up)
            {
                _mediaPlayer.Volume += 5;
            }
            else if (e.Key == Key.Down)
            {
                _mediaPlayer.Volume -= 5;
            }
            else if (e.Key == Key.T)
            {
                if (timeLabel.Visibility == Visibility.Hidden)
                {
                    TimeSpan t = TimeSpan.FromSeconds(_mediaPlayer.Position * _mediaPlayer.Media.Duration / 1000);

                    string t1= string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds);

                    TimeSpan f = TimeSpan.FromSeconds(_mediaPlayer.Media.Duration / 1000);
                    string f1 = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    f.Hours,
                                    f.Minutes,
                                    f.Seconds);
                    timeLabel.Content = t1+ "/" + f1;
                    timeLabel.FontWeight = FontWeights.Bold;
                    timeLabel.FontSize = 40;
                    timeLabel.Foreground = System.Windows.Media.Brushes.White;
                    timeLabel.Visibility = Visibility.Visible;
                }
                else
                    timeLabel.Visibility = Visibility.Hidden;

            }
        }

        private void selectVideoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Media Files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;*.rmi;*.mkv;*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV";
            if (ofd.ShowDialog() == true)
            {
                fileName = ofd.FileName;
                _mediaPlayer.Play(new Media(_libVlc, new Uri(fileName)));
                if (File.Exists(fileName.Substring(0, fileName.Length - 4) + "_pos.txt"))
                {
                    float pos = float.Parse(File.ReadAllText(fileName.Substring(0, fileName.Length - 4) + "_pos.txt").ToString());
                    _mediaPlayer.Position = pos;
                }
                selectVideoButton.Visibility = Visibility.Hidden;
            }
        }
    }
}
