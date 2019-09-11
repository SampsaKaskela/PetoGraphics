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

            Texts[0] = new GraphicText(container)
            {
                Name = "P1_Name",
                Content = "Player1",
                FontWeight = FontWeights.Bold,
                ContentAlign = HorizontalAlignment.Right,
                FontSize = 32,
                Width = 700,
                X = 0,
                Y = 10
            };

            Texts[1] = new GraphicText(container)
            {
                Name = "P2_Name",
                Content = "Player2",
                FontWeight = FontWeights.Bold,
                ContentAlign = HorizontalAlignment.Left,
                FontSize = 32,
                Width = 600,
                X = 1200,
                Y = 10
            };

            Texts[2] = new GraphicText(container)
            {
                Name = "P1_Score",
                Content = "0",
                FontWeight = FontWeights.Bold,
                ContentAlign = HorizontalAlignment.Center,
                FontSize = 40,
                Width = 75,
                X = 765,
                Y = 10
            };

            Texts[3] = new GraphicText(container)
            {
                Name = "P2_Score",
                Content = "0",
                FontWeight = FontWeights.Bold,
                ContentAlign = HorizontalAlignment.Center,
                FontSize = 40,
                Width = 75,
                X = 1070,
                Y = 10
            };

            Texts[4] = new GraphicText(container)
            {
                Name = "Info_text",
                Content = "Info text",
                FontWeight = FontWeights.Bold,
                ContentAlign = HorizontalAlignment.Center,
                FontSize = 32,
                Width = 1920,
                X = 0,
                Y = 10
            };
        }
    }
}
