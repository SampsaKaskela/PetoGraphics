using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PetoGraphics
{
    /// <summary>
    /// Interaction logic for Alpha.xaml
    /// </summary>
    public partial class Alpha : Window
    {
        public Alpha()
        {
            InitializeComponent();
            CompositionTarget.Rendering += DrawAlpha;
            grid.OpacityMask = new ImageBrush();
            ((ImageBrush)grid.OpacityMask).Stretch = Stretch.Fill;
        }

        private void DrawAlpha(object sender, EventArgs e)
        {
            if (Visibility == Visibility.Hidden)
            {
                return;
            }

            int width = Convert.ToInt32(Windows.Source.canvas.Width);
            int height = Convert.ToInt32(Windows.Source.canvas.Height);

            RenderTargetBitmap targetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            targetBitmap.Render(Windows.Source.canvas);
            ((ImageBrush)Windows.Alpha.grid.OpacityMask).ImageSource = targetBitmap;
        }

        private void Alpha_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Windows.Alpha.Hide();
            e.Cancel = true;
        }
    }
}
