using PetoGraphics.Helpers;
using PetoGraphics.UI.Common;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetoGraphics.UI.Settings
{
    /// <summary>
    /// Interaction logic for TextUI.xaml
    /// </summary>
    public partial class TextUI : UserControl
    {
        private List<GraphicText> textlist = new List<GraphicText>();
        private int labelIndex;
        private bool controlchanged;

        public TextUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic != null && selected.Graphic.Texts != null && selected.Graphic.Texts.Length > 0)
            {
                Visibility = Visibility.Visible;
                Validators.ClearAllErrors(grid);
                textlist.Clear();
                textlist = selected.Graphic.Texts.ToList();
                controlchanged = true;
                textselect.Items.Clear();
                for (int i = 0; i < textlist.Count; i++)
                {
                    textselect.Items.Add(textlist[i].Name);
                }
                controlchanged = false;
                textselect.SelectedIndex = 0;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void Text_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!controlchanged)
            {
                labelIndex = textlist.IndexOf(textlist.Find(x => x.Name == textselect.SelectedItem.ToString()));
                GraphicText text = textlist[labelIndex];
                textX.Text = text.X.ToString();
                textY.Text = text.Y.ToString();
                textwidth.Text = text.Width.ToString();
                if (text.Rich)
                {
                    lineHeightLabel.Visibility = Visibility.Visible;
                    lineHeight.Visibility = Visibility.Visible;
                    lineHeight.Text = text.LineHeight.ToString();
                }
                else
                {
                    lineHeightLabel.Visibility = Visibility.Collapsed;
                    lineHeight.Visibility = Visibility.Collapsed;
                }
                glowEnable.IsChecked = textlist[labelIndex].HasGlow;
                glowColor.Text = text.GlowColor;
                switch (textlist[labelIndex].ContentAlign)
                {
                    case HorizontalAlignment.Right:
                        {
                            alignRight.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                            break;
                        }

                    case HorizontalAlignment.Center:
                        {
                            alignCenter.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                            break;
                        }

                    case HorizontalAlignment.Left:
                        {
                            alignLeft.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                            break;
                        }
                }
            }
        }

        private void Font_Click(object sender, RoutedEventArgs e)
        {
            FontPicker picker = new FontPicker();
            picker.Text = textlist[labelIndex];
            picker.ShowDialog();
        }

        private void TextX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null && GraphicController.Selected.Graphic.Texts != null)
            {
                Validators.Double((NumericUpDown)sender, (num) =>
                {
                    textlist[labelIndex].X = num;
                });
            }
        }

        private void TextY_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null && GraphicController.Selected.Graphic.Texts != null)
            {
                Validators.Double((NumericUpDown)sender, (num) =>
                {
                    textlist[labelIndex].Y = num;
                });
            }
        }

        private void TextWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null && GraphicController.Selected.Graphic.Texts != null)
            {
                Validators.PositiveDouble((NumericUpDown)sender, (num) =>
                {
                    textlist[labelIndex].Width = num;
                });
            }
        }

        private void LineHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null && GraphicController.Selected.Graphic.Texts != null)
            {
                Validators.PositiveDouble((NumericUpDown)sender, (num) =>
                {
                    textlist[labelIndex].LineHeight = num;
                }, false);
            }
        }

        private void AlignRight_Click(object sender, RoutedEventArgs e)
        {
            textlist[labelIndex].ContentAlign = HorizontalAlignment.Right;
            alignRight.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFF3F3F3"), (Color)ColorConverter.ConvertFromString("#FF2AA2CF"), 90);
            alignCenter.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFF3F3F3"), (Color)ColorConverter.ConvertFromString("#FF6B6B6B"), 90);
            alignLeft.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFF3F3F3"), (Color)ColorConverter.ConvertFromString("#FF6B6B6B"), 90);
        }

        private void AlignCenter_Click(object sender, RoutedEventArgs e)
        {
            textlist[labelIndex].ContentAlign = HorizontalAlignment.Center;
            alignCenter.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFF3F3F3"), (Color)ColorConverter.ConvertFromString("#FF2AA2CF"), 90);
            alignLeft.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFF3F3F3"), (Color)ColorConverter.ConvertFromString("#FF6B6B6B"), 90);
            alignRight.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFF3F3F3"), (Color)ColorConverter.ConvertFromString("#FF6B6B6B"), 90);
        }

        private void AlignLeft_Click(object sender, RoutedEventArgs e)
        {
            textlist[labelIndex].ContentAlign = HorizontalAlignment.Left;
            alignLeft.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFF3F3F3"), (Color)ColorConverter.ConvertFromString("#FF2AA2CF"), 90);
            alignCenter.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFF3F3F3"), (Color)ColorConverter.ConvertFromString("#FF6B6B6B"), 90);
            alignRight.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFF3F3F3"), (Color)ColorConverter.ConvertFromString("#FF6B6B6B"), 90);
        }

        private void GlowColor_LostFocus(object sender, RoutedEventArgs e)
        {
            textlist[labelIndex].GlowColor = glowColor.Text;
        }

        private void Glow_Checked(object sender, RoutedEventArgs e)
        {
            glowColor.IsEnabled = true;
            textlist[labelIndex].HasGlow = true;
        }

        private void Glow_Unchecked(object sender, RoutedEventArgs e)
        {
            glowColor.IsEnabled = false;
            textlist[labelIndex].HasGlow = false;
        }
    }
}
