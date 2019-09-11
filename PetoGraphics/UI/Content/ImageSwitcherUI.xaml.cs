using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    /// <summary>
    /// Interaction logic for GroupControllerUI.xaml
    /// </summary>
    public partial class ImageSwitcherUI : UserControl
    {
        public ImageSwitcherUI()
        {
            InitializeComponent();
            imageSelector.DisplayMemberPath = "Key";
            imageSelector.SelectedValuePath = "Value";
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is ImageSwitcher)
            {
                Visibility = Visibility.Visible;
                ComputeItems();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        public void ComputeItems()
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is ImageSwitcher)
            {
                imageSelector.Items.Clear();
                imageSelector.Items.Add(new KeyValuePair<string, string>("None", string.Empty));
                ImageSwitcher imageSwitcher = (ImageSwitcher)GraphicController.Selected.Graphic;
                foreach (string image in imageSwitcher.Images)
                {
                    imageSelector.Items.Add(new KeyValuePair<string, string>(Path.GetFileName(image), image));
                }
                imageSelector.SelectedIndex = Array.FindIndex(imageSwitcher.Images, x => x == imageSwitcher.ActiveImage) + 1;
            }
        }

        private void ImageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is ImageSwitcher && imageSelector.Items.Count > 0 && imageSelector.SelectedIndex != -1)
            {
                if ((string)imageSelector.SelectedValue != string.Empty)
                {
                    ((ImageSwitcher)GraphicController.Selected.Graphic).ActiveImage = (string)imageSelector.SelectedValue;
                }
                else
                {
                    ((ImageSwitcher)GraphicController.Selected.Graphic).SwitcherImage.Clear();
                }
            }
        }
    }
}
