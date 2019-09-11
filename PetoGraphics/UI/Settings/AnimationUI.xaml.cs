using PetoGraphics.Helpers;
using PetoGraphics.UI.Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetoGraphics.UI.Settings
{
    /// <summary>
    /// Interaction logic for AnimationUI.xaml
    /// </summary>
    public partial class AnimationUI : UserControl
    {

        public AnimationUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic != null)
            {
                if (selected.Graphic is SequencePlayer)
                {
                    basicAnimation.Visibility = Visibility.Collapsed;
                    sequenceAnimation.Visibility = Visibility.Visible;
                    Validators.ClearAllErrors(sequenceAnimation);
                    SequencePlayer sequencePlayer = (SequencePlayer)selected.Graphic;
                    sequenceInStart.Text = sequencePlayer.InStartFrame.ToString();
                    sequenceLoopStart.Text = sequencePlayer.LoopStartFrame.ToString();
                    sequenceOutStart.Text = sequencePlayer.OutStartFrame.ToString();
                }
                else
                {
                    basicAnimation.Visibility = Visibility.Visible;
                    sequenceAnimation.Visibility = Visibility.Collapsed;
                    Validators.ClearAllErrors(basicAnimation);
                    animationInStyle.SelectedIndex = (int)GraphicController.Selected.Graphic.AnimationIn.Style;
                    animationInDuration.Text = GraphicController.Selected.Graphic.AnimationIn.Duration.ToString();
                    animationInDelay.Text = GraphicController.Selected.Graphic.AnimationIn.Delay.ToString();
                    animationInEasePower.Text = GraphicController.Selected.Graphic.AnimationIn.EasePower.ToString();
                    animationOutStyle.SelectedIndex = (int)GraphicController.Selected.Graphic.AnimationOut.Style;
                    animationOutDuration.Text = GraphicController.Selected.Graphic.AnimationOut.Duration.ToString();
                    animationOutDelay.Text = GraphicController.Selected.Graphic.AnimationOut.Delay.ToString();
                    animationOutEasePower.Text = GraphicController.Selected.Graphic.AnimationOut.EasePower.ToString();
                    if (GraphicController.Selected.Graphic.AnimationIn.AddFade)
                    {
                        animationInAddFade.IsChecked = true;
                    }
                    else
                    {
                        animationInAddFade.IsChecked = false;
                    }
                    if (GraphicController.Selected.Graphic.AnimationOut.AddFade)
                    {
                        animationOutAddFade.IsChecked = true;
                    }
                    else
                    {
                        animationOutAddFade.IsChecked = false;
                    }
                    if (GraphicController.Selected.Graphic.AnimationIn.Style == GraphicAnimation.AnimationStyle.Fade)
                    {
                        animationInAddFade.IsEnabled = false;
                        animationInAddFade.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                    }
                    if (GraphicController.Selected.Graphic.AnimationOut.Style == GraphicAnimation.AnimationStyle.Fade)
                    {
                        animationOutAddFade.IsEnabled = false;
                        animationOutAddFade.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                    }
                    if (GraphicController.Selected.Graphic.AnimationIn.Ease)
                    {
                        animationineasing.IsChecked = true;
                        animationInEasePower.IsEnabled = true;
                    }
                    else
                    {
                        animationineasing.IsChecked = false;
                        animationInEasePower.IsEnabled = false;
                    }
                    if (GraphicController.Selected.Graphic.AnimationOut.Ease)
                    {
                        animationOutEasing.IsChecked = true;
                        animationOutEasePower.IsEnabled = true;
                    }
                    else
                    {
                        animationOutEasing.IsChecked = false;
                        animationOutEasePower.IsEnabled = false;
                    }
                }
            }
        }

        private void AnimationInStyle_Changed(object sender, EventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                if (animationInStyle.SelectedIndex != 0)
                {
                    animationInAddFade.IsEnabled = true;
                    animationInAddFade.ClearValue(ForegroundProperty);
                }
                else
                {
                    animationInAddFade.IsEnabled = false;
                    animationInAddFade.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                }
                if (GraphicController.Selected.Graphic.AnimationIn.AddFade)
                {
                    animationInAddFade.IsChecked = true;
                }
                else
                {
                    animationInAddFade.IsChecked = false;
                }
                GraphicController.Selected.Graphic.AnimationIn.Style = (GraphicAnimation.AnimationStyle)animationInStyle.SelectedIndex;
            }
        }

        private void AnimationInDuration_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                Validators.PositiveDouble((NumericUpDown)sender, (num) =>
                {
                    GraphicController.Selected.Graphic.AnimationIn.Duration = num;
                });
            }
        }

        private void AnimationInDelay_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                Validators.PositiveDouble((NumericUpDown)sender, (num) =>
                {
                    GraphicController.Selected.Graphic.AnimationIn.Delay = num;
                });
            }
        }

        private void AnimationInAddFade_Checked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                GraphicController.Selected.Graphic.AnimationIn.AddFade = true;
            }
        }

        private void AnimationInAddFade_Unchecked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                GraphicController.Selected.Graphic.AnimationIn.AddFade = false;
            }
        }

        private void AnimationInEasePower_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                Validators.PositiveDouble((NumericUpDown)sender, (num) =>
                {
                    GraphicController.Selected.Graphic.AnimationIn.EasePower = num;
                });
            }
        }

        private void AnimationInEasing_Checked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                GraphicController.Selected.Graphic.AnimationIn.Ease = true;
                animationInEasePower.IsEnabled = true;
            }
        }

        private void AnimationInEasing_Unchecked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                GraphicController.Selected.Graphic.AnimationIn.Ease = false;
                animationInEasePower.IsEnabled = false;
            }
        }

        private void AnimationOutStyle_Changed(object sender, EventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                if (animationOutStyle.SelectedIndex != 0)
                {
                    animationOutAddFade.IsEnabled = true;
                    animationOutAddFade.ClearValue(ForegroundProperty);
                }
                else
                {
                    animationOutAddFade.IsEnabled = false;
                    animationInAddFade.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                }
                if (GraphicController.Selected.Graphic.AnimationOut.AddFade)
                {
                    animationOutAddFade.IsChecked = true;
                }
                else
                {
                    animationOutAddFade.IsChecked = false;
                }
                GraphicController.Selected.Graphic.AnimationOut.Style = (GraphicAnimation.AnimationStyle)animationOutStyle.SelectedIndex;
            }
        }

        private void AnimationOutDuration_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                Validators.PositiveDouble((NumericUpDown)sender, (num) =>
                {
                    GraphicController.Selected.Graphic.AnimationOut.Duration = num;
                });
            }
        }

        private void AnimationOutDelay_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                Validators.PositiveDouble((NumericUpDown)sender, (num) =>
                {
                    GraphicController.Selected.Graphic.AnimationOut.Delay = num;
                });
            }
        }

        private void AnimationOutAddFade_Checked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                GraphicController.Selected.Graphic.AnimationOut.AddFade = true;
            }
        }

        private void AnimationOutAddFade_Unchecked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                GraphicController.Selected.Graphic.AnimationOut.AddFade = false;
            }
        }

        private void AnimationOutEasePower_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                Validators.PositiveDouble((NumericUpDown)sender, (num) =>
                {
                    GraphicController.Selected.Graphic.AnimationOut.EasePower = num;
                });
            }
        }

        private void AnimationOutEasing_Checked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                GraphicController.Selected.Graphic.AnimationOut.Ease = true;
                animationOutEasePower.IsEnabled = true;
            }
        }

        private void AnimationOutEasing_Unchecked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                GraphicController.Selected.Graphic.AnimationOut.Ease = false;
                animationOutEasePower.IsEnabled = false;
            }
        }

        private void SequenceInStart_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is SequencePlayer)
            {
                Validators.PositiveInteger((TextBox)sender, (num) =>
                {
                    ((SequencePlayer)GraphicController.Selected.Graphic).InStartFrame = num;
                });
            }
        }

        private void SequenceLoopStart_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is SequencePlayer)
            {
                Validators.PositiveInteger((TextBox)sender, (num) =>
                {
                    ((SequencePlayer)GraphicController.Selected.Graphic).LoopStartFrame = num;
                });
            }
        }

        private void SequenceOutStart_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is SequencePlayer)
            {
                Validators.PositiveInteger((TextBox)sender, (num) =>
                {
                    ((SequencePlayer)GraphicController.Selected.Graphic).OutStartFrame = num;
                });
            }
        }

    }
}
