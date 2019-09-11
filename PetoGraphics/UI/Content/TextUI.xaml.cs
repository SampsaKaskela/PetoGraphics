using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    public partial class TextUI : UserControl
    {
        public TextUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && (selected.Graphic is SingleText || selected.Graphic is DoubleText))
            {
                Visibility = Visibility.Visible;
                if (selected.Graphic is SingleText)
                {
                    SingleText singleText = (SingleText)selected.Graphic;
                    lineOneText1.Text = singleText.TextStorage[0];
                    lineOneText2.Text = singleText.TextStorage[2];
                    lineOneText3.Text = singleText.TextStorage[4];
                    switch (singleText.SelectedIndex)
                    {
                        case 0:
                            {
                                radioButton1.IsChecked = true;
                                break;
                            }

                        case 1:
                            {
                                radioButton2.IsChecked = true;
                                break;
                            }

                        case 2:
                            {
                                radioButton3.IsChecked = true;
                                break;
                            }
                    }
                    lineTwoText1.Visibility = Visibility.Collapsed;
                    lineTwoText2.Visibility = Visibility.Collapsed;
                    lineTwoText3.Visibility = Visibility.Collapsed;
                }
                else
                {
                    DoubleText doubleText = (DoubleText)selected.Graphic;
                    lineOneText1.Text = doubleText.TextStorage[0];
                    lineOneText2.Text = doubleText.TextStorage[2];
                    lineOneText3.Text = doubleText.TextStorage[4];
                    lineTwoText1.Text = doubleText.TextStorage[1];
                    lineTwoText2.Text = doubleText.TextStorage[3];
                    lineTwoText3.Text = doubleText.TextStorage[5];
                    switch (doubleText.SelectedIndex)
                    {
                        case 0:
                            {
                                radioButton1.IsChecked = true;
                                break;
                            }

                        case 1:
                            {
                                radioButton2.IsChecked = true;
                                break;
                            }

                        case 2:
                            {
                                radioButton3.IsChecked = true;
                                break;
                            }
                    }
                    lineTwoText1.Visibility = Visibility.Visible;
                    lineTwoText2.Visibility = Visibility.Visible;
                    lineTwoText3.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void SingleText1_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateText1(lineOneText1, radioButton1, 0);
        }

        private void SingleText2_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateText1(lineOneText2, radioButton2, 2);
        }

        private void SingleText3_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateText1(lineOneText3, radioButton3, 4);
        }

        private void DoubleText1_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateText2(lineTwoText1, radioButton1, 1);
        }

        private void DoubleText2_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateText2(lineTwoText2, radioButton2, 3);
        }

        private void DoubleText3_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateText2(lineTwoText3, radioButton3, 5);
        }

        private void UpdateText1(TextBox box, RadioButton radiobutton, int index)
        {
            if (GraphicController.Selected != null)
            {
                if (GraphicController.Selected.Graphic is SingleText)
                {
                    if ((bool)radiobutton.IsChecked)
                    {
                        GraphicController.Selected.Graphic.Texts[0].Content = box.Text;
                    }
                    ((SingleText)GraphicController.Selected.Graphic).TextStorage[index] = box.Text;
                }
                else if (GraphicController.Selected.Graphic is DoubleText)
                {
                    if ((bool)radiobutton.IsChecked)
                    {
                        GraphicController.Selected.Graphic.Texts[0].Content = box.Text;
                    }
                    ((DoubleText)GraphicController.Selected.Graphic).TextStorage[index] = box.Text;
                }
            }
        }

        private void UpdateText2(TextBox box, RadioButton radiobutton, int index)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is DoubleText)
            {
                if ((bool)radiobutton.IsChecked)
                {
                    GraphicController.Selected.Graphic.Texts[1].Content = box.Text;
                }
                ((DoubleText)GraphicController.Selected.Graphic).TextStorage[index] = box.Text;
            }

        }

        private void RadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            LoadText(0, 0, 1);
        }

        private void RadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            LoadText(1, 2, 3);
        }

        private void RadioButton3_Checked(object sender, RoutedEventArgs e)
        {
            LoadText(2, 4, 5);
        }

        private void LoadText(int selectedIndex, int firstIndex, int secondIndex)
        {
            if (GraphicController.Selected != null)
            {
                if (GraphicController.Selected.Graphic is SingleText)
                {
                    ((SingleText)GraphicController.Selected.Graphic).SelectedIndex = selectedIndex;
                    GraphicController.Selected.Graphic.Texts[0].Content = ((SingleText)GraphicController.Selected.Graphic).TextStorage[firstIndex];
                }
                else if (GraphicController.Selected.Graphic is DoubleText)
                {
                    ((DoubleText)GraphicController.Selected.Graphic).SelectedIndex = selectedIndex;
                    GraphicController.Selected.Graphic.Texts[0].Content = ((DoubleText)GraphicController.Selected.Graphic).TextStorage[firstIndex];
                    GraphicController.Selected.Graphic.Texts[1].Content = ((DoubleText)GraphicController.Selected.Graphic).TextStorage[secondIndex];
                }
            }
        }
    }
}
