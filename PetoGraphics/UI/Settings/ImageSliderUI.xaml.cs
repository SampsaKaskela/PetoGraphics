using PetoGraphics.Helpers;
using PetoGraphics.UI.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Settings
{
    /// <summary>
    /// Interaction logic for SliderUI.xaml
    /// </summary>
    public partial class ImageSliderUI : UserControl
    {

        public ImageSliderUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is ImageSlider)
            {
                Visibility = Visibility.Visible;
                Validators.ClearAllErrors(grid);
                sliderDuration.Text = ((ImageSlider)selected.Graphic).Duration.ToString();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void SliderDuration_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is ImageSlider)
            {
                Validators.Double((NumericUpDown)sender, (num) =>
                {
                    ((ImageSlider)GraphicController.Selected.Graphic).Duration = num;
                });
            }
        }
    }
}
