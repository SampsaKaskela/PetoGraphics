using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace PetoGraphics
{
    public class GraphicAnimation
    {
        private DispatcherTimer delayTimer = new DispatcherTimer();
        private Graphic targetGraphic;
        private bool isIn;

        public GraphicAnimation(Graphic target, bool isIn)
        {
            targetGraphic = target;
            delayTimer.Tick += DelayTimer_Tick;
            this.isIn = isIn;
        }

        public enum AnimationStyle
        {
            Fade = 0,
            SlideUp = 1,
            SlideDown = 2,
            SlideLeft = 3,
            SlideRight = 4,
            WipeUp = 5,
            WipeLeft = 6,
            SquishY = 7,
            SquishX = 8
        }

        public bool AddFade { get; set; } = false;

        public double Delay { get; set; } = 0;

        public double Duration { get; set; } = 500;

        public bool Ease { get; set; } = false;

        public double EasePower { get; set; } = 2;

        public AnimationStyle Style { get; set; } = AnimationStyle.Fade;

        public void Play()
        {
            if (Delay > 0)
            {
                delayTimer.Interval = TimeSpan.FromMilliseconds(Delay);
                delayTimer.Start();
            }
            else
            {
                if (isIn)
                {
                    if (targetGraphic.Image.IsSequence)
                    {
                        targetGraphic.Image.StartSequence();
                    }
                    PlayIn();
                }
                else
                {
                    if (targetGraphic.Image.IsSequence)
                    {
                        targetGraphic.Image.StopSequence();
                    }
                    PlayOut();
                }
            }
        }

        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            delayTimer.Stop();
            if (isIn)
            {
                if (targetGraphic.Image.IsSequence)
                {
                    targetGraphic.Image.StartSequence();
                }
                PlayIn();
            }
            else
            {
                if (targetGraphic.Image.IsSequence)
                {
                    targetGraphic.Image.StopSequence();
                }
                PlayOut();
            }
        }

        // Execute show animation
        private void PlayIn()
        {
            DependencyProperty property = null;
            double targetValue = 0;
            Prepare();
            switch (Style)
            {
                case AnimationStyle.Fade:
                    property = UIElement.OpacityProperty;
                    targetGraphic.Opacity = 0;
                    targetValue = 1;
                    break;

                case AnimationStyle.SlideUp:
                    property = Canvas.TopProperty;
                    Canvas.SetTop(targetGraphic, -targetGraphic.Height);
                    targetValue = targetGraphic.Y;
                    break;

                case AnimationStyle.SlideDown:
                    property = Canvas.TopProperty;
                    Canvas.SetTop(targetGraphic, Windows.Source.Height);
                    targetValue = targetGraphic.Y;
                    break;

                case AnimationStyle.SlideLeft:
                    property = Canvas.LeftProperty;
                    Canvas.SetLeft(targetGraphic, -targetGraphic.Width);
                    targetValue = targetGraphic.X;
                    break;

                case AnimationStyle.SlideRight:
                    property = Canvas.LeftProperty;
                    Canvas.SetLeft(targetGraphic, Windows.Source.Width);
                    targetValue = targetGraphic.X;
                    break;

                case AnimationStyle.WipeUp:
                    property = FrameworkElement.HeightProperty;
                    targetGraphic.Height = 0;
                    targetValue = targetGraphic.GraphicHeight;
                    break;

                case AnimationStyle.WipeLeft:
                    property = FrameworkElement.WidthProperty;
                    targetGraphic.Width = 0;
                    targetValue = targetGraphic.GraphicWidth;
                    break;

                case AnimationStyle.SquishY:
                    property = ScaleTransform.ScaleYProperty;
                    ((ScaleTransform)targetGraphic.RenderTransform).ScaleY = 0;
                    targetValue = 1;
                    break;

                case AnimationStyle.SquishX:
                    property = ScaleTransform.ScaleXProperty;
                    ((ScaleTransform)targetGraphic.RenderTransform).ScaleX = 0;
                    targetValue = 1;
                    break;
            }
            DoubleAnimation showAnimation = new DoubleAnimation(targetValue, TimeSpan.FromMilliseconds(Duration));
            Timeline.SetDesiredFrameRate(showAnimation, (int)Math.Ceiling(Windows.OutputSettings.FPS));
            targetGraphic.Visibility = Visibility.Visible;
            if (Ease)
            {
                PowerEase ease = new PowerEase();
                ease.Power = EasePower;
                showAnimation.EasingFunction = ease;
            }
            if (Style == AnimationStyle.SquishY || Style == AnimationStyle.SquishX)
            {
                targetGraphic.RenderTransform.BeginAnimation(property, showAnimation);
            }
            else
            {
                targetGraphic.BeginAnimation(property, showAnimation);
            }
            if (AddFade)
            {
                targetGraphic.Opacity = 0;
                DoubleAnimation addFadeAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(Duration));
                Timeline.SetDesiredFrameRate(addFadeAnimation, (int)Math.Ceiling(Windows.OutputSettings.FPS));
                targetGraphic.BeginAnimation(UIElement.OpacityProperty, addFadeAnimation);
            }
        }

        // Execute hide animation
        private void PlayOut()
        {
            DependencyProperty property = null;
            double targetValue = 0;
            switch (Style)
            {
                case AnimationStyle.Fade:
                    property = UIElement.OpacityProperty;
                    targetValue = 0;
                    break;

                case AnimationStyle.SlideUp:
                    property = Canvas.TopProperty;
                    targetValue = -targetGraphic.Height;
                    break;

                case AnimationStyle.SlideDown:
                    property = Canvas.TopProperty;
                    targetValue = Windows.Source.Height;
                    break;

                case AnimationStyle.SlideLeft:
                    property = Canvas.LeftProperty;
                    targetValue = -targetGraphic.Width;
                    break;

                case AnimationStyle.SlideRight:
                    property = Canvas.LeftProperty;
                    targetValue = Windows.Source.Width;
                    break;

                case AnimationStyle.WipeUp:
                    property = FrameworkElement.HeightProperty;
                    targetValue = 0;
                    break;

                case AnimationStyle.WipeLeft:
                    property = FrameworkElement.WidthProperty;
                    targetValue = 0;
                    break;

                case AnimationStyle.SquishY:
                    property = ScaleTransform.ScaleYProperty;
                    targetValue = 0;
                    break;

                case AnimationStyle.SquishX:
                    property = ScaleTransform.ScaleXProperty;
                    targetValue = 0;
                    break;
            }

            DoubleAnimation hideAnimation = new DoubleAnimation(targetValue, TimeSpan.FromMilliseconds(Duration));
            Timeline.SetDesiredFrameRate(hideAnimation, (int)Math.Ceiling(Windows.OutputSettings.FPS));
            hideAnimation.Completed += (object sender, EventArgs e) =>
            {
                targetGraphic.Visibility = Visibility.Collapsed;
            };
            if (Ease)
            {
                PowerEase ease = new PowerEase();
                ease.Power = EasePower;
                hideAnimation.EasingFunction = ease;
            }
            if (Style == AnimationStyle.SquishY || Style == AnimationStyle.SquishX)
            {
                targetGraphic.RenderTransform.BeginAnimation(property, hideAnimation);
            }
            else
            {
                targetGraphic.BeginAnimation(property, hideAnimation);
            }
            if (AddFade)
            {
                DoubleAnimation addFadeAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(Duration));
                Timeline.SetDesiredFrameRate(addFadeAnimation, (int)Math.Ceiling(Windows.OutputSettings.FPS));
                targetGraphic.BeginAnimation(UIElement.OpacityProperty, addFadeAnimation);
            }
        }

        private void Prepare()
        {
            targetGraphic.BeginAnimation(UIElement.OpacityProperty, null);
            targetGraphic.BeginAnimation(Canvas.LeftProperty, null);
            targetGraphic.BeginAnimation(Canvas.TopProperty, null);
            targetGraphic.BeginAnimation(FrameworkElement.WidthProperty, null);
            targetGraphic.BeginAnimation(FrameworkElement.HeightProperty, null);
            targetGraphic.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            targetGraphic.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);

            targetGraphic.Opacity = 1;
            Canvas.SetLeft(targetGraphic, targetGraphic.X);
            Canvas.SetTop(targetGraphic, targetGraphic.Y);
            targetGraphic.Height = targetGraphic.GraphicHeight;
            targetGraphic.Width = targetGraphic.GraphicWidth;
            ((ScaleTransform)targetGraphic.RenderTransform).ScaleX = 1;
            ((ScaleTransform)targetGraphic.RenderTransform).ScaleY = 1;
        }
    }
}
