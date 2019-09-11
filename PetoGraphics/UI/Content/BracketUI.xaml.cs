using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    public partial class BracketUI : UserControl
    {
        public BracketUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is Bracket)
            {
                BracketTitle.Text = ((Bracket)GraphicController.Selected.Graphic).Texts[0].Content;
                N1.Text = ((Bracket)GraphicController.Selected.Graphic).Names[0].Content.ToString();
                N2.Text = ((Bracket)GraphicController.Selected.Graphic).Names[1].Content.ToString();
                N3.Text = ((Bracket)GraphicController.Selected.Graphic).Names[2].Content.ToString();
                N4.Text = ((Bracket)GraphicController.Selected.Graphic).Names[3].Content.ToString();
                N5.Text = ((Bracket)GraphicController.Selected.Graphic).Names[4].Content.ToString();
                N6.Text = ((Bracket)GraphicController.Selected.Graphic).Names[5].Content.ToString();
                if (((Bracket)GraphicController.Selected.Graphic).Competitors >= 8)
                {
                    N7.Text = ((Bracket)GraphicController.Selected.Graphic).Names[6].Content.ToString();
                    N8.Text = ((Bracket)GraphicController.Selected.Graphic).Names[7].Content.ToString();
                    N9.Text = ((Bracket)GraphicController.Selected.Graphic).Names[8].Content.ToString();
                    N10.Text = ((Bracket)GraphicController.Selected.Graphic).Names[9].Content.ToString();
                    N11.Text = ((Bracket)GraphicController.Selected.Graphic).Names[10].Content.ToString();
                    N12.Text = ((Bracket)GraphicController.Selected.Graphic).Names[11].Content.ToString();
                    N13.Text = ((Bracket)GraphicController.Selected.Graphic).Names[12].Content.ToString();
                    N14.Text = ((Bracket)GraphicController.Selected.Graphic).Names[13].Content.ToString();
                }
                else
                {
                    N7.IsEnabled = false;
                    N8.IsEnabled = false;
                    N9.IsEnabled = false;
                    N10.IsEnabled = false;
                    N11.IsEnabled = false;
                    N12.IsEnabled = false;
                    N13.IsEnabled = false;
                    N14.IsEnabled = false;
                }
                S1.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[0].Content.ToString();
                S2.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[1].Content.ToString();
                S3.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[2].Content.ToString();
                S4.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[3].Content.ToString();
                S5.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[4].Content.ToString();
                S6.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[5].Content.ToString();
                if (((Bracket)GraphicController.Selected.Graphic).Competitors >= 8)
                {
                    S7.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[6].Content.ToString();
                    S8.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[7].Content.ToString();
                    S9.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[8].Content.ToString();
                    S10.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[9].Content.ToString();
                    S11.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[10].Content.ToString();
                    S12.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[11].Content.ToString();
                    S13.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[12].Content.ToString();
                    S14.Text = ((Bracket)GraphicController.Selected.Graphic).Scores[13].Content.ToString();
                }
                else
                {
                    S7.IsEnabled = false;
                    S8.IsEnabled = false;
                    S9.IsEnabled = false;
                    S10.IsEnabled = false;
                    S11.IsEnabled = false;
                    S12.IsEnabled = false;
                    S13.IsEnabled = false;
                    S14.IsEnabled = false;
                }
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void BracketTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Bracket)
            {
                TextBox textbox = (TextBox)sender;
                string textname = textbox.Name;
                if (textname[0] == 'N')
                {
                    string number = textname.Remove(0, 1);
                    ((Bracket)GraphicController.Selected.Graphic).Names[int.Parse(number) - 1].Content = textbox.Text;
                }
                else
                {
                    string number = textname.Remove(0, 1);
                    ((Bracket)GraphicController.Selected.Graphic).Scores[int.Parse(number) - 1].Content = textbox.Text;
                }
            }
        }

        private void BracketTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Bracket)
            {
                GraphicController.Selected.Graphic.Texts[0].Content = BracketTitle.Text;
            }
        }
    }
}
