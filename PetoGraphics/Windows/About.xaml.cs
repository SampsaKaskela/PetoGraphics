using System.Windows;

namespace PetoGraphics
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Windows.About.Hide();
            e.Cancel = true;
        }
    }
}
