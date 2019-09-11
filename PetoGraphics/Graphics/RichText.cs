using System.Windows.Controls;
using System.Windows;

namespace PetoGraphics
{
    public class RichText : Graphic
    {
        public RichText(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "RichText";

            Texts = new GraphicText[1];
            GraphicWidth = 500;
            GraphicHeight = 600;
            X = 200;
            Y = 200;

            Texts[0] = new GraphicText(container)
            {
                Rich = true,
                Name = "Text",
                Content = "Example\nExample\nExample\nExample\nExample",
                X = 32,
                Y = 10,
                FontSize = 40,
                FontWeight = FontWeights.Bold,
                Width = 450,
                LineHeight = 50
            };
        }
    }
}
