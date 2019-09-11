using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Common
{
    public partial class NumericUpDown : UserControl
    {
        public NumericUpDown()
        {
            InitializeComponent();
        }

        public string Text
        {
            get
            {
                return textField.Text;
            }
            set
            {
                textField.Text = value;
            }
        }

        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            double number;
            if (double.TryParse(textField.Text, out number))
            {
                number++;
                textField.Text = number.ToString();
                maincontrol.RaiseEvent(new RoutedEventArgs(LostFocusEvent, maincontrol));
            }
        }

        private void Decrement_Click(object sender, RoutedEventArgs e)
        {
            double number;
            if (double.TryParse(textField.Text, out number))
            {
                number--;
                textField.Text = number.ToString();
                maincontrol.RaiseEvent(new RoutedEventArgs(LostFocusEvent, maincontrol));
            }
        }
    }
}
