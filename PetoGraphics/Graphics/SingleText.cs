using System.Windows.Controls;
using System.Windows;

namespace PetoGraphics
{
    public class SingleText : Graphic
    {
        public SingleText(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "SingleText";

            Texts = new GraphicText[1];
            GraphicHeight = 150;
            GraphicWidth = 600;
            X = 0;
            Y = 900;

            Texts[0] = new GraphicText(container)
            {
                Name = "Text",
                Content = "Example",
                X = 50,
                Y = 15,
                FontSize = 60,
                FontWeight = FontWeights.Bold,
                Width = 600
            };
        }

        public int SelectedIndex { get; set; } = 0;

        public string[] TextStorage { get; set; } = new string[6];
    }
}
