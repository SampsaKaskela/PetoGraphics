using System.Windows.Controls;
using System.Windows;
using CefSharp.Wpf;
using System;

namespace PetoGraphics
{
    public class WebSource : Graphic
    {
        public WebSource(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "WebSource";

            GraphicWidth = 1920;
            GraphicHeight = 1080;
            X = 0;
            Y = 0;
            container.Children.Add(Browser);
        }

        public override double GraphicWidth
        {
            get { return base.GraphicWidth; }
            set
            {
                base.GraphicWidth = value;
                Browser.Width = value;
            }
        }

        public override double GraphicHeight
        {
            get { return base.GraphicHeight; }
            set
            {
                base.GraphicHeight = value;
                Browser.Height = value;
            }
        }

        public ChromiumWebBrowser Browser { get; } = new ChromiumWebBrowser();

        public string Url
        {
            get { return Browser.Address; }
            set
            {
                Browser.Address = value;
                Refresh(null, null);
            }
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            Browser.Load(Browser.Address);
        }
    }
}
