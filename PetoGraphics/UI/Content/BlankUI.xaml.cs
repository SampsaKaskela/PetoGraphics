using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    /// <summary>
    /// Interaction logic for GroupControllerUI.xaml
    /// </summary>
    public partial class BlankUI : UserControl
    {
        public BlankUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected is Blank)
            {
                Visibility = Visibility.Visible;
                controllerName.Text = selected.name.Content.ToString();
                Windows.Main.tabControl.SelectedIndex = 0;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void ControllerName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected is Blank)
            {
                GraphicController.Selected.name.Content = controllerName.Text.ToString();
            }
        }
    }
}
