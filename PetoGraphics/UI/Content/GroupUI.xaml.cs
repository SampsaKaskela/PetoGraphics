using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    public partial class GroupUI : UserControl
    {
        public GroupUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is Group)
            {
                Visibility = Visibility.Visible;
                Group group = (Group)selected.Graphic;
                title.Text = group.Texts[0].Content.ToString();
                n1.Text = group.Texts[1].Content.ToString();
                n2.Text = group.Texts[2].Content.ToString();
                n3.Text = group.Texts[3].Content.ToString();
                n4.Text = group.Texts[4].Content.ToString();
                n5.Text = group.Texts[5].Content.ToString();
                s1.Text = group.Texts[6].Content.ToString();
                s2.Text = group.Texts[7].Content.ToString();
                s3.Text = group.Texts[8].Content.ToString();
                s4.Text = group.Texts[9].Content.ToString();
                s5.Text = group.Texts[10].Content.ToString();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void GroupTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Group)
            {
                TextBox textbox = (TextBox)sender;
                if (textbox.Name[0] == 'n')
                {
                    ((Group)GraphicController.Selected.Graphic).Texts[int.Parse(textbox.Name[1].ToString())].Content = textbox.Text;
                }
                else if (textbox.Name[0] == 's')
                {
                    ((Group)GraphicController.Selected.Graphic).Texts[int.Parse(textbox.Name[1].ToString()) + 5].Content = textbox.Text;
                }
            }
        }

        private void Title_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Group)
            {
                GraphicController.Selected.Graphic.Texts[0].Content = title.Text;
            }
        }
    }
}
