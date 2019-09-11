using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;
using System.Collections.Generic;
using PetoGraphics.Helpers;

namespace PetoGraphics
{
    public partial class OutputSettings : Window
    {
        public OutputSettings()
        {
            InitializeComponent();
        }

        public int SourceMonitor { get; set; } = 0;

        public int AlphaMonitor { get; set; } = 0;

        public double FPS { get; set; } = 60;

        public void SetWidth(double width)
        {
            if (width > 1920)
            {
                width = 1920;
                widthSetting.Text = "1920";
            }
            Windows.Source.container.Width = width;
            Windows.Alpha.container.Width = width;
            widthSetting.Text = width.ToString();
        }

        public void SetHeight(double height)
        {
            if (height > 1080)
            {
                height = 1080;
                heightSetting.Text = "1080";
            }
            Windows.Source.container.Height = height;
            Windows.Alpha.container.Height = height;
            heightSetting.Text = height.ToString();
        }

        public void SetFPS(double fps, List<GraphicController> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i].Graphic is Media) && list[i].Graphic != null)
                {
                    list[i].Graphic.Image.FPS = fps;
                }
                SetFPS(fps, list[i].Children);
            }
        }

        private void OutputSettings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Windows.OutputSettings.Hide();
            e.Cancel = true;
        }

        private void Color_Changed(object sender, EventArgs e)
        {
            switch (bgColor.SelectedIndex)
            {
                case 0:
                    bgColor.Text = "None";
                    Windows.Source.canvas.Background = Brushes.Black;
                    break;

                case 1:
                    bgColor.Text = "Red";
                    Windows.Source.canvas.Background = Brushes.Red;
                    break;

                case 2:
                    bgColor.Text = "Green";
                    Windows.Source.canvas.Background = Brushes.Lime;
                    break;

                case 3:
                    bgColor.Text = "Blue";
                    Windows.Source.canvas.Background = Brushes.Blue;
                    break;
            }
        }

        private void Width_LostFocus(object sender, RoutedEventArgs e)
        {
            Validators.PositiveInteger(widthSetting, (num) =>
            {
                SetWidth(num);
            });
        }
        private void Height_LostFocus(object sender, RoutedEventArgs e)
        {
            Validators.PositiveInteger(heightSetting, (num) =>
            {
                SetHeight(num);
            });
        }
        private void Fullscreen_Checked(object sender, RoutedEventArgs e)
        {
            widthSetting.IsEnabled = false;
            heightSetting.IsEnabled = false;
            Windows.Source.WindowState = WindowState.Maximized;
            Windows.Source.WindowStyle = WindowStyle.None;
            Windows.Alpha.WindowState = WindowState.Maximized;
            Windows.Alpha.WindowStyle = WindowStyle.None;
        }

        private void Fullscreen_Unchecked(object sender, RoutedEventArgs e)
        {
            widthSetting.IsEnabled = true;
            heightSetting.IsEnabled = true;
            Windows.Source.WindowState = WindowState.Normal;
            Windows.Source.WindowStyle = WindowStyle.SingleBorderWindow;
            Windows.Alpha.WindowState = WindowState.Normal;
            Windows.Alpha.WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private void FPS_LostFocus(object sender, RoutedEventArgs e)
        {
            Validators.PositiveDouble(FPSsetting, (num) =>
            {
                FPS = num;
                SetFPS(num, GraphicController.rootList);
            });
        }

        private void SourceMonitorSelect_Changed(object sender, EventArgs e)
        {
            SourceMonitor = sourceMonitorSelect.SelectedIndex;
            if (SourceMonitor >= Screen.AllScreens.Length)
            {
                SourceMonitor = 0;
            }
            Screen s1 = Screen.AllScreens[SourceMonitor];
            System.Drawing.Rectangle r1 = s1.WorkingArea;
            Windows.Source.Left = r1.Right - Windows.Source.Width;
            Windows.Source.Top = r1.Top;
        }

        private void AlphaMonitorSelect_Changed(object sender, EventArgs e)
        {
            AlphaMonitor = alphaMonitorSelect.SelectedIndex;
            if (AlphaMonitor >= Screen.AllScreens.Length)
            {
                AlphaMonitor = 0;
            }
            Screen s2 = Screen.AllScreens[AlphaMonitor];
            System.Drawing.Rectangle r2 = s2.WorkingArea;
            Windows.Alpha.Left = r2.Right - Windows.Source.Width;
            Windows.Alpha.Top = r2.Top;
        }
    }
}
