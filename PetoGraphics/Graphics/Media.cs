using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace PetoGraphics
{
    public class Media : Graphic
    {
        private DispatcherTimer counter = new DispatcherTimer();
        private bool playing = false;

        public Media(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "Media";
            Controller.ShowText = "Play";
            Controller.HideText = "Stop";
            Controller.Active = false;
            Opacity = 0;
            Background = Brushes.Black;
            counter.Interval = TimeSpan.FromSeconds(1);
            counter.Tick += Count_Elapsed;

            GraphicWidth = 1920;
            GraphicHeight = 1080;
            X = 0;
            Y = 0;

            Player.Stretch = Stretch.Fill;
            Player.MediaEnded += Media_Ended;
            Player.MediaOpened += Media_Opened;
            Player.LoadedBehavior = MediaState.Manual;
            Player.UnloadedBehavior = MediaState.Manual;
            container.Children.Add(Player);
        }

        public override double GraphicWidth
        {
            get { return base.GraphicWidth; }
            set
            {
                base.GraphicWidth = value;
                Player.Width = value;
            }
        }

        public override double GraphicHeight
        {
            get { return base.GraphicHeight; }
            set
            {
                base.GraphicHeight = value;
                Player.Height = value;
            }
        }

        public MediaElement Player { get; } = new MediaElement();

        public string Source { get; set; } = string.Empty;

        public TimeSpan Duration { get; set; }

        public bool Loop { get; set; } = false;

        public double Volume
        {
            get { return Player.Volume * 200; }
            set { Player.Volume = value / 200; }
        }

        public override void Show()
        {
            if (Source != string.Empty)
            {
                Controller.ToggleChilren();
                if (Player.Source != new Uri(Source))
                {
                    Player.Source = new Uri(Source);
                }
                else
                {
                    AnimationIn.Play();
                }
                Player.Play();
                Count_Elapsed(null, null);
                counter.Start();
                playing = true;
            }
        }

        public override void Hide()
        {
            if (playing)
            {
                AnimationOut.Play();
                Player.Stop();
                counter.Stop();
                playing = false;
            }
        }

        private void Count_Elapsed(object sender, EventArgs e)
        {
            if (Player.NaturalDuration.HasTimeSpan)
            {
                TimeSpan remaining = Player.NaturalDuration.TimeSpan.Subtract(Player.Position);
                if ((remaining.Hours * 60) + remaining.Minutes == 0)
                {
                    Controller.button.Content = remaining.Seconds;
                }
                else
                {
                    Controller.button.Content = string.Format("{0:00}:{1:00}", (remaining.Hours * 60) + remaining.Minutes, remaining.Seconds);
                }
            }
        }

        private void Media_Opened(object sender, RoutedEventArgs e)
        {
            TimeSpan duration = Player.NaturalDuration.TimeSpan;
            if ((duration.Hours * 60) + duration.Minutes == 0)
            {
                Controller.button.Content = duration.Seconds;
            }
            else
            {
                Controller.button.Content = string.Format("{0:00}:{1:00}", (duration.Hours * 60) + duration.Minutes, duration.Seconds);
            }
            AnimationIn.Play();
        }

        private void Media_Ended(object sender, RoutedEventArgs e)
        {
            Player.Pause();
            if (Loop)
            {
                Player.Position = TimeSpan.FromMilliseconds(1);
                Player.Play();
            }
            else
            {
                Controller.Active = false;
            }
        }
    }
}
