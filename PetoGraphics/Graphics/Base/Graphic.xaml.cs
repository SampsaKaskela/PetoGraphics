using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PetoGraphics
{
    /// <summary>
    /// Interaction logic for Graphic.xaml
    /// </summary>
    public abstract partial class Graphic : UserControl
    {
        private double width;
        private double height;
        private double x;
        private double y;

        public Graphic(Canvas sourceCanvas)
        {
            InitializeComponent();
            RenderTransform = new ScaleTransform();
            RenderTransformOrigin = new Point(0.5, 0.5);
            Panel.SetZIndex(this, 200 - GraphicController.rootList.Count);
            sourceCanvas.Children.Add(this);

            Image = new GraphicImage(container);
            Controller = new GraphicController(this);
            AnimationIn = new GraphicAnimation(this, true);
            AnimationOut = new GraphicAnimation(this, false);

            editor.Target = this;
            editor.Visibility = Windows.Source.Editing ? Visibility.Visible : Visibility.Collapsed;
        }

        public GraphicAnimation AnimationIn { get; private set; }

        public GraphicAnimation AnimationOut { get; private set; }

        public GraphicController Controller { get; private set; }

        public GraphicImage Image { get; private set; }

        public GraphicText[] Texts { get; protected set; }

        public int Channel { get; set; } = 1;

        public virtual double GraphicHeight
        {
            get { return height; }
            set
            {
                if (value >= 0)
                {
                    height = value;
                    Image.Height = value;
                    if (Controller.Active || !(AnimationIn.Style == GraphicAnimation.AnimationStyle.WipeUp))
                    {
                        BeginAnimation(HeightProperty, null);
                        Height = value;
                    }
                }
            }
        }

        public virtual double GraphicWidth
        {
            get { return width; }
            set
            {
                if (value >= 0)
                {
                    width = value;
                    Image.Width = value;
                    if (Controller.Active || !(AnimationIn.Style == GraphicAnimation.AnimationStyle.WipeLeft))
                    {
                        BeginAnimation(WidthProperty, null);
                        Width = value;
                    }
                }
            }
        }

        public virtual double X
        {
            get { return x; }
            set
            {
                // Apply to all child graphics
                double difference = value - x;
                foreach (GraphicController child in Controller.Children)
                {
                    child.Graphic.X += difference;
                }

                // Apply to this graphic
                x = value;
                if (Controller.Active || (!(AnimationIn.Style == GraphicAnimation.AnimationStyle.SlideLeft) && !(AnimationIn.Style == GraphicAnimation.AnimationStyle.SlideRight)))
                {
                    BeginAnimation(Canvas.LeftProperty, null);
                    Canvas.SetLeft(this, value);
                }
            }
        }

        public virtual double Y
        {
            get { return y; }
            set
            {
                // Apply to all child graphics
                double difference = value - y;
                foreach (GraphicController child in Controller.Children)
                {
                    child.Graphic.Y += difference;
                }

                // Apply to this graphic
                y = value;
                if (Controller.Active || (!(AnimationIn.Style == GraphicAnimation.AnimationStyle.SlideDown) && !(AnimationIn.Style == GraphicAnimation.AnimationStyle.SlideUp)))
                {
                    BeginAnimation(Canvas.TopProperty, null);
                    Canvas.SetTop(this, value);
                }
            }
        }

        public virtual void Show()
        {
            AnimationIn.Play();
            Controller.Active = true;
        }

        public virtual void Hide()
        {
            AnimationOut.Play();
            Controller.Active = false;
        }

        public void Remove()
        {
            Windows.Source.canvas.Children.Remove(this);
        }
    }
}
