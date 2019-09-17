using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System;
using System.IO;
using System.Windows.Threading;
using System.Linq;

namespace PetoGraphics
{
    public class GraphicImage
    {
        private bool isSequence = false;
        private DispatcherTimer sequenceTimer = new DispatcherTimer();
        private string uriSource = null;

        public GraphicImage(Grid container)
        {
            Image.Stretch = Stretch.Fill;
            Image.Margin = new Thickness(0, 0, 0, 0);
            RenderOptions.SetBitmapScalingMode(Image, BitmapScalingMode.HighQuality);
            container.Children.Add(Image);
            sequenceTimer.Interval = TimeSpan.FromMilliseconds(1000 / Windows.OutputSettings.FPS);
            sequenceTimer.Tick += SequenceFrame_Elapsed;
        }

        public Image Image { get; } = new Image();

        public int CurrentSequenceFrame { get; set; } = 0;

        public double Width
        {
            get { return Image.Width; }
            set { Image.Width = value; }
        }

        public double Height
        {
            get { return Image.Height; }
            set { Image.Height = value; }
        }

        public double X
        {
            get { return Image.Margin.Left; }
            set { Image.Margin = new Thickness(value, Image.Margin.Top, 0, 0); }
        }

        public double Y
        {
            get { return Image.Margin.Top; }
            set { Image.Margin = new Thickness(Image.Margin.Left, value, 0, 0); }
        }

        public bool IsSequence
        {
            get { return isSequence; }
            set
            {
                if (value)
                {
                    if (uriSource != null)
                    {
                        SequenceFrames = Directory.GetFiles(Path.GetDirectoryName(uriSource), "*" + Path.GetExtension(uriSource)).ToList();
                    }
                    else
                    {
                        SequenceFrames = new List<string>();
                    }
                }
                else
                {
                    SequenceFrames.Clear();
                    sequenceTimer.Stop();
                    if (uriSource != null)
                    {
                        BitmapImage bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri(uriSource);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        Image.Source = bmp;
                    }
                }
                isSequence = value;
            }
        }

        public string Name
        {
            get { return Image.Name; }
            set { Image.Name = value; }
        }

        public double Opacity
        {
            get { return Image.Opacity; }
            set { Image.Opacity = value; }
        }

        public List<string> SequenceFrames { get; set; } = new List<string>();

        public Stretch Stretch
        {
            get { return Image.Stretch; }
            set { Image.Stretch = value; }
        }

        public string UriSource
        {
            get { return uriSource; }
            set
            {
                uriSource = value;
                try
                {
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(value);

                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    if (Path.GetExtension(value) == ".gif")
                    {
                        WpfAnimatedGif.ImageBehavior.SetAnimatedSource(Image, bmp);
                    }
                    else
                    {
                        WpfAnimatedGif.ImageBehavior.SetAnimatedSource(Image, null);
                        Image.Source = bmp;
                    }

                    if (isSequence)
                    {
                        SequenceFrames = Directory.GetFiles(Path.GetDirectoryName(value), "*" + Path.GetExtension(value)).ToList();
                    }
                    else
                    {
                        SequenceFrames = new List<string>();
                    }
                }
                catch
                {
                    CustomMessageBox.Show("Image format not supported.");
                }
            }
        }

        public double FPS
        {
            set
            {
                sequenceTimer.Interval = TimeSpan.FromMilliseconds(1000 / value);
            }
        }

        public void Clear()
        {
            uriSource = null;
            sequenceTimer.Stop();
            SequenceFrames.Clear();
            Image.Source = null;
        }

        public void StartSequence()
        {
            CurrentSequenceFrame = 0;
            sequenceTimer.Start();
        }

        public void StopSequence()
        {
            sequenceTimer.Stop();
        }

        // Move to next frame in sequence
        private void SequenceFrame_Elapsed(object sender, EventArgs e)
        {
            if (SequenceFrames.Count > 1)
            {
                if (CurrentSequenceFrame >= SequenceFrames.Count)
                {
                    CurrentSequenceFrame = 0;
                }
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(SequenceFrames[CurrentSequenceFrame]);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                Image.Source = bmp;
                CurrentSequenceFrame++;
            }
        }
    }
}
