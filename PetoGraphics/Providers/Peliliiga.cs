using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Threading;

namespace PetoGraphics.Providers
{
    public class Peliliiga : GraphicController
    {
        private DispatcherTimer timer = new DispatcherTimer();

        public Peliliiga(Graphic graphic) : base(graphic)
        {
            name.Content = "Peliliiga";
            button.Click += ToggleListener;
            ShowText = "Start";
            HideText = "Stop";
            Active = false;
            timer.Tick += GetData;
        }

        public string Store { get; set; } = "Peliliiga";

        public double Interval { get; set; } = 10;

        public string Url { get; set; } = "http://tournaments.peliliiga.fi/winter16/groups/view/4.json";

        private void ToggleListener(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                StopListening();
            }
            else
            {
                StartListening();
            }
        }

        private void StartListening()
        {
            if (Uri.IsWellFormedUriString(Url, UriKind.Absolute))
            {
                timer.Interval = TimeSpan.FromSeconds(Interval);
                GetData(null, null);
                timer.Start();
            }
            else
            {
                info.Content = "Not valid url";
                Active = false;
            }
        }

        private void StopListening()
        {
            timer.Stop();
        }

        private void GetData(object sender, EventArgs e)
        {
            try
            {
                WebRequest request = WebRequest.Create(Url);
                request.Timeout = 5000;
                request.Method = "GET";
                request.ContentType = "application/json";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Stream stream = request.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string result = sr.ReadToEnd();
                JObject json = JObject.Parse(result);
                info.Content = "Data received at " + DateTime.Now.ToString("hh:mm:ss");
                Providers.Store.Instance.UpdateStore(Store, json);
            }
            catch (Exception exception)
            {
                info.Content = exception.Message;
                Active = false;
            }
        }
    }
}
