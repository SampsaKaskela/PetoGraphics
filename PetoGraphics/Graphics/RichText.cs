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

            Texts[0] = new GraphicText(container);
            Texts[0].Rich = true;
            Texts[0].Name = "Text";
            Texts[0].Content = "Example\nExample\nExample\nExample\nExample";
            Texts[0].X = 32;
            Texts[0].Y = 10;
            Texts[0].FontSize = 40;
            Texts[0].FontWeight = FontWeights.Bold;
            Texts[0].Width = 450;
            Texts[0].LineHeight = 50;
        }
    }
}
