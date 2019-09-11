using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

namespace PetoGraphics
{
    public class Clock : Graphic
    {
        private DispatcherTimer count = new DispatcherTimer();

        public Clock(Canvas sourceCanvas) : base(sourceCanvas)
        {
            count.Interval = TimeSpan.FromSeconds(1);
            count.Tick += Count_Elapsed;
            count.Start();
            Controller.name.Content = "Clock";

            GraphicWidth = 200;
            GraphicHeight = 150;
            X = 1600;
            Y = 100;

            Texts = new GraphicText[1];

            Texts[0] = new GraphicText(container);
            Texts[0].Name = "Time";
            Texts[0].Content = DateTime.Now.ToString("HH:mm");
            Texts[0].X = 20;
            Texts[0].Y = 35;
            Texts[0].FontSize = 60;
            Texts[0].FontWeight = FontWeights.Bold;
            Texts[0].Width = 200;
        }

        private void Count_Elapsed(object sender, EventArgs e)
        {
            Texts[0].Content = DateTime.Now.ToString("HH:mm");
        }
    }
}
