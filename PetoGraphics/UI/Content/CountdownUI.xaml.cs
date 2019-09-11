using PetoGraphics.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    public partial class CountdownUI : UserControl
    {

        public CountdownUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is Countdown)
            {
                Visibility = Visibility.Visible;
                Validators.ClearAllErrors(grid);
                Countdown countdown = (Countdown)selected.Graphic;
                displayFormat.Text = countdown.Format;
                remainingHours.Text = countdown.RemainingHours.ToString("00");
                remainingMinutes.Text = countdown.RemainingMin.ToString("00");
                remainingSeconds.Text = countdown.RemainingSec.ToString("00");
                startHours.Text = countdown.StartHour.ToString("00");
                startMinutes.Text = countdown.StartMin.ToString("00");
                startSeconds.Text = countdown.StartSec.ToString("00");
                if (countdown.StartAt)
                {
                    Windows.Main.CountdownUI.startTimer.IsChecked = true;
                }
                else
                {
                    Windows.Main.CountdownUI.remainingTimer.IsChecked = true;
                }
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void RemainingHours_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Countdown)
            {
                Validators.PositiveInteger(remainingHours, (num) =>
                {
                    ((Countdown)GraphicController.Selected.Graphic).RemainingHours = num;
                });
            }
        }

        private void RemainingMinutes_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Countdown)
            {
                Validators.PositiveInteger(remainingMinutes, (num) =>
                {
                    ((Countdown)GraphicController.Selected.Graphic).RemainingMin = num;
                });
            }
        }

        private void RemainingSeconds_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Countdown)
            {
                Validators.PositiveInteger(remainingSeconds, (num) =>
                {
                    ((Countdown)GraphicController.Selected.Graphic).RemainingSec = num;
                });
            }
        }

        private void StartHours_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Countdown)
            {
                Validators.PositiveInteger(startHours, (num) =>
                {
                    ((Countdown)GraphicController.Selected.Graphic).StartHour = num;
                });
            }
        }

        private void StartMinutes_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Countdown)
            {
                Validators.PositiveInteger(startMinutes, (num) =>
                {
                    ((Countdown)GraphicController.Selected.Graphic).StartMin = num;
                });
            }
        }

        private void StartSeconds_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Countdown)
            {
                Validators.PositiveInteger(startSeconds, (num) =>
                {
                    ((Countdown)GraphicController.Selected.Graphic).StartSec = num;
                });
            }
        }

        private void RemainingTimer_Checked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                ((Countdown)GraphicController.Selected.Graphic).StartAt = false;
            }
        }

        private void StartTimer_Checked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                ((Countdown)GraphicController.Selected.Graphic).StartAt = true;
            }
        }

        private void DisplayFormat_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is Countdown)
            {
                ((Countdown)GraphicController.Selected.Graphic).Format = displayFormat.Text;
            }
        }
    }
}
