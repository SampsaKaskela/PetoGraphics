using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetoGraphics
{
    /// <summary>
    /// Interaction logic for GraphicEditor.xaml
    /// </summary>
    public partial class GraphicEditor : UserControl
    {
        private OpenFileDialog fd = new OpenFileDialog();

        public GraphicEditor()
        {
            InitializeComponent();
        }

        public Graphic Target { get; set; }

        private void Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            Target.X = 0;
            Target.Y = 0;
            Target.GraphicWidth = Windows.Source.canvas.Width;
            Target.GraphicHeight = Windows.Source.canvas.Height;
        }

        private void Image_Click(object sender, RoutedEventArgs e)
        {
            if (fd.ShowDialog() ?? true)
            {
                GraphicController.Selected.Graphic.Image.UriSource = fd.FileName;
            }
        }

        private void Center_Click(object sender, RoutedEventArgs e)
        {
            CenterHorizontal_Click(sender, e);
            CenterVertical_Click(sender, e);
        }

        private void CenterHorizontal_Click(object sender, RoutedEventArgs e)
        {
            double difference = Windows.Source.container.Width - Target.Width;
            Target.X = difference / 2;
        }

        private void CenterVertical_Click(object sender, RoutedEventArgs e)
        {
            double difference = Windows.Source.container.Height - Target.Height;
            Target.Y = difference / 2;
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            Target.X = 0;
        }

        private void Top_Click(object sender, RoutedEventArgs e)
        {
            Target.Y = 0;
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            Target.X = Windows.Source.container.Width - Target.GraphicWidth;
        }

        private void Bottom_Click(object sender, RoutedEventArgs e)
        {
            Target.Y = Windows.Source.container.Height - Target.GraphicHeight;
        }

        private void MoveThumb_Delta(object sender, DragDeltaEventArgs e)
        {
            Target.X += e.HorizontalChange;
            Target.Y += e.VerticalChange;
        }

        private void BottomLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Target.X += e.HorizontalChange;
            Target.GraphicWidth -= e.HorizontalChange;
            Target.GraphicHeight += e.VerticalChange;
        }

        private void BottomRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Target.GraphicWidth += e.HorizontalChange;
            Target.GraphicHeight += e.VerticalChange;
        }

        private void TopLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Target.X += e.HorizontalChange;
            Target.Y += e.VerticalChange;
            Target.GraphicWidth -= e.HorizontalChange;
            Target.GraphicHeight -= e.VerticalChange;
        }

        private void TopRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Target.Y += e.VerticalChange;
            Target.GraphicWidth += e.HorizontalChange;
            Target.GraphicHeight -= e.VerticalChange;
        }
    }
}
