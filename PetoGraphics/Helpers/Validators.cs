using PetoGraphics.UI.Common;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetoGraphics.Helpers
{
    class Validators
    {
        public static bool Double(TextBox textbox, Action<double> callback)
        {
            if (textbox.Text == string.Empty)
            {
                textbox.Text = "0";
            }
            if (double.TryParse(textbox.Text, out double num))
            {
                ClearError(textbox);
                callback(num);
                return true;
            }
            else
            {
                SetError(textbox);
                CustomMessageBox.Show("Must be a number.", "Error");
                return false;
            }
        }

        public static bool Double(NumericUpDown textBox, Action<double> callback)
        {
            if (textBox.Text == string.Empty)
            {
                textBox.Text = "0";
            }
            if (double.TryParse(textBox.Text, out double num))
            {
                ClearError(textBox);
                callback(num);
                return true;
            }
            else
            {
                SetError(textBox);
                CustomMessageBox.Show("Must be a number.", "Error");
                return false;
            }
        }

        public static bool PositiveDouble(TextBox textbox, Action<double> callback, bool includeZero = true)
        {
            if (textbox.Text == string.Empty)
            {
                textbox.Text = "0";
            }
            if (double.TryParse(textbox.Text, out double num) && ((num >= 0 && includeZero) || (num > 0 && !includeZero)))
            {
                ClearError(textbox);
                callback(num);
                return true;
            }
            else
            {
                SetError(textbox);
                CustomMessageBox.Show("Must be a positive number.", "Error");
                return false;
            }
        }

        public static bool PositiveDouble(NumericUpDown textBox, Action<double> callback, bool includeZero = true)
        {
            if (textBox.Text == string.Empty)
            {
                textBox.Text = "0";
            }
            if (double.TryParse(textBox.Text, out double num) && ((num >= 0 && includeZero) || (num > 0 && !includeZero)))
            {
                ClearError(textBox);
                callback(num);
                return true;
            }
            else
            {
                SetError(textBox);
                CustomMessageBox.Show("Must be a positive number.", "Error");
                return false;
            }
        }

        public static bool Integer(TextBox textBox, Action<int> callback)
        {
            if (textBox.Text == string.Empty)
            {
                textBox.Text = "0";
            }
            if (int.TryParse(textBox.Text, out int num))
            {
                ClearError(textBox);
                callback(num);
                return true;
            }
            else
            {
                SetError(textBox);
                CustomMessageBox.Show("Must be an integer.", "Error");
                return false;
            }
        }

        public static bool Integer(NumericUpDown textBox, Action<int> callback)
        {
            if (textBox.Text == string.Empty)
            {
                textBox.Text = "0";
            }
            if (int.TryParse(textBox.Text, out int num))
            {
                ClearError(textBox);
                callback(num);
                return true;
            }
            else
            {
                SetError(textBox);
                CustomMessageBox.Show("Must be an integer.", "Error");
                return false;
            }
        }

        public static bool PositiveInteger(TextBox textBox, Action<int> callback, bool includeZero = true)
        {
            if (textBox.Text == string.Empty)
            {
                textBox.Text = "0";
            }
            if (int.TryParse(textBox.Text, out int num) && ((num >= 0 && includeZero) || (num > 0 && !includeZero)))
            {
                ClearError(textBox);
                callback(num);
                return true;
            }
            else
            {
                SetError(textBox);
                CustomMessageBox.Show("Must be an positive integer.", "Error");
                return false;
            }
        }

        public static bool PositiveInteger(NumericUpDown textBox, Action<int> callback, bool includeZero = true)
        {
            if (textBox.Text == string.Empty)
            {
                textBox.Text = "0";
            }
            if (int.TryParse(textBox.Text, out int num) && ((num >= 0 && includeZero) || (num > 0 && !includeZero)))
            {
                ClearError(textBox);
                callback(num);
                return true;
            }
            else
            {
                SetError(textBox);
                CustomMessageBox.Show("Must be an positive integer.", "Error");
                return false;
            }
        }

        public static bool Url(TextBox textBox, Action<string> callback)
        {
            if (Uri.IsWellFormedUriString(textBox.Text, UriKind.Absolute))
            {
                ClearError(textBox);
                callback(textBox.Text);
                return true;
            }
            else
            {
                SetError(textBox);
                CustomMessageBox.Show("Not valid url.", "Error");
                return false;
            }
        }

        public static bool Required(TextBox textBox, Action<string> callback)
        {
            if (textBox.Text.Length > 0)
            {
                ClearError(textBox);
                callback(textBox.Text);
                return true;
            }
            else
            {
                SetError(textBox);
                CustomMessageBox.Show("Can't be empty.", "Error");
                return false;
            }
        }

        public static void ClearError(TextBox textBox)
        {
            textBox.ClearValue(Control.BorderBrushProperty);
        }

        public static void ClearError(NumericUpDown upDown)
        {
            upDown.textField.ClearValue(Control.BorderBrushProperty);
        }

        public static void ClearAllErrors(Grid grid)
        {
            TextBox[] textBoxes = grid.Children.OfType<TextBox>().ToArray();
            foreach (TextBox child in textBoxes)
            {
                ClearError(child);
            }
            NumericUpDown[] upDowns = grid.Children.OfType<NumericUpDown>().ToArray();
            foreach (NumericUpDown child in upDowns)
            {
                ClearError(child);
            }
        }

        public static void SetError(TextBox textBox)
        {
            textBox.BorderBrush = Brushes.Red;
        }

        public static void SetError(NumericUpDown upDown)
        {
            upDown.textField.BorderBrush = Brushes.Red;
        }
    }
}
