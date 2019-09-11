using System.Windows.Controls;

namespace PetoGraphics
{
    public class ImageOnly : Graphic
    {
        public ImageOnly(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "Image";

            GraphicWidth = 1920;
            GraphicHeight = 1080;
            X = 0;
            Y = 0;
        }
    }
}
