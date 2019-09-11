using PetoGraphics.Helpers;
using PetoGraphics.Providers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    /// <summary>
    /// Interaction logic for PeliliigaUI.xaml
    /// </summary>
    public partial class PeliliigaUI : UserControl
    {

        public PeliliigaUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected is Peliliiga)
            {
                Visibility = Visibility.Visible;
                Validators.ClearAllErrors(grid);
                Peliliiga peliliiga = (Peliliiga)selected;
                controllerName.Text = selected.name.Content.ToString();
                url.Text = peliliiga.Url;
                store.Text = peliliiga.Store;
                interval.Text = peliliiga.Interval.ToString();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void ControllerName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected is Peliliiga)
            {
                GraphicController.Selected.name.Content = controllerName.Text.ToString();
            }
        }

        private void Url_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected is Peliliiga)
            {
                Validators.Required(url, (text) =>
                {
                    ((Peliliiga)GraphicController.Selected).Url = text;
                });
            }
        }

        private void Store_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected is Peliliiga)
            {
                Validators.Required(store, (text) =>
                {
                    ((Peliliiga)GraphicController.Selected).Store = text;
                });
            }
        }

        private void Interval_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected is Peliliiga)
            {
                Validators.PositiveInteger(store, (num) =>
                {
                    ((Peliliiga)GraphicController.Selected).Interval = num;
                });
            }
        }
    }
}
