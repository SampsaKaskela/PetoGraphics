using System.Windows;

namespace PetoGraphics
{
    /// <summary>
    /// Interaction logic for Savefolder.xaml
    /// </summary>
    public partial class SaveFolder : Window
    {
        public string name;
        public bool saveFolder = false;

        public SaveFolder()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (folderName.Text != string.Empty)
            {
                name = folderName.Text;
                saveFolder = true;
                this.Close();
            }
            else
            {
                CustomMessageBox.Show("Give folder name");
            }
        }
    }
}
