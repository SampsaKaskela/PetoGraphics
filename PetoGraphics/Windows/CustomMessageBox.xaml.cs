using System.Windows;

namespace PetoGraphics
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        private static CustomMessageBox messageBox;

        public CustomMessageBox()
        {
            InitializeComponent();
        }

        public static bool? Show(string message, string title = "", bool yesno = false)
        {
            messageBox = new CustomMessageBox();
            messageBox.Title = title;
            messageBox.text.Text = message;
            if (yesno)
            {
                messageBox.okButton.Visibility = Visibility.Collapsed;
                messageBox.yesnoButtons.Visibility = Visibility.Visible;
            }
            else
            {
                messageBox.okButton.Visibility = Visibility.Visible;
                messageBox.yesnoButtons.Visibility = Visibility.Collapsed;
            }
            return messageBox.ShowDialog();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
