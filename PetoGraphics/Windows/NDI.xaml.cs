using PetoGraphics.Helpers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace PetoGraphics
{
    /// <summary>
    /// Interaction logic for GeneralProperties.xaml
    /// </summary>
    public partial class NDIwindow : Window
    {
        private bool initialized = false;
        private bool sendingSource = false;
        private bool sendingAlpha = false;

        public NDIwindow()
        {
            InitializeComponent();
        }

        private void Ndi_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Windows.Ndi.Hide();
            e.Cancel = true;
        }

        private void SendSource_Click(object sender, RoutedEventArgs e)
        {
            if (!initialized)
            {
                initialized = Windows.Source.NDIsender.Init();
                Windows.Alpha.NDIsender.Init();
            }
            if (initialized)
            {
                if (!sendingSource)
                {
                    Validators.Required(NDI_name, (text) =>
                    {
                        Windows.Source.NDIsender.NdiWidth = Convert.ToInt32(Windows.Source.container.ActualWidth);
                        Windows.Source.NDIsender.NdiHeight = Convert.ToInt32(Windows.Source.container.ActualHeight);
                        Windows.Source.NDIsender.NdiFrameRateNumerator = int.Parse(Windows.OutputSettings.FPS.ToString()) * 1000;
                        Windows.Source.NDIsender.NdiName = text + " (Source)";
                        Windows.Source.NDIsender.Start();
                        NDI_name.IsEnabled = false;
                        sendingSource = true;
                        sendSource.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF51"));
                    });
                }
                else
                {
                    Windows.Source.NDIsender.Stop();
                    if (!sendingAlpha)
                    {
                        NDI_name.IsEnabled = true;
                    }
                    sendSource.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"), (Color)ColorConverter.ConvertFromString("#FF6B6B6B"), 90);
                    sendingSource = false;
                }
            }
        }

        private void SendAlpha_Click(object sender, RoutedEventArgs e)
        {
            if (!initialized)
            {
                initialized = Windows.Source.NDIsender.Init();
                Windows.Alpha.NDIsender.Init();
            }
            if (initialized)
            {
                if (!sendingAlpha)
                {
                    Validators.Required(NDI_name, (text) =>
                    {
                        Windows.Alpha.NDIsender.NdiWidth = Convert.ToInt32(Windows.Source.container.ActualWidth);
                        Windows.Alpha.NDIsender.NdiHeight = Convert.ToInt32(Windows.Source.container.ActualHeight);
                        Windows.Alpha.NDIsender.NdiFrameRateNumerator = Convert.ToInt32(Windows.OutputSettings.FPS.ToString()) * 1000;
                        Windows.Alpha.NDIsender.NdiName = text + " (Alpha)";
                        Windows.Alpha.NDIsender.Start();
                        NDI_name.IsEnabled = false;
                        sendingAlpha = true;
                        sendAlpha.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF51"));
                    });
                }
                else
                {
                    Windows.Alpha.NDIsender.Stop();
                    if (!sendingSource)
                    {
                        NDI_name.IsEnabled = true;
                    }
                    sendingAlpha = false;
                    sendAlpha.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"), (Color)ColorConverter.ConvertFromString("#FF6B6B6B"), 90);
                }
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
