using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

namespace PetoGraphics
{
    public class Countdown : Graphic
    {
        private Button start;
        private Button reset;
        private DispatcherTimer count = new DispatcherTimer();
        private DateTime target;

        public Countdown(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "Countdown";
            count.Interval = TimeSpan.FromMilliseconds(200);
            count.Tick += Count_Elapsed;

            start = new Button();
            start.Content = "Start";
            start.HorizontalAlignment = HorizontalAlignment.Right;
            start.Margin = new Thickness(0, 10, 115, 0);
            start.VerticalAlignment = VerticalAlignment.Top;
            start.Width = 80;
            start.Click += new RoutedEventHandler(Start_Click);
            Controller.grid.Children.Add(start);

            reset = new Button();
            reset.Content = "Reset";
            reset.HorizontalAlignment = HorizontalAlignment.Right;
            reset.Margin = new Thickness(0, 10, 210, 0);
            reset.VerticalAlignment = VerticalAlignment.Top;
            reset.Width = 80;
            reset.Click += new RoutedEventHandler(Reset_Click);
            Controller.grid.Children.Add(reset);

            Texts = new GraphicText[1];
            GraphicWidth = 400;
            GraphicHeight = 150;
            X = 300;
            Y = 900;

            Texts[0] = new GraphicText(container);
            Texts[0].Name = "Timer";
            Texts[0].Content = "00:00:00";
            Texts[0].X = 0;
            Texts[0].Y = 40;
            Texts[0].FontSize = 60;
            Texts[0].FontWeight = FontWeights.Bold;
            Texts[0].Width = 300;
        }

        public bool StartAt { get; set; } = true;

        public string Format { get; set; } = "hh:mm:ss";

        public int RemainingHours { get; set; } = 0;

        public int RemainingMin { get; set; } = 5;

        public int RemainingSec { get; set; } = 0;

        public int StartHour { get; set; } = 10;

        public int StartMin { get; set; } = 0;

        public int StartSec { get; set; } = 0;

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            SetTargetTime();
            Count_Elapsed(null, null);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (start.Content.ToString() == "Start")
            {
                start.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF51"));
                start.Content = "Stop";
                SetTargetTime();
                count.Start();
            }
            else
            {
                start.ClearValue(BackgroundProperty);
                start.Content = "Start";
                count.Stop();
            }
        }

        private void SetTargetTime()
        {
            if (StartAt)
            {
                target = DateTime.Today.AddHours(StartHour).AddMinutes(StartMin).AddSeconds(StartSec);
                if (target < DateTime.Now)
                {
                    target = target.AddDays(1);
                }
            }
            else
            {
                target = DateTime.Now.AddHours(RemainingHours).AddMinutes(RemainingMin).AddSeconds(RemainingSec);
            }
        }

        private void Count_Elapsed(object sender, EventArgs e)
        {
            TimeSpan difference = target - DateTime.Now;
            Texts[0].Content = CreateFormat(difference.Hours, difference.Minutes, difference.Seconds);
            if (difference.Hours <= 0 && difference.Minutes <= 0 && difference.Seconds <= 0)
            {
                Texts[0].Content = CreateFormat(0, 0, 0);
                start.ClearValue(BackgroundProperty);
                start.Content = "Start";
                count.Stop();
            }
        }

        private string CreateFormat(int hour, int min, int sec)
        {
            string formatstring = Format.ToLower();
            string[] strings = { "hh", "mm", "ss", "h", "m", "s" };
            string[] replace = { hour.ToString("00"), min.ToString("00"), sec.ToString("00"),
                hour.ToString(), min.ToString(), sec.ToString() };
            for (int i = 0; i < strings.Length; i++)
            {
                formatstring = formatstring.Replace(strings[i], replace[i]);
            }
            return formatstring;
        }
    }
}
