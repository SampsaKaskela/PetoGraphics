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

            Texts[0] = new GraphicText(container);
            Texts[0].Name = "Title";
            Texts[0].Content = "Title";
            Texts[0].X = 20;
            Texts[0].Y = 20;
            Texts[0].Width = 400;
            Texts[0].FontSize = 40;

            // Teams
            for (int i = 1; i < 6; i++)
            {
                Texts[i] = new GraphicText(container);
                Texts[i].Name = "Team" + i.ToString();
                Texts[i].Content = "Team";
                Texts[i].X = 20;
                Texts[i].Y = i * 75 + 30;
                Texts[i].Width = 400;
                Texts[i].FontSize = 40;
            }

            // Scores
            for (int i = 6; i < 11; i++)
            {
                Texts[i] = new GraphicText(container);
                Texts[i].Name = "Score" + (i - 5).ToString();
                Texts[i].Content = "0-0";
                Texts[i].ContentAlign = HorizontalAlignment.Center;
                Texts[i].X = 400;
                Texts[i].Y = (i - 5) * 75 + 30;
                Texts[i].Width = 75;
                Texts[i].FontSize = 40;
            }
        }
    }
}
