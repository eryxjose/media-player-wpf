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

using Microsoft.Win32;

namespace MediaPlayerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();

        bool posSliderDragging = false;
        bool isPlaying = false;
        String trackPath = String.Empty; 

        public MainWindow()
        {
            InitializeComponent();
            InitMediaPlayer();
        }

        private void InitMediaPlayer()
        {
            isPlaying = false;
            MediaElementPlayer.Volume = 0.5;
            MediaElementPlayer.SpeedRatio = 1;
        }

        private void ShowPlayerControls(bool isVisible)
        {
            MediaElementPlayer.Visibility = Visibility.Visible;
            PosicaoSlider.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            VolumeSlider.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            btnPause.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            btnStart.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            //btnStop.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        #region Sliders


        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaElementPlayer.Volume = (double)VolumeSlider.Value;
        }

        private void PosicaoSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int SliderValue = (int)PosicaoSlider.Value;
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            MediaElementPlayer.Position = ts;
            MediaElementPlayer.Play();
            System.Threading.Thread.Sleep(10);
            MediaElementPlayer.Pause();
            btnStart.Visibility = Visibility.Visible;
            btnPause.Visibility = Visibility.Hidden;
        }



        #endregion


        private void Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            PosicaoSlider.Maximum = MediaElementPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            ShowPlayerControls(true);
        }

        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            int nextTrackIndex = -1;
            int numberOfTracks = -1;
            MediaElementPlayer.Stop();
            MediaElementPlayer.Position = new TimeSpan(0, 0, 0, 0, 5000);
            numberOfTracks = playlistContainer.Items.Count;
            if (numberOfTracks > 0)
            {
                nextTrackIndex = playlistContainer.SelectedIndex + 1;
                if (nextTrackIndex >= numberOfTracks)
                {
                    nextTrackIndex = 0;
                }
                playlistContainer.SelectedIndex = nextTrackIndex;
                PlayPlaylist();
            }
        }

        private void PlayPlaylist()
        {
            int selectedItemIndex = -1;
            if (playlistContainer.Items.Count > 0)
            {
                selectedItemIndex = playlistContainer.SelectedIndex;
                if (selectedItemIndex > -1)
                {
                    trackPath = playlistContainer.Items[selectedItemIndex].ToString();
                    trackLabel.Content = trackPath;
                    PlayTrack();
                }
            }
        }

        private void Media_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            // This fires if the media.Source can't be found or can't be played
            MessageBox.Show("Unable to play " + trackPath + " [" + e.ErrorException.Message + "]");
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            Nullable<bool> result;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "";
            dlg.DefaultExt = ".mp4";
            dlg.Filter = ".mp4|*.mp4|.mp3|*.mp3|.mpg|*.mpg|.wmv|*.wmv|All files (*.*)|*.*";
            dlg.CheckFileExists = true;
            result = dlg.ShowDialog();
            if (result == true)
            {
                //playlistContainer.Items.Clear();
                //playlistContainer.Visibility = Visibility.Hidden;
                // Open document
                trackPath = dlg.FileName;
                trackLabel.Content = trackPath;
                
                PlayTrack();
            }
        }

        private void PlayTrack()
        {
            bool ok = true;
            FileInfo fi = null;
            Uri src;
            try
            {
                fi = new FileInfo(trackPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ok = false;
            }
            if (ok)
            {
                // check that the file actually exists
                if (!fi.Exists)
                {
                    MessageBox.Show("Cannot find " + trackPath);
                }
                else
                {
                    
                    src = new Uri(trackPath);
                    MediaElementPlayer.Source = src;
                    MediaElementPlayer.SpeedRatio = 1;
                    MediaElementPlayer.Volume = VolumeSlider.Value;
                    MediaElementPlayer.Position = new TimeSpan(0, 0, 0, 0, 5000);
                    MediaElementPlayer.Play();
                    isPlaying = true;
                    btnStart.Visibility = Visibility.Hidden;
                    timer.Start();
                }
            }
        }
        private void TogglePlayPauseVisibility()
        {
            btnStart.Visibility = isPlaying ? Visibility.Hidden : Visibility.Visible;
            btnPause.Visibility = isPlaying ? Visibility.Visible : Visibility.Hidden;
            isPlaying = !isPlaying;
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            btnPause.Visibility = Visibility.Visible;
            btnStart.Visibility = Visibility.Hidden;
            MediaElementPlayer.Play();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            btnPause.Visibility = Visibility.Hidden;
            btnStart.Visibility = Visibility.Visible;
            MediaElementPlayer.Pause();
        }

    }
}
