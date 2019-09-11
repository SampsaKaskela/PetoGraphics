using System.Windows.Controls;
using System.Windows;

namespace PetoGraphics
{
    public class DoubleText : Graphic
    {
        public DoubleText(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "DoubleText";

            Texts = new GraphicText[2];
            GraphicHeight = 150;
            GraphicWidth = 700;
            X = 0;
            Y = 900;

            Texts[0] = new GraphicText(container);
            Texts[0].Name = "Text1";
            Texts[0].Content = "Example";
            Texts[0].X = 50;
            Texts[0].Y = 15;
            Texts[0].FontSize = 60;
            Texts[0].FontWeight = FontWeights.Bold;
            Texts[0].Width = 600;

            Texts[1] = new GraphicText(container);
            Texts[1].Name = "Text2";
            Texts[1].Content = "Example";
            Texts[1].X = 50;
            Texts[1].Y = 75;
            Texts[1].FontSize = 60;
            Texts[1].FontWeight = FontWeights.Bold;
            Texts[1].Width = 600;
        }

        public int SelectedIndex { get; set; } = 0;

        public string[] TextStorage { get; set; } = new string[6];
    }
}
