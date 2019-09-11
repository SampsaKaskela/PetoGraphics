using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;

namespace PetoGraphics
{
    public class Playlist : Graphic
    {
        private DispatcherTimer counter = new DispatcherTimer();
        private bool playing = false;
        private int currentIndex = 0;

        public Playlist(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "Playlist";
            Controller.ShowText = "Play";
            Controller.HideText = "Stop";
            Controller.Active = false;
            Opacity = 0;
            Background = Brushes.Black;
            counter = new DispatcherTimer();
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

        public List<Tuple<string, TimeSpan>> Sources { get; } = new List<Tuple<string, TimeSpan>>();

        public bool Loop { get; set; } = false;

        public double Volume
        {
            get { return Player.Volume * 200; }
            set { Player.Volume = Player.Volume / 200; }
        }

        public override void Show()
        {
            if (Sources.Count > 0)
            {
                Controller.ToggleChilren();
                currentIndex = 0;
                if (Player.Source != new Uri(Sources[currentIndex].Item1))
                {
                    Player.Source = new Uri(Sources[currentIndex].Item1);
                }
                else
                {
                    playing = true;
                    AnimationIn.Play();
                }
                Player.Play();
                Count_Elapsed(null, null);
                counter.Start();
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
            Controller.button.Content = GetTotalRemaining();
        }

        private void Media_Opened(object sender, RoutedEventArgs e)
        {
            Controller.button.Content = GetTotalRemaining();
            if (!playing)
            {
                playing = true;
                AnimationIn.Play();
            }
        }

        private string GetTotalRemaining()
        {
            TimeSpan total = TimeSpan.Zero;
            if (Player.NaturalDuration.HasTimeSpan)
            {
                total = total.Add(Player.NaturalDuration.TimeSpan.Subtract(Player.Position));
            }
            else
            {
                total = total.Add(Sources[currentIndex].Item2);
            }
            for (int i = currentIndex + 1; i < Sources.Count; i++)
            {
                total = total.Add(Sources[i].Item2);
            }
            if ((total.Hours * 60) + total.Minutes == 0)
            {
                return total.Seconds.ToString();
            }
            else
            {
                return string.Format("{0:00}:{1:00}", (total.Hours * 60) + total.Minutes, total.Seconds);
            }
        }

        private void Media_Ended(object sender, RoutedEventArgs e)
        {
            Player.Pause();
            currentIndex++;
            if (currentIndex < Sources.Count)
            {
                if (Player.Source != new Uri(Sources[currentIndex].Item1))
                {
                    Player.Source = new Uri(Sources[currentIndex].Item1);
                }
                Player.Play();
            }
            else
            {
                if (Loop)
                {
                    if (Sources.Count == 1)
                    {
                        Player.Position = TimeSpan.FromMilliseconds(1);
                        Player.Play();
                    }
                    else
                    {
                        currentIndex = 0;
                        if (Player.Source != new Uri(Sources[currentIndex].Item1))
                        {
                            Player.Source = new Uri(Sources[currentIndex].Item1);
                        }
                        Player.Play();
                    }
                }
                else
                {
                    Controller.Active = false;
                }
            }
        }
    }
}
