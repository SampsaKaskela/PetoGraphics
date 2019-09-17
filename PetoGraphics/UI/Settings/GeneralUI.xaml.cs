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
    public partial class GeneralUI : UserControl
    {
        private OpenFileDialog fd = new OpenFileDialog();

        public GeneralUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic != null)
            {
                Validators.ClearAllErrors(grid);
                if (selected.Graphic.Image.Stretch == Stretch.Fill)
                {
                    fill.IsChecked = true;
                }
                else
                {
                    fit.IsChecked = true;
                }
                name.Text = selected.name.Content.ToString();
                controlX.Text = selected.Graphic.X.ToString();
                controlY.Text = selected.Graphic.Y.ToString();
                controlWidth.Text = selected.Graphic.GraphicWidth.ToString();
                controlHeight.Text = selected.Graphic.GraphicHeight.ToString();
                if (GraphicController.Selected.Graphic.Image.UriSource != null)
                {
                    controlImage.Text = System.IO.Path.GetFileName(selected.Graphic.Image.UriSource);
                }
                else
                {
                    controlImage.Text = string.Empty;
                }
            }
        }

        private void Name_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null)
            {
                GraphicController.Selected.name.Content = name.Text;
            }
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            if (fd.ShowDialog() ?? true)
            {
                try
                {
                    GraphicController.Selected.Graphic.Image.UriSource = fd.FileName;
                    if (GraphicController.Selected.Graphic is SequencePlayer)
                    {
                        if (((SequencePlayer)GraphicController.Selected.Graphic).InStartFrame >= GraphicController.Selected.Graphic.Image.SequenceFrames.Count)
                        {
                            ((SequencePlayer)GraphicController.Selected.Graphic).InStartFrame = GraphicController.Selected.Graphic.Image.SequenceFrames.Count - 1;
                            Windows.Main.AnimationSettingsUI.sequenceInStart.Text = ((SequencePlayer)GraphicController.Selected.Graphic).InStartFrame.ToString();
                        }
                        if (((SequencePlayer)GraphicController.Selected.Graphic).LoopStartFrame >= GraphicController.Selected.Graphic.Image.SequenceFrames.Count)
                        {
                            ((SequencePlayer)GraphicController.Selected.Graphic).LoopStartFrame = GraphicController.Selected.Graphic.Image.SequenceFrames.Count - 1;
                            Windows.Main.AnimationSettingsUI.sequenceLoopStart.Text = ((SequencePlayer)GraphicController.Selected.Graphic).LoopStartFrame.ToString();
                        }
                        if (((SequencePlayer)GraphicController.Selected.Graphic).OutStartFrame >= GraphicController.Selected.Graphic.Image.SequenceFrames.Count)
                        {
                            ((SequencePlayer)GraphicController.Selected.Graphic).OutStartFrame = GraphicController.Selected.Graphic.Image.SequenceFrames.Count - 1;
                            Windows.Main.AnimationSettingsUI.sequenceOutStart.Text = ((SequencePlayer)GraphicController.Selected.Graphic).OutStartFrame.ToString();
                        }
                    }
                    controlImage.Text = System.IO.Path.GetFileName(fd.FileName);
                }
                catch
                {
                    CustomMessageBox.Show("Image format not supported.");
                }

            }
        }

        private void ClearImage_Click(object sender, RoutedEventArgs e)
        {
            GraphicController.Selected.Graphic.Image.SequenceFrames.Clear();
            GraphicController.Selected.Graphic.Image.Clear();
            controlImage.Text = string.Empty;
        }

        private void PositionX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                Validators.Double((NumericUpDown)sender, (num) =>
                {
                    GraphicController.Selected.Graphic.X = num;
                });
            }
        }

        private void PositionY_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                Validators.Double((NumericUpDown)sender, (num) =>
                {
                    GraphicController.Selected.Graphic.Y = num;
                });
            }
        }

        private void Width_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                Validators.PositiveDouble((NumericUpDown)sender, (num) =>
                {
                    GraphicController.Selected.Graphic.GraphicWidth = num;
                });
            }
        }

        private void Height_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                Validators.PositiveDouble((NumericUpDown)sender, (num) =>
                {
                    GraphicController.Selected.Graphic.GraphicHeight = num;
                });
            }
        }

        private void Fill_Checked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                GraphicController.Selected.Graphic.Image.Stretch = Stretch.Fill;
            }
        }

        private void Fit_Checked(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic != null)
            {
                GraphicController.Selected.Graphic.Image.Stretch = Stretch.Uniform;
            }
        }
    }
}
