using PetoGraphics.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    public partial class WebSourceUI : UserControl
    {
        public WebSourceUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is WebSource)
            {
                Visibility = Visibility.Visible;
                Validators.ClearAllErrors(grid);
                Windows.Main.WebSourceUI.url.Text = ((WebSource)selected.Graphic).Url;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void Url_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is WebSource)
            {
                Validators.Url(url, (text) =>
                {
                    ((WebSource)GraphicController.Selected.Graphic).Url = text;
                });
            }
        }
    }
}
