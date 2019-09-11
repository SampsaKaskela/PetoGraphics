using Microsoft.Win32;
using PetoGraphics.Helpers;
using PetoGraphics.UI.Common;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PetoGraphics.UI.Settings
{
    /// <summary>
    /// Interaction logic for GeneralUI.xaml
    /// </summary>
    public partial class ImageSwitcherUI : UserControl
    {
        private System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();

        public ImageSwitcherUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is ImageSwitcher)
            {
                Visibility = Visibility.Visible;
                ImageSwitcher imageSwitcher = (ImageSwitcher)GraphicController.Selected.Graphic;
                folder.Text = imageSwitcher.Folder;
                if (imageSwitcher.SwitcherImage.Stretch == Stretch.Fill)
                {
                    fill.IsChecked = true;
                }
                else
                {
                    fit.IsChecked = true;
                }
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ((ImageSwitcher)GraphicController.Selected.Graphic).Folder = fd.SelectedPath;           
                folder.Text = System.IO.Path.GetDirectoryName(fd.SelectedPath);
                Windows.Main.ImageSwitcherUI.ComputeItems();
            }
        }

        private void ClearFolder_Click(object sender, RoutedEventArgs e)
        {
            ((ImageSwitcher)GraphicController.Selected.Graphic).Folder = string.Empty;
            folder.Text = string.Empty;
        }

        private void Fill_Checked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is ImageSwitcher)
            {
                ((ImageSwitcher)GraphicController.Selected.Graphic).SwitcherImage.Stretch = Stretch.Fill;
            }
        }

        private void Fit_Checked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is ImageSwitcher)
            {
                ((ImageSwitcher)GraphicController.Selected.Graphic).SwitcherImage.Stretch = Stretch.Uniform;
            }
        }
    }
}
