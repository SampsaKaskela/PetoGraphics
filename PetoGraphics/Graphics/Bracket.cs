using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;

namespace PetoGraphics
{
    public class Bracket : Graphic
    {
        private double currentHeight = 0;
        private int index = 0;
        private List<Line> lines = new List<Line>();

        public Bracket(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "Bracket";

            Texts = new GraphicText[1];
            GraphicWidth = 1920;
            GraphicHeight = 1080;
            X = 0;
            Y = 0;

            Texts[0] = new GraphicText(container);
            Texts[0].Name = "Title";
            Texts[0].Content = "Title";
            Texts[0].X = 270;
            Texts[0].Y = 90;
            Texts[0].FontWeight = FontWeights.Bold;
            Texts[0].FontSize = 40;
            Texts[0].Width = 780;

            for (int i = 0; i < Names.Length; i++)
            {
                Names[i] = new GraphicText(container);
                Names[i].Content = "TBD";
                Scores[i] = new GraphicText(container);
                Scores[i].Content = "0";
            }

            Color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7A0606"));
            ExampleLabel = new GraphicText(container);
            ExampleLabel.Name = "examplelabel";
            ExampleLabel.FontSize = 40;
        }

        public double BracketHeight { get; set; } = 340;

        public double BracketWidth { get; set; } = 500;

        public Brush Color { get; set; }

        public int Competitors { get; set; } = 8;

        public bool HideHorizontal { get; set; } = false;

        public bool HideVertical { get; set; } = false;

        public GraphicText ExampleLabel { get; set; }

        public GraphicText[] Names { get; set; } = new GraphicText[30];

        public double OffsetX { get; set; } = 0;

        public double OffsetY { get; set; } = 0;

        public double OriginX { get; set; } = 1770;

        public double OriginY { get; set; } = 600;

        public double ScoreOffset { get; set; } = -50;

        public GraphicText[] Scores { get; set; } = new GraphicText[30];

        public double Thickness { get; set; } = 4;

        public void Create(double startX, double startY)
        {
            index = 0;
            currentHeight = BracketHeight;
            BracketPointPair one = AddPair(LenghtLine(new BracketPoint(startX, startY)));
            if (Competitors >= 8)
            {
                currentHeight *= (double)1 / 2;
                BracketPointPair two = AddPair(one.point1);
                BracketPointPair three = AddPair(one.point2);
                if (Competitors == 16)
                {
                    currentHeight *= (double)1 / 2;
                    BracketPointPair fourth = AddPair(two.point1);
                    BracketPointPair fifth = AddPair(two.point2);
                    BracketPointPair six = AddPair(three.point1);
                    BracketPointPair seven = AddPair(three.point2);
                }
            }
        }

        public void Redraw()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                container.Children.Remove(lines[i]);
            }
            lines.Clear();
            for (int i = 0; i < Names.Length; i++)
            {
                Names[i].Visibility = Visibility.Collapsed;
                Scores[i].Visibility = Visibility.Collapsed;
            }
            Windows.Source.UpdateLayout();
            Create(OriginX, OriginY);
        }

        private BracketPointPair AddPair(BracketPoint startpoint)
        {
            BracketPointPair newPoints = WidthLine(startpoint);
            BracketPointPair pair = new BracketPointPair(LenghtLine(newPoints.point1), LenghtLine(newPoints.point2));
            return pair;
        }

        private BracketPoint LenghtLine(BracketPoint point)
        {
            NameLabel(index, point, true);
            ScoreLabel(index, point, true);
            index++;

            NameLabel(index, point, false);
            ScoreLabel(index, point, false);
            index++;

            Line line = new Line();
            line.X1 = point.X;
            line.X2 = point.X - BracketWidth;
            line.Y1 = point.Y;
            line.Y2 = point.Y;
            line.StrokeThickness = Thickness;
            line.Stroke = Color;
            lines.Add(line);

            if (!HideHorizontal)
            {
                container.Children.Add(line);
            }
            BracketPoint newpoint = new BracketPoint(line.X2, line.Y2);
            return newpoint;
        }

        private void NameLabel(int index, BracketPoint point, bool up)
        {
            Names[index].FontColor = ExampleLabel.FontColor;
            Names[index].FontSize = ExampleLabel.FontSize;
            Names[index].FontFamily = ExampleLabel.FontFamily;
            Names[index].FontWeight = ExampleLabel.FontWeight;
            Names[index].FontStyle = ExampleLabel.FontStyle;
            Names[index].Width = BracketWidth;
            if (up)
            {
                Names[index].X = point.X - BracketWidth + OffsetX;
                Names[index].Y = point.Y - ExampleLabel.FontSize * 1.5 - OffsetY;
            }
            else
            {
                Names[index].X = point.X - BracketWidth + OffsetX;
                Names[index].Y = point.Y + OffsetY;
            }
            Names[index].Visibility = Visibility.Visible;
        }

        private void ScoreLabel(int index, BracketPoint point, bool up)
        {
            Scores[index].FontColor = ExampleLabel.FontColor;
            Scores[index].FontSize = ExampleLabel.FontSize;
            Scores[index].FontFamily = ExampleLabel.FontFamily;
            Scores[index].FontWeight = ExampleLabel.FontWeight;
            Scores[index].FontStyle = ExampleLabel.FontStyle;

            if (up)
            {
                Scores[index].X = point.X + OffsetX + ScoreOffset;
                Scores[index].Y = point.Y - ExampleLabel.FontSize * 1.5 - OffsetY;
            }
            else
            {
                Scores[index].X = point.X + OffsetX + ScoreOffset;
                Scores[index].Y = point.Y + OffsetY;
            }
            Scores[index].Visibility = Visibility.Visible;
        }

        private BracketPointPair WidthLine(BracketPoint startPoint)
        {
            Line line = new Line();
            line.X1 = startPoint.X;
            line.X2 = startPoint.X;
            line.Y1 = startPoint.Y + currentHeight / 2;
            line.Y2 = line.Y1 - currentHeight;
            line.StrokeThickness = Thickness;
            line.Stroke = Color;
            lines.Add(line);

            if (!HideVertical)
            {
                container.Children.Add(line);
            }
            BracketPointPair pair = new BracketPointPair(new BracketPoint(line.X1, line.Y1), new BracketPoint(line.X2, line.Y2));
            return pair;
        }

        public struct BracketPoint
        {
            public double X;
            public double Y;

            public BracketPoint(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        public struct BracketPointPair
        {
            public BracketPoint point1;
            public BracketPoint point2;

            public BracketPointPair(BracketPoint point1, BracketPoint point2)
            {
                this.point1 = point1;
                this.point2 = point2;
            }
        }
    }
}
