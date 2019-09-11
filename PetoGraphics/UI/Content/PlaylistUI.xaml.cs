using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PetoGraphics.UI.Content
{
    public partial class PlaylistUI : UserControl
    {
        private OpenFileDialog fd = new OpenFileDialog();
        private Queue<string> fileQueue = new Queue<string>();

        public PlaylistUI()
        {
            InitializeComponent();
            fd.Multiselect = true;
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is Playlist)
            {
                Visibility = Visibility.Visible;
                Playlist playlist = (Playlist)selected.Graphic;
                playlistBox.Items.Clear();
                foreach (Tuple<string, TimeSpan> source in playlist.Sources)
                {
                    playlistBox.Items.Add(source.Item1);
                }
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (fd.ShowDialog() ?? true && fd.FileNames.Length > 0)
            {
                foreach (string filename in fd.FileNames)
                {
                    fileQueue.Enqueue(filename);
                    playlistBox.Items.Add(filename);
                }
                GraphicController.Selected.info.Content = "Calculating duration";
                mediaElement.Source = new Uri(fileQueue.Dequeue());
                mediaElement.Play();
            }
        }

        private void RemoveFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Playlist && playlistBox.SelectedIndex != -1)
            {
                ((Playlist)GraphicController.Selected.Graphic).Sources.RemoveAt(playlistBox.SelectedIndex);
                playlistBox.Items.RemoveAt(playlistBox.SelectedIndex);
                GraphicController.Selected.info.Content = ComputeDuration();
            }
        }

        private void File_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected.Graphic is Playlist)
            {
                TimeSpan duration = mediaElement.NaturalDuration.TimeSpan;
                if (!((Playlist)GraphicController.Selected.Graphic).Sources.Exists(x => x.Item1 == mediaElement.Source.ToString()))
                {
                    ((Playlist)GraphicController.Selected.Graphic).Sources.Add(Tuple.Create(mediaElement.Source.ToString(), duration));
                }
                mediaElement.Stop();
                if (fileQueue.Count > 0)
                {
                    mediaElement.Source = new Uri(fileQueue.Dequeue());
                    mediaElement.Play();
                }
                else
                {
                    GraphicController.Selected.info.Content = ComputeDuration();
                    mediaElement.Source = null;
                }
            }
        }

        private string ComputeDuration()
        {
            if (((Playlist)GraphicController.Selected.Graphic).Sources.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                TimeSpan total = TimeSpan.Zero;
                foreach (Tuple<string, TimeSpan> source in ((Playlist)GraphicController.Selected.Graphic).Sources)
                {
                    total = total.Add(source.Item2);
                }
                return string.Format("Duration: {0:00}:{1:00}", (total.Hours * 60) + total.Minutes, total.Seconds);
            }
        }
    }
}
