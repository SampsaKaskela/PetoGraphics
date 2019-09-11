using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    public partial class ScoreboardUI : UserControl
    {
        public ScoreboardUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is Scoreboard)
            {
                Visibility = Visibility.Visible;
                Scoreboard scoreboard = (Scoreboard)selected.Graphic;
                name1.Text = scoreboard.Texts[0].Content.ToString();
                name2.Text = scoreboard.Texts[1].Content.ToString();
                p1Score.Text = scoreboard.Texts[2].Content.ToString();
                p2Score.Text = scoreboard.Texts[3].Content.ToString();
                info.Text = scoreboard.Texts[4].Content.ToString();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void Name1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Scoreboard)
            {
                GraphicController.Selected.Graphic.Texts[0].Content = name1.Text;
            }
        }

        private void Name2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Scoreboard)
            {
                GraphicController.Selected.Graphic.Texts[1].Content = name2.Text;
            }
        }

        private void P1ScoreAdd_Click(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Scoreboard)
            {
                p1Score.Text = (int.Parse(p1Score.Text) + 1).ToString();
                GraphicController.Selected.Graphic.Texts[2].Content = p1Score.Text;
            }
        }

        private void P1ScoreSubstract_Click(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Scoreboard)
            {
                p1Score.Text = (int.Parse(p1Score.Text) - 1).ToString();
                GraphicController.Selected.Graphic.Texts[2].Content = p1Score.Text;
            }
        }

        private void P2ScoreAdd_Click(object sender, RoutedEventArgs e)
        {
            p2Score.Text = (int.Parse(p2Score.Text) + 1).ToString();
            GraphicController.Selected.Graphic.Texts[3].Content = p2Score.Text;
        }

        private void P2ScoreSubstract_Click(object sender, RoutedEventArgs e)
        {
            p2Score.Text = (int.Parse(p2Score.Text) - 1).ToString();
            GraphicController.Selected.Graphic.Texts[3].Content = p2Score.Text;
        }

        private void InfoText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Scoreboard)
            {
                GraphicController.Selected.Graphic.Texts[4].Content = info.Text;
            }
        }

        private void Swap_Click(object sender, RoutedEventArgs e)
        {
            string nametemp = name1.Text;
            name1.Text = name2.Text;
            name2.Text = nametemp;
            string scoretemp = p1Score.Text;
            p1Score.Text = p2Score.Text;
            p2Score.Text = scoretemp;
            GraphicController.Selected.Graphic.Texts[0].Content = name1.Text;
            GraphicController.Selected.Graphic.Texts[1].Content = name2.Text;
            GraphicController.Selected.Graphic.Texts[2].Content = p1Score.Text;
            GraphicController.Selected.Graphic.Texts[3].Content = p2Score.Text;
        }
    }
}
