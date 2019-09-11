using System.Windows.Controls;
using System.Windows;

namespace PetoGraphics
{
    public class Scoreboard : Graphic
    {
        public Scoreboard(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "Scoreboard";

            Texts = new GraphicText[5];
            GraphicHeight = 75;
            GraphicWidth = 1920;
            X = 0;
            Y = 0;

            Texts[0] = new GraphicText(container);
            Texts[0].Name = "P1_Name";
            Texts[0].Content = "Player1";
            Texts[0].FontWeight = FontWeights.Bold;
            Texts[0].ContentAlign = HorizontalAlignment.Right;
            Texts[0].FontSize = 32;
            Texts[0].Width = 700;
            Texts[0].X = 0;
            Texts[0].Y = 10;

            Texts[1] = new GraphicText(container);
            Texts[1].Name = "P2_Name";
            Texts[1].Content = "Player2";
            Texts[1].FontWeight = FontWeights.Bold;
            Texts[1].ContentAlign = HorizontalAlignment.Left;
            Texts[1].FontSize = 32;
            Texts[1].Width = 600;
            Texts[1].X = 1200;
            Texts[1].Y = 10;

            Texts[2] = new GraphicText(container);
            Texts[2].Name = "P1_Score";
            Texts[2].Content = "0";
            Texts[2].FontWeight = FontWeights.Bold;
            Texts[2].ContentAlign = HorizontalAlignment.Center;
            Texts[2].FontSize = 40;
            Texts[2].Width = 75;
            Texts[2].X = 765;
            Texts[2].Y = 10;

            Texts[3] = new GraphicText(container);
            Texts[3].Name = "P2_Score";
            Texts[3].Content = "0";
            Texts[3].FontWeight = FontWeights.Bold;
            Texts[3].ContentAlign = HorizontalAlignment.Center;
            Texts[3].FontSize = 40;
            Texts[3].Width = 75;
            Texts[3].X = 1070;
            Texts[3].Y = 10;

            Texts[4] = new GraphicText(container);
            Texts[4].Name = "Info_text";
            Texts[4].Content = "Info text";
            Texts[4].FontWeight = FontWeights.Bold;
            Texts[4].ContentAlign = HorizontalAlignment.Center;
            Texts[4].FontSize = 32;
            Texts[4].Width = 1920;
            Texts[4].X = 0;
            Texts[4].Y = 10;
        }
    }
}
