using PetoGraphics.Helpers;
using PetoGraphics.Providers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    /// <summary>
    /// Interaction logic for XerberusUI.xaml
    /// </summary>
    public partial class XerberusUI : UserControl
    {

        public XerberusUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected is Xerberus)
            {
                Visibility = Visibility.Visible;
                Validators.ClearAllErrors(grid);
                Xerberus xerberus = (Xerberus)selected;
                controllerName.Text = selected.name.Content.ToString();
                url.Text = xerberus.Url;
                clientId.Text = Xerberus.ClientId;
                clientSecret.Text = Xerberus.ClientSecret;
                store.Text = xerberus.Store;
                interval.Text = xerberus.Interval.ToString();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void ControllerName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected is Xerberus)
            {
                GraphicController.Selected.name.Content = controllerName.Text.ToString();
            }
        }

        private void Url_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected is Xerberus)
            {
                Validators.Url(url, (text) =>
                {
                    ((Xerberus)GraphicController.Selected).Url = url.Text;
                });
            }
        }

        private void ClientId_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected is Xerberus)
            {
                Validators.Required(clientId, (text) =>
                {
                    Xerberus.ClientId = text;
                });
            }
        }

        private void ClientSecret_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected is Xerberus)
            {
                Validators.Required(clientSecret, (text) =>
                {
                    Xerberus.ClientSecret = text;
                });
            }
        }

        private void Store_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected is Xerberus)
            {
                Validators.Required(store, (text) =>
                {
                    ((Xerberus)GraphicController.Selected).Store = text;
                });
            }
        }

        private void Interval_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected is Xerberus)
            {
                Validators.PositiveInteger(interval, (num) =>
                {
                    ((Xerberus)GraphicController.Selected).Interval = num;
                });
            }
        }
    }
}
