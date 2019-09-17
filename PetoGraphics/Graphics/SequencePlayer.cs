using System.Windows.Controls;
using System;
using System.Windows.Threading;

namespace PetoGraphics
{
    public class SequencePlayer : Graphic
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private bool exit = false;

        public SequencePlayer(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "Sequence";
            Image.IsSequence = true;
            timer.Tick += SequenceFrame_Elapsed;

            GraphicWidth = 1920;
            GraphicHeight = 1080;
            X = 0;
            Y = 0;
        }

        public int InStartFrame { get; set; } = 0;

        public int LoopStartFrame { get; set; } = 0;

        public int OutStartFrame { get; set; } = 0;

        public override void Show()
        {
            Image.CurrentSequenceFrame = InStartFrame;
            exit = false;
            Controller.Active = true;
            timer.Start();
        }

        public override void Hide()
        {
            exit = true;
            Controller.Active = false;
        }

        private void SequenceFrame_Elapsed(object sender, EventArgs e)
        {
            if (Image.SequenceFrames.Count > 1)
            {
                if (Image.CurrentSequenceFrame >= OutStartFrame)
                {
                    if (exit)
                    {
                        if (Image.CurrentSequenceFrame >= Image.SequenceFrames.Count)
                        {
                            Image.CurrentSequenceFrame = InStartFrame;
                            timer.Stop();
                        }
                    }
                    else
                    {
                        Image.CurrentSequenceFrame = LoopStartFrame;
                    }
                }
                Image.UriSource = Image.SequenceFrames[Image.CurrentSequenceFrame];
                Image.CurrentSequenceFrame++;
            }
        }
    }
}
