using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Threading;

namespace PetoGraphics.Providers
{
    public class Xerberus : GraphicController
    {
        private static string authorization = string.Empty;
        private DispatcherTimer timer = new DispatcherTimer();

        public Xerberus(Graphic graphic) : base(graphic)
        {
            name.Content = "Xerberus";
            button.Click += ToggleListener;
            ShowText = "Start";
            HideText = "Stop";
            Active = false;
            timer.Tick += GetData;
        }

        public static string ClientId { get; set; } = string.Empty;

        public static string ClientSecret { get; set; } = string.Empty;

        public string Store { get; set; } = "Xerberus";

        public double Interval { get; set; } = 10;

        public string Url { get; set; } = string.Empty;

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
                if (authorization == string.Empty)
                {
                    GetToken();
                }
                else
                {
                    timer.Interval = TimeSpan.FromSeconds(Interval);
                    timer.Start();
                }
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

        private void GetToken()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.xerberusapp.com/token");
                request.Timeout = 5000;
                request.Method = "POST";
                request.ContentType = "application/json";
                string payload = JsonConvert.SerializeObject(new
                {
                    client_id = ClientId,
                    client_secret = ClientSecret,
                    grant_type = "client_credentials"
                });
                using (Stream requestStream = request.GetRequestStream())
                {
                    using (StreamWriter streamWriter = new StreamWriter(requestStream))
                    {
                        streamWriter.Write(payload);
                        streamWriter.Flush();
                    }
                }
                JObject json = ParseResponse(request.GetResponse());
                authorization = json["token_type"] + " " + json["access_token"].ToString();
                info.Content = "Token received at" + DateTime.Now.ToString("hh:mm:ss");
                GetData(null, null);
                timer.Interval = TimeSpan.FromSeconds(Interval);
                timer.Start();
            }
            catch (WebException exception)
            {
                HandleBadRequest(exception);
                Active = false;
            }
            catch (Exception exception)
            {
                info.Content = exception.Message;
                Active = false;
            }
        }

        private void GetData(object sender, EventArgs e)
        {
            try
            {
                WebRequest request = WebRequest.Create(Url);
                request.Timeout = 5000;
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", authorization);
                JObject json = ParseResponse(request.GetResponse());
                info.Content = "Data received at " + DateTime.Now.ToString("hh:mm:ss");
                Providers.Store.Instance.UpdateStore(Store, json);
            }
            catch (WebException exception)
            {
                HandleBadRequest(exception);
                timer.Stop();
                GetToken();
            }
            catch (Exception exception)
            {
                info.Content = exception.Message;
            }
        }

        private JObject ParseResponse(WebResponse response)
        {
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string result = reader.ReadToEnd();
            return JObject.Parse(result);
        }

        private void HandleBadRequest(WebException exception)
        {
            Stream responseStream = exception.Response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string result = reader.ReadToEnd();
            JObject json = JObject.Parse(result);
            if (json["error_description"] != null)
            {
                info.Content = json["error_description"].ToString();
            }
            else
            {
                info.Content = exception.Message;
            }
        }
    }
}
