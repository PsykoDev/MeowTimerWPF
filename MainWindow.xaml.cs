using System;
using System.ComponentModel;
using System.Media;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MeowTimerWPF
{
    public partial class MainWindow : Window
    {

        int TimeVar;
        int TimeVarEDIT;
        TimeSpan OutputTime;
        string SonPath = @"Resources\Cat_Purring.wav";
        string ImgPath = "/Resources/big_sleep.png";
        SoundPlayer player;
        DispatcherTimer timer;
        Build build;
        bool color = false;
        string BaseTime = "00:05:00";


        public MainWindow()
        {
            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);

            InitializeComponent();

            build = new();
            LoadJSON();

            if(Build.Config.son != null)
                SonPath = Build.Config.son;

            if (Build.Config.img != null)
                NekoImg.Source = new BitmapImage(new Uri(Build.Config.img, UriKind.RelativeOrAbsolute));
            else
                NekoImg.Source = new BitmapImage(new Uri(ImgPath, UriKind.RelativeOrAbsolute));

            if (Build.Config.color)
            { LaGrid.Background = Brushes.White; color = true; }
            else
            { LaGrid.Background = Brushes.Black; color = false; }

            if (Build.Config.Timer != null)
                TimerText.Text = Build.Config.Timer;
            else
                TimerText.Text = BaseTime;

            CurrentSound.Text = "Current Sound " + SonPath.Split($"/")[1];

            player = new SoundPlayer(SonPath);
            player.Load();
        }

        async void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            await build.JsonUpdate(ImgPath, SonPath, color, BaseTime);
        }

        private async void LoadJSON()
        {
            await build.InitializeAsync();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.Visibility = Visibility.Hidden;
            PauseButton.Visibility = Visibility.Visible;
            StopButton.Visibility = Visibility.Visible;

            int hours = int.Parse(TimerText.Text.Split(":", StringSplitOptions.TrimEntries)[0]);
            int mins = int.Parse(TimerText.Text.Split(":", StringSplitOptions.TrimEntries)[1]);
            int secs = int.Parse(TimerText.Text.Split(":", StringSplitOptions.TrimEntries)[2]);

            TimeVar = hours * 60 * 60 + mins * 60 + secs;

            if (TimeVar == 0)
            {
                MessageBox.Show("Pas 0 t'abuse");
                TimerText.Text = "00:00:01";
                return;
            }
            else
            {

                OutputTime = TimeSpan.FromSeconds(TimeVar);

                TimerText.IsReadOnly = true;

                TimeVarEDIT = TimeVar;
                progressBar1.Maximum = TimeVar;
                progressBar1.Value = TimeVar;

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1); ;
                timer.Tick += Timer_Tick;
                timer.Start();
            }

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TimeVarEDIT = TimeVarEDIT - 1;
            sectime.Text = Convert.ToString(TimeVarEDIT) + " Seconds";
            TimeSpan Time = TimeSpan.FromSeconds(TimeVarEDIT);
            if (TimeVar > Convert.ToInt32("86400"))
            {
                string CompleteOutput = Time.ToString(@"dd\:hh\:mm\:ss");
                totaltime.Text = CompleteOutput;
            }
            else
            {
                string CompleteOutput = Time.ToString(@"hh\:mm\:ss");
                totaltime.Text = CompleteOutput;
            }
            progressBar1.Value = TimeVarEDIT;

            if(TimeVarEDIT == 0)
            {
                TimerText.IsReadOnly = false;
                PauseButton.Visibility = Visibility.Hidden;
                StopButton.Visibility = Visibility.Hidden;
                StartButton.Visibility = Visibility.Visible;
                timer.Stop();
                StartSound();
            }
        }

        private void StartSound()
        {
            player.PlayLooping();

            if (MessageBox.Show("Au boulot!") == MessageBoxResult.OK)
            {
                player.Stop();
            }

        }

        bool Paused = false;
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (!Paused)
            {
                PauseButton.Content = "Resume";
                Paused = true;
                timer.Stop();
            }
            else
            {
                PauseButton.Content = "Pause";
                Paused = false;
                timer.Start();
            }
            
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            PauseButton.Visibility = Visibility.Hidden;
            StopButton.Visibility = Visibility.Hidden;
            StartButton.Visibility = Visibility.Visible;
            TimerText.IsReadOnly = false;
            totaltime.Text = "00:00:00";
            sectime.Text = "0 Seconds";
            timer.Stop();
            TimeVar = 0;
            TimeVarEDIT = TimeVar;
            progressBar1.Value = TimeVar;
        }

        private void LightMode_Click(object sender, RoutedEventArgs e)
        {
            color = true;
            LaGrid.Background = Brushes.White; 
        }

        private void DarkMode_Click(object sender, RoutedEventArgs e)
        {
            color = false;
            LaGrid.Background = Brushes.Black;
        }
    }
}
