using PetoGraphics.Helpers;
using PetoGraphics.UI.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetoGraphics.UI.Settings
{
    /// <summary>
    /// Interaction logic for BracketUI.xaml
    /// </summary>
    public partial class BracketUI : UserControl
    {

        public BracketUI()
        {
            InitializeComponent();
            competitorSelect.Items.Add(4);
            competitorSelect.Items.Add(8);
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is Bracket)
            {
                Visibility = Visibility.Visible;
                Validators.ClearAllErrors(grid);
                Bracket bracket = (Bracket)selected.Graphic;
                competitorSelect.Text = bracket.Competitors.ToString();
                bracketX.Text = bracket.OriginX.ToString();
                bracketY.Text = bracket.OriginY.ToString();
                bracketWidth.Text = bracket.BracketWidth.ToString();
                bracketHeight.Text = bracket.BracketHeight.ToString();
                thickness.Text = bracket.Thickness.ToString();
                offsetX.Text = bracket.OffsetX.ToString();
                offsetY.Text = bracket.OffsetY.ToString();
                SolidColorBrush brush = bracket.Color as SolidColorBrush;
                red.Text = brush.Color.R.ToString();
                green.Text = brush.Color.G.ToString();
                blue.Text = brush.Color.B.ToString();
                if (bracket.HideHorizontal)
                {
                    hideHorizontal.IsChecked = true;
                }
                if (bracket.HideVertical)
                {
                    hideVertical.IsChecked = true;
                }
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void BracketX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Bracket)
            {
                Validators.Double((NumericUpDown)sender, (num) =>
                {
                    ((Bracket)GraphicController.Selected.Graphic).OriginX = num;
                    ((Bracket)GraphicController.Selected.Graphic).Redraw();
                });
            }
        }

        private void BracketY_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Bracket)
            {
                Validators.Double((NumericUpDown)sender, (num) =>
                {
                    ((Bracket)GraphicController.Selected.Graphic).OriginY = num;
                    ((Bracket)GraphicController.Selected.Graphic).Redraw();
                });
            }
        }

        private void BracketWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Bracket)
            {
                Validators.Double((NumericUpDown)sender, (num) =>
                {
                    ((Bracket)GraphicController.Selected.Graphic).BracketWidth = num;
                    ((Bracket)GraphicController.Selected.Graphic).Redraw();
                });
            }
        }

        private void BracketHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Bracket)
            {
                Validators.Double((NumericUpDown)sender, (num) =>
                {
                    ((Bracket)GraphicController.Selected.Graphic).BracketHeight = num;
                    ((Bracket)GraphicController.Selected.Graphic).Redraw();
                });
            }
        }

        private void CompetitorSelect_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Bracket)
            {
                ((Bracket)GraphicController.Selected.Graphic).Competitors = int.Parse(competitorSelect.SelectedItem.ToString());
                ((Bracket)GraphicController.Selected.Graphic).Redraw();
                if (((Bracket)GraphicController.Selected.Graphic).Competitors == 4)
                {
                    Windows.Main.BracketUI.N7.IsEnabled = false;
                    Windows.Main.BracketUI.N8.IsEnabled = false;
                    Windows.Main.BracketUI.N9.IsEnabled = false;
                    Windows.Main.BracketUI.N10.IsEnabled = false;
                    Windows.Main.BracketUI.N11.IsEnabled = false;
                    Windows.Main.BracketUI.N12.IsEnabled = false;
                    Windows.Main.BracketUI.N13.IsEnabled = false;
                    Windows.Main.BracketUI.N14.IsEnabled = false;
                    Windows.Main.BracketUI.S7.IsEnabled = false;
                    Windows.Main.BracketUI.S8.IsEnabled = false;
                    Windows.Main.BracketUI.S9.IsEnabled = false;
                    Windows.Main.BracketUI.S10.IsEnabled = false;
                    Windows.Main.BracketUI.S11.IsEnabled = false;
                    Windows.Main.BracketUI.S12.IsEnabled = false;
                    Windows.Main.BracketUI.S13.IsEnabled = false;
                    Windows.Main.BracketUI.S14.IsEnabled = false;
                }
                else
                {
                    Windows.Main.BracketUI.N7.IsEnabled = true;
                    Windows.Main.BracketUI.N8.IsEnabled = true;
                    Windows.Main.BracketUI.N9.IsEnabled = true;
                    Windows.Main.BracketUI.N10.IsEnabled = true;
                    Windows.Main.BracketUI.N11.IsEnabled = true;
                    Windows.Main.BracketUI.N12.IsEnabled = true;
                    Windows.Main.BracketUI.N13.IsEnabled = true;
                    Windows.Main.BracketUI.N14.IsEnabled = true;
                    Windows.Main.BracketUI.S7.IsEnabled = true;
                    Windows.Main.BracketUI.S8.IsEnabled = true;
                    Windows.Main.BracketUI.S9.IsEnabled = true;
                    Windows.Main.BracketUI.S10.IsEnabled = true;
                    Windows.Main.BracketUI.S11.IsEnabled = true;
                    Windows.Main.BracketUI.S12.IsEnabled = true;
                    Windows.Main.BracketUI.S13.IsEnabled = true;
                    Windows.Main.BracketUI.S14.IsEnabled = true;
                }
            }
        }

        private void BracketFont_Click(object sender, RoutedEventArgs e)
        {
            FontPicker picker = new FontPicker();
            picker.Text = ((Bracket)GraphicController.Selected.Graphic).ExampleLabel;
            picker.BracketRedraw = true;
            picker.ShowDialog();
        }

        private void HideHorizontal_Checked(object sender, RoutedEventArgs e)
        {
            ((Bracket)GraphicController.Selected.Graphic).HideHorizontal = true;
            ((Bracket)GraphicController.Selected.Graphic).Redraw();
        }

        private void HideVertical_Unchecked(object sender, RoutedEventArgs e)
        {
            ((Bracket)GraphicController.Selected.Graphic).HideVertical = false;
            ((Bracket)GraphicController.Selected.Graphic).Redraw();
        }

        private void HideVertical_Checked(object sender, RoutedEventArgs e)
        {
            ((Bracket)GraphicController.Selected.Graphic).HideVertical = true;
            ((Bracket)GraphicController.Selected.Graphic).Redraw();
        }

        private void HideHorizontal_Unchecked(object sender, RoutedEventArgs e)
        {
            ((Bracket)GraphicController.Selected.Graphic).HideHorizontal = false;
            ((Bracket)GraphicController.Selected.Graphic).Redraw();
        }

        private void Thickness_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Bracket)
            {
                Validators.Double((NumericUpDown)sender, (num) =>
                {
                    ((Bracket)GraphicController.Selected.Graphic).Thickness = num;
                    ((Bracket)GraphicController.Selected.Graphic).Redraw();
                });
            }
        }

        private void Color_LostFocus(object sender, RoutedEventArgs e)
        {
            Validators.Integer((NumericUpDown)sender, (redNum) =>
            {
                Validators.Integer((NumericUpDown)sender, (greenNum) =>
                {
                    Validators.Integer((NumericUpDown)sender, (blueNum) =>
                    {
                        ((Bracket)GraphicController.Selected.Graphic).Color = new SolidColorBrush(Color.FromRgb((byte)redNum, (byte)greenNum, (byte)blueNum));
                        ((Bracket)GraphicController.Selected.Graphic).Redraw();
                    });
                });
            });

        }

        private void OffsetX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Bracket)
            {
                Validators.Double((NumericUpDown)sender, (num) =>
                {
                    ((Bracket)GraphicController.Selected.Graphic).OriginX = num;
                    ((Bracket)GraphicController.Selected.Graphic).Redraw();
                });
            }
        }

        private void OffsetY_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Bracket)
            {
                Validators.Double((NumericUpDown)sender, (num) =>
                {
                    ((Bracket)GraphicController.Selected.Graphic).OffsetY = num;
                    ((Bracket)GraphicController.Selected.Graphic).Redraw();
                });
            }
        }
    }
}
