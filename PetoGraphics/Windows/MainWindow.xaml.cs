using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Windows.Threading;
using System.Diagnostics;
using PetoGraphics.Providers;

namespace PetoGraphics
{
    public partial class MainWindow : Window
    {
        private OpenFileDialog fd = new OpenFileDialog();
        private BrushConverter bc = new BrushConverter();

        #region MainWindow

        public MainWindow()
        {
            InitializeComponent();
            Windows.Main = this;
            Windows.Source = new Source();
            Windows.Alpha = new Alpha();
            Windows.Alpha.Close();
            Windows.Ndi = new NDIwindow();
            Windows.About = new About();
            Windows.OutputSettings = new OutputSettings();
            Windows.ProgressWindow = new ProgressWindow();
            Windows.OutputSettings.bgColor.Text = "Green";

            GraphicController.holdTimer = new DispatcherTimer();
            GraphicController.holdTimer.Interval = TimeSpan.FromMilliseconds(500);
            GraphicController.holdTimer.Tick += GraphicController.Dragging_Tick;
            GraphicController.OnSelectedChanged += OnSelectedChanged;

            // Load configs
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Configs"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Configs");
            }
            string[] folders = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "Configs", "*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < folders.Length; i++)
            {
                targetFolder.Items.Add(Path.GetFileName(folders[i]));
            }

            // Initial size for smaller screens
            if (SystemParameters.PrimaryScreenHeight < 1080)
            {
                Windows.Main.Width = 392;
                Windows.Main.Height = 666;
                Windows.OutputSettings.SetWidth(720);
                Windows.OutputSettings.SetHeight(480);
                Windows.OutputSettings.widthSetting.Text = "720";
                Windows.OutputSettings.heightSetting.Text = "480";
            }
            //InitVoiceControl();
            //InitDiagnostics();
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null)
            {
                contentTab.Visibility = Visibility.Visible;
                if (selected.Graphic != null)
                {
                    settingsTab.Visibility = Visibility.Visible;
                }
                else
                {
                    settingsTab.Visibility = Visibility.Collapsed;
                    tabControl.SelectedIndex = 0;
                }
            }
            else
            {
                contentTab.Visibility = Visibility.Collapsed;
                settingsTab.Visibility = Visibility.Collapsed;
            }
        }

        private void Main_Rendered(object sender, EventArgs e)
        {
            Windows.Source.Show();
            targetFolder.SelectedIndex = 0;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //diagnosticstimer.Stop();
            CefSharp.Cef.Shutdown();
            App.Current.Shutdown();
        }


        #endregion

        #region Controller tree

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (GraphicController.potentialParent != null && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                GraphicController.potentialParent.border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1B1B1C"));
                GraphicController.potentialParent = null;
            }
        }

        private void ContainerScrollViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                e.Handled = true;
            }
        }

        // Move controller with mouse
        private void ControllerContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && GraphicController.dragging)
            {
                int offset = -23;
                Point mousepoint = e.GetPosition(Windows.Main.controllerContainer);
                GraphicController.Selected.Margin = new Thickness(mousepoint.X - GraphicController.mouseStartPoint.X, mousepoint.Y + offset, 0, 0);
            }
        }

        // Mouse was released, move controllers to proper positions
        private void ControllerContainer_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            GraphicController.holdTimer.Stop();
            if (GraphicController.dragging)
            {
                GraphicController.Selected.Opacity = 1;
                Debug.Write("Is hittest visible");
                GraphicController.Selected.IsHitTestVisible = true;
                if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                {
                    GraphicController.Selected.SetParent();
                }
                else
                {
                    // If returns false, it's the last element
                    if (!GraphicController.Selected.CalculatePosition(GraphicController.rootList, null))
                    {
                        GraphicController last = ControllerHelpers.FindLastVisible(GraphicController.rootList);
                        if (last != null && last != GraphicController.Selected)
                        {
                            GraphicController.Selected.RemovePreviousParent();
                            if (last.ControllerParent != null)
                            {
                                last.ControllerParent.Children.Add(GraphicController.Selected);
                            }
                            else
                            {
                                GraphicController.rootList.Add(GraphicController.Selected);
                            }
                            GraphicController.Selected.ControllerParent = last.ControllerParent;
                        }
                    }
                }
                ControllerHelpers.RepositionControls(GraphicController.rootList);
                ControllerHelpers.CheckResize();
                GraphicController.dragging = false;
            }
        }

        #endregion

        #region Menu

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            if (!Windows.About.IsVisible)
            {
                Windows.About.Show();
            }
        }

        private void Alpha_Click(object sender, RoutedEventArgs e)
        {
            if (!Windows.Alpha.IsVisible)
            {
                Windows.Alpha.Show();
            }
        }

        private void NDI_Click(object sender, RoutedEventArgs e)
        {
            if (!Windows.Ndi.IsVisible)
            {
                Windows.Ndi.Show();
            }
        }

        private void PreviewSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!Windows.OutputSettings.IsVisible)
            {
                //Find available monitors
                Windows.OutputSettings.sourceMonitorSelect.Items.Clear();
                Windows.OutputSettings.alphaMonitorSelect.Items.Clear();
                for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
                {
                    Windows.OutputSettings.sourceMonitorSelect.Items.Add((i + 1).ToString());
                    Windows.OutputSettings.alphaMonitorSelect.Items.Add((i + 1).ToString());
                }
                if (Windows.OutputSettings.SourceMonitor < Windows.OutputSettings.sourceMonitorSelect.Items.Count)
                {
                    Windows.OutputSettings.sourceMonitorSelect.SelectedIndex = Windows.OutputSettings.SourceMonitor;
                }
                else
                {
                    Windows.OutputSettings.sourceMonitorSelect.SelectedIndex = 0;
                }
                if (Windows.OutputSettings.AlphaMonitor < Windows.OutputSettings.alphaMonitorSelect.Items.Count)
                {
                    Windows.OutputSettings.alphaMonitorSelect.SelectedIndex = Windows.OutputSettings.AlphaMonitor;
                }
                else
                {
                    Windows.OutputSettings.alphaMonitorSelect.SelectedIndex = 0;
                }
                Windows.OutputSettings.Show();
            }
        }


        private void Editor_Click(object sender, RoutedEventArgs e)
        {
            Windows.Source.ToggleEditing();
        }

        #endregion

        #region Add controls

        private void AddScoreboard_Click(object sender, RoutedEventArgs e)
        {
            Scoreboard scoreboard = new Scoreboard(Windows.Source.canvas);
        }

        private void AddSingleText_Click(object sender, RoutedEventArgs e)
        {
            SingleText singleText = new SingleText(Windows.Source.canvas);
        }

        private void AddDoubleText_Click(object sender, RoutedEventArgs e)
        {
            DoubleText doubleText = new DoubleText(Windows.Source.canvas);
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            ImageOnly imageOnly = new ImageOnly(Windows.Source.canvas);
        }

        private void AddCountdown_Click(object sender, RoutedEventArgs e)
        {
            Countdown countdown = new Countdown(Windows.Source.canvas);
        }

        private void AddGroups_Click(object sender, RoutedEventArgs e)
        {
            Group groups = new Group(Windows.Source.canvas);
        }

        private void AddBracket_Click(object sender, RoutedEventArgs e)
        {
            Bracket bracket = new Bracket(Windows.Source.canvas);
            bracket.Redraw();
        }

        private void AddSequencePlayer_Click(object sender, RoutedEventArgs e)
        {
            SequencePlayer sequencePlayer = new SequencePlayer(Windows.Source.canvas);
        }

        private void AddMedia_Click(object sender, RoutedEventArgs e)
        {
            Media media = new Media(Windows.Source.canvas);
        }

        private void AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            Playlist playlist = new Playlist(Windows.Source.canvas);
        }

        private void AddClock_Click(object sender, RoutedEventArgs e)
        {
            Clock clock = new Clock(Windows.Source.canvas);
        }

        private void AddImageSlider_Click(object sender, RoutedEventArgs e)
        {
            ImageSlider imageSlider = new ImageSlider(Windows.Source.canvas);
        }

        private void AddImageSwitcher_Click(object sender, RoutedEventArgs e)
        {
            ImageSwitcher imageSwitcher = new ImageSwitcher(Windows.Source.canvas);
        }

        private void AddWebSource_Click(object sender, RoutedEventArgs e)
        {
            WebSource websource = new WebSource(Windows.Source.canvas);
        }

        private void AddRichText_Click(object sender, RoutedEventArgs e)
        {
            RichText richText = new RichText(Windows.Source.canvas);
        }

        private void AddBlank_Click(object sender, RoutedEventArgs e)
        {
            Blank blank = new Blank(null);
        }

        private void AddPeliliiga_Click(object sender, RoutedEventArgs e)
        {
            Peliliiga peliliiga = new Peliliiga(null);
        }

        private void AddXerberus_Click(object sender, RoutedEventArgs e)
        {
            Xerberus xerberus = new Xerberus(null);
        }

        #endregion

        #region Config

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (targetFolder.Items.Count > 0)
            {
                Config.Save(targetFolder.Text);
            }
            else
            {
                SaveAs_Click(sender, e);
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFolder savefolder = new SaveFolder();
            savefolder.ShowDialog();
            if (savefolder.saveFolder)
            {
                targetFolder.SelectionChanged -= new SelectionChangedEventHandler(TargetFolder_SelectionChanged); // prevent reloading config
                Config.Save(savefolder.name);
                targetFolder.SelectedValue = savefolder.name;
                targetFolder.SelectionChanged += new SelectionChangedEventHandler(TargetFolder_SelectionChanged);
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            if (fd.ShowDialog() ?? true)
            {
                targetFolder.SelectionChanged -= new SelectionChangedEventHandler(TargetFolder_SelectionChanged); // prevent reloading config
                targetFolder.SelectedValue = Path.GetFileName(Path.GetDirectoryName(fd.FileName));
                targetFolder.SelectionChanged += new SelectionChangedEventHandler(TargetFolder_SelectionChanged);
                Config.Load(fd.FileName);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CustomMessageBox.Show("Do you want to delete config with name \"" + targetFolder.SelectedItem + "\" and all its files?", "Delete config", true) == true)
            {
                targetFolder.SelectionChanged -= new SelectionChangedEventHandler(TargetFolder_SelectionChanged); // prevent reloading config
                int result = Config.Delete(AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + targetFolder.SelectedItem + @"\");
                targetFolder.SelectedValue = string.Empty;
                targetFolder.SelectionChanged += new SelectionChangedEventHandler(TargetFolder_SelectionChanged);
                switch (result)
                {
                    case 0:
                        {
                            CustomMessageBox.Show("Config file deleted.");
                            break;
                        }

                    case 1:
                        {
                            CustomMessageBox.Show("Failed to delete config file. Some files are used by another application or are read-only.");
                            break;
                        }

                    case 2:
                        {
                            CustomMessageBox.Show("Failed to delete config file. You don't have required permissions.");
                            break;
                        }

                    case 3:
                        {
                            CustomMessageBox.Show("Failed to delete config file.");
                            break;
                        }

                }
            }
        }

        private void TargetFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (targetFolder.Items.Count > 0)
            {
                Config.Load(AppDomain.CurrentDomain.BaseDirectory + @"Configs\" + targetFolder.SelectedItem + @"\" + targetFolder.SelectedItem + ".json");
            }
        }

        #endregion

        #region Hotkeys

        private void Main_PreviewKeydown(object sender, KeyEventArgs e)
        {
            // Move to previous controller
            if (e.Key == Key.Up && GraphicController.Selected != null && !(Keyboard.FocusedElement is TextBox || Keyboard.FocusedElement is RichTextBox))
            {
                GraphicController previous = ControllerHelpers.FindPreviousVisibleFrom(GraphicController.Selected);
                if (previous != null)
                {
                    MouseButtonEventArgs mouseEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                    mouseEvent.RoutedEvent = MouseLeftButtonDownEvent;
                    previous.border.RaiseEvent(mouseEvent);
                    // Release event by giving opposite event
                    mouseEvent.RoutedEvent = MouseLeftButtonUpEvent;
                    GraphicController.Selected.border.RaiseEvent(mouseEvent);
                    // Bring selected to view if it is not already
                    if (GraphicController.Selected.Margin.Top < containerscrollviewer.VerticalOffset)
                    {
                        containerscrollviewer.ScrollToVerticalOffset(GraphicController.Selected.Margin.Top - containerscrollviewer.Height);
                    }
                }
            }

            // Move to next controller
            if (e.Key == Key.Down && GraphicController.Selected != null && !(Keyboard.FocusedElement is TextBox || Keyboard.FocusedElement is RichTextBox))
            {
                GraphicController next = ControllerHelpers.FindNextVisibleFrom(GraphicController.Selected);
                if (next != null)
                {
                    MouseButtonEventArgs mouseEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                    mouseEvent.RoutedEvent = MouseLeftButtonDownEvent;
                    next.border.RaiseEvent(mouseEvent);
                    // Release event by giving opposite event
                    mouseEvent.RoutedEvent = MouseLeftButtonUpEvent;
                    GraphicController.Selected.border.RaiseEvent(mouseEvent);
                    // Bring selected to view if it is not already
                    if (GraphicController.Selected.Margin.Top >= containerscrollviewer.VerticalOffset + containerscrollviewer.Height)
                    {
                        containerscrollviewer.ScrollToVerticalOffset(GraphicController.Selected.Margin.Top + GraphicController.Selected.border.Height - containerscrollviewer.Height);
                    }
                }
            }
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            // Delete controller
            if (e.Key == Key.Delete && GraphicController.Selected != null)
            {
                GraphicController.Selected.ShowChildren();
                GraphicController.Selected.Delete();
                GraphicController.Selected = null;
                Windows.Main.contentTab.Visibility = Visibility.Hidden;
                Windows.Main.tabControl.SelectedIndex = 0;
            }
            // Prepare to copy
            if (e.Key == Key.C && (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl)))
            {
                GraphicController.copy = GraphicController.Selected;
            }
            // Copy controller
            if (GraphicController.copy != null && e.Key == Key.V && (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl)))
            {
                GraphicController.copy.Copy();
                containerscrollviewer.ScrollToBottom();
            }
            // Hide all graphics
            if (e.Key == Key.Escape)
            {
                for (int i = 0; i < GraphicController.rootList.Count; i++)
                {
                    GraphicController.rootList[i].Active = false;
                }
            }
            if (GraphicController.Selected != null && e.Key == Key.F2)
            {
                GraphicController.Selected.button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        #endregion

        #region Experimental

        #region Voice Control

        /*private void InitVoiceControl()
        {
            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commands = new Choices("show", "hide");
            grammarBuilder.Append(commands);
            recognizer.LoadGrammar(new Grammar(grammarBuilder));
            recognizer.SpeechRecognized += recognizer_SpeechRecognized;
            recognizer.SpeechDetected += recognizer_SpeechDetected;
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void recognizer_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            MessageBox.Show("I hear your");
        }

        void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            MessageBox.Show(e.Result.Text);
            switch(e.Result.Text)
            {
                case "show":
                    {
                        if (!Controller.selected.graphic.visible)
                        {
                            Controller.selected.button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        }
                        break;
                    }

                case "hide":
                    {
                        if (Controller.selected.graphic.visible)
                        {
                            Controller.selected.button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        }
                        break;
                    }
            }
        }*/

        #endregion

        #region Performance Diagnostics

        /*PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;
        DispatcherTimer diagnosticstimer;

        private void InitDiagnostics()
        {
            cpuCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
            ramCounter = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);
            diagnosticstimer = new DispatcherTimer();
            diagnosticstimer.Interval = TimeSpan.FromSeconds(1);
            diagnosticstimer.Tick += updateDiagnostics;
            diagnosticstimer.Start();
        }

        private void UpdateDiagnostics(object sender, EventArgs e)
        {
            CPUusage.Content = string.Format("{0:0.0}", cpuCounter.NextValue() / Environment.ProcessorCount) + " %";
            RAMusage.Content = string.Format("{0:0.0}", ramCounter.NextValue() / (1024 * 1024)) + " MB";
        }*/

        #endregion

        #endregion
    }
}