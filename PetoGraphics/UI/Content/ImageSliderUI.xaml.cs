using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    /// <summary>
    /// Interaction logic for ImagesliderUI.xaml
    /// </summary>
    public partial class ImageSliderUI : UserControl
    {
        private OpenFileDialog fd = new OpenFileDialog();

        public ImageSliderUI()
        {
            InitializeComponent();
            fd.Multiselect = true;
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is ImageSlider)
            {
                Visibility = Visibility.Visible;
                ImageSlider imageSlider = (ImageSlider)selected.Graphic;
                sliderImageList.Items.Clear();
                for (int i = 0; i < imageSlider.ImageSources.Count; i++)
                {
                    sliderImageList.Items.Add(imageSlider.ImageSources[i]);
                }
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void AddToSlides_Click(object sender, RoutedEventArgs e)
        {
            if (fd.ShowDialog() ?? true && fd.FileNames.Length > 0)
            {
                foreach (string filename in fd.FileNames)
                {
                    ((ImageSlider)GraphicController.Selected.Graphic).ImageSources.Add(filename);
                    sliderImageList.Items.Add(filename);
                }
            }
        }

        private void DeleteFromSlides_Click(object sender, RoutedEventArgs e)
        {
            if (sliderImageList.SelectedIndex != -1)
            {
                ((ImageSlider)GraphicController.Selected.Graphic).ImageSources.RemoveAt(sliderImageList.SelectedIndex);
                sliderImageList.Items.RemoveAt(sliderImageList.SelectedIndex);
            }
        }
    }
}
