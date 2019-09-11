using Microsoft.Win32;
using PetoGraphics.Helpers;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    public partial class MediaUI : UserControl
    {
        private OpenFileDialog fd = new OpenFileDialog();

        public MediaUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is Media)
            {
                Visibility = Visibility.Visible;
                Validators.ClearAllErrors(grid);
                Media media = (Media)selected.Graphic;
                fileName.Text = Path.GetFileName(media.Source);
                if (media.Loop)
                {
                    loop.IsChecked = true;
                }
                else
                {
                    loop.IsChecked = false;
                }
                volume.Text = media.Volume.ToString();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void MediaFile_Click(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Media && (fd.ShowDialog() ?? true))
            {
                ((Media)GraphicController.Selected.Graphic).Source = fd.FileName;
                fileName.Text = fd.FileName;
                mediaElement.Source = new Uri(fd.FileName);
                mediaElement.Play();
            }
        }

        private void MediaLoop_Checked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Media)
            {
                ((Media)GraphicController.Selected.Graphic).Loop = true;
            }
        }

        private void MediaLoop_Unchecked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Media)
            {
                ((Media)GraphicController.Selected.Graphic).Loop = false;
            }
        }

        private void MediaVolume_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Media)
            {
                Validators.PositiveInteger(volume, (num) =>
                {
                    ((Media)GraphicController.Selected.Graphic).Volume = num;
                });
            }
        }

        private void File_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Media)
            {
                TimeSpan duration = mediaElement.NaturalDuration.TimeSpan;
                ((Media)GraphicController.Selected.Graphic).Duration = duration;
                GraphicController.Selected.info.Content = string.Format("Duration: {0:00}:{1:00}", (duration.Hours * 60) + duration.Minutes, duration.Seconds);
                mediaElement.Stop();
                mediaElement.Source = null;
            }
        }
    }
}
