using System.Windows;

namespace PetoGraphics
{
    /// <summary>
    /// Interaction logic for JSON.xaml
    /// </summary>
    public partial class JSON : Window
    {
        public JSON()
        {
            InitializeComponent();
            parser.SelectedIndex = 0;
        }

        private void Json_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
