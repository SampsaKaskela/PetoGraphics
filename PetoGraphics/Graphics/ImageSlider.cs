using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Collections.Generic;

namespace PetoGraphics
{
    public class ImageSlider : Graphic
    {
        private DispatcherTimer changeTimer;
        private GraphicImage slider1;
        private GraphicImage slider2;
        private int index = 0;

        public ImageSlider(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "Image Slider";
            changeTimer = new DispatcherTimer();
            changeTimer.Interval = TimeSpan.FromSeconds(10);
            changeTimer.Tick += ChangeTimer_Elapsed;
            changeTimer.Start();

            GraphicHeight = 300;
            GraphicWidth = 300;
            X = 1500;
            Y = 650;

            slider1 = new GraphicImage(container);
            slider1.Stretch = Stretch.Uniform;

            slider2 = new GraphicImage(container);
            slider2.Stretch = Stretch.Uniform;
            slider2.Opacity = 0;
        }

        public List<string> ImageSources { get; set; } = new List<string>();

        public double Duration
        {
            get
            {
                return changeTimer.Interval.TotalSeconds;
            }
            set
            {
                changeTimer.Interval = TimeSpan.FromSeconds(value);
            }
        }

        private void ChangeTimer_Elapsed(object sender, EventArgs e)
        {
            if (ImageSources.Count > 0)
            {
                if (index >= ImageSources.Count)
                {
                    index = 0;
                }
                DoubleAnimation fadeOut = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
                DoubleAnimation fadeIn = new DoubleAnimation(1, TimeSpan.FromSeconds(1));
                if (slider1.Opacity == 1)
                {
                    slider2.UriSource = ImageSources[index];
                    slider1.Image.BeginAnimation(MainWindow.OpacityProperty, fadeOut);
                    slider2.Image.BeginAnimation(MainWindow.OpacityProperty, fadeIn);
                }
                else
                {
                    slider1.UriSource = ImageSources[index];
                    slider1.Image.BeginAnimation(MainWindow.OpacityProperty, fadeIn);
                    slider2.Image.BeginAnimation(MainWindow.OpacityProperty, fadeOut);
                }
                index++;
            }
        }
    }
}
