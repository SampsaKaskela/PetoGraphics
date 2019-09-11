using System.Windows;

namespace PetoGraphics
{
    /// <summary>
    /// Interaction logic for Savefolder.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {

        public ProgressWindow()
        {
            InitializeComponent();
        }

        private void ProggressWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
