using System.Windows.Controls;
using System.Windows;

namespace PetoGraphics
{
    public class Group : Graphic
    {
        public Group(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "Groups";

            Texts = new GraphicText[11];
            GraphicHeight = 500;
            GraphicWidth = 800;
            X = 200;
            Y = 300;

            Texts[0] = new GraphicText(container)
            {
                Name = "Title",
                Content = "Title",
                X = 20,
                Y = 20,
                Width = 400,
                FontSize = 40
            };

            // Teams
            for (int i = 1; i < 6; i++)
            {
                Texts[i] = new GraphicText(container)
                {
                    Name = "Team" + i.ToString(),
                    Content = "Team",
                    X = 20,
                    Y = i * 75 + 30,
                    Width = 400,
                    FontSize = 40
                };
            }

            // Scores
            for (int i = 6; i < 11; i++)
            {
                Texts[i] = new GraphicText(container)
                {
                    Name = "Score" + (i - 5).ToString(),
                    Content = "0-0",
                    ContentAlign = HorizontalAlignment.Center,
                    X = 400,
                    Y = (i - 5) * 75 + 30,
                    Width = 75,
                    FontSize = 40
                };
            }
        }
    }
}
