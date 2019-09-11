using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Windows.Controls;

namespace PetoGraphics
{
    class JSONreader
    {
        // Read data from url to selected group
        public static void ReadGroup(string url, string parser)
        {

            if (url != string.Empty)
            {
                try
                {
                    string jsonstring;
                    try
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        using (WebClientWithTimeout wc = new WebClientWithTimeout())
                        {
                            jsonstring = wc.DownloadString(url);
                        }
                    }
                    catch
                    {
                        CustomMessageBox.Show("Failed to load JSON file. Request timeout.");
                        jsonstring = string.Empty;
                    }

                    if (jsonstring != string.Empty)
                    {
                        JObject root = JObject.Parse(jsonstring);

                        switch (parser)
                        {
                            case "tournaments.peliliiga.fi":
                                {
                                    ParsePeliliigaGroup(root);
                                    break;
                                }

                            case "Toornaments":
                                {
                                    ParseToornamentGroup(root);
                                    break;
                                }

                            default:
                                {
                                    CustomMessageBox.Show("Parser not selected.");
                                    break;
                                }
                        }
                    }
                }
                catch
                {
                    CustomMessageBox.Show("Could not read JSON file.");
                }
            }
            else
            {
                CustomMessageBox.Show("JSON url not set.");
            }
        }

        // Read from url to selected bracket
        public static void readBracket(string url, string parser)
        {
            if (url != string.Empty)
            {
                try
                {
                    string jsonstring;
                    try
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        using (WebClientWithTimeout wc = new WebClientWithTimeout())
                        {
                            jsonstring = wc.DownloadString(url);
                        }
                    }
                    catch
                    {
                        CustomMessageBox.Show("Failed to load JSON file. Request timeout.");
                        jsonstring = string.Empty;
                    }

                    if (jsonstring != string.Empty)
                    {
                        JObject root = JObject.Parse(jsonstring);

                        switch (parser)
                        {
                            case "tournaments.peliliiga.fi":
                                {
                                    ParsePeliliigaBracket(root);
                                    break;
                                }

                            case "Toornaments":
                                {
                                    ParseToornamentBracket(root);
                                    break;
                                }

                            default:
                                {
                                    CustomMessageBox.Show("Parser not selected.");
                                    break;
                                }
                        }
                    }
                }
                catch
                {
                    CustomMessageBox.Show("Could not read JSON file.");
                }
            }
            else
            {
                CustomMessageBox.Show("JSON url not set.");
            }
        }

        private static void ParsePeliliigaGroup(JObject root)
        {
            Windows.Main.GroupsUI.title.Text = root["Group"]["name"].ToString(); // Group name
            ((Group)GraphicController.Selected.Graphic).Texts[0].Content = Windows.Main.GroupsUI.title.Text;

            JArray items = (JArray)root["GroupResult"]; // Contestants
            for (int i = 1; i < 6; i++)
            {
                TextBox namebox = (TextBox)Windows.Main.GroupsUI.FindName("n" + i.ToString());
                TextBox scorebox = (TextBox)Windows.Main.GroupsUI.FindName("s" + i.ToString());
                GraphicText name = ((Group)GraphicController.Selected.Graphic).Texts[i];
                GraphicText score = ((Group)GraphicController.Selected.Graphic).Texts[i + 5];
                if (i < items.Count) // if there is data
                {
                    namebox.Text = items[i]["Contestant"]["name"].ToString(); // Contestant name
                    scorebox.Text = items[i]["wins"].ToString() + "-" + items[i]["losses"].ToString(); // wins and losses
                    name.Content = namebox.Text;
                    score.Content = scorebox.Text;
                }
                else
                {
                    namebox.Text = string.Empty;
                    scorebox.Text = string.Empty;
                    name.Content = string.Empty;
                    score.Content = string.Empty;
                }
            }
        }

        private static void ParseToornamentGroup(JObject root)
        {

        }

        private static void ParsePeliliigaBracket(JObject root)
        {
            Windows.Main.BracketUI.BracketTitle.Text = root["League"]["name"].ToString(); // Tournament name
            GraphicController.Selected.Graphic.Texts[0].Content = Windows.Main.BracketUI.BracketTitle.Text;
            JArray items = (JArray)root["Matches"][0]; // Matches. Array inside a array so take array at 0 index.
            int increment = 1;
            for (int i = 0; i < 7; i++) // loop matches. Fill from last to first
            {
                TextBox namebox = (TextBox)Windows.Main.BracketUI.FindName("N" + increment.ToString());
                TextBox scorebox = (TextBox)Windows.Main.BracketUI.FindName("S" + increment.ToString());
                if (i < items.Count) // has data
                {
                    scorebox.Text = items[items.Count - i - 1]["MatchContestant1"]["score"].ToString(); // Contestant1 score
                    namebox.Text = items[items.Count - i - 1]["Contestant1"]["name"].ToString(); // Contestant1 name
                }
                else
                {
                    scorebox.Text = string.Empty;
                    namebox.Text = string.Empty;
                }
                ((Bracket)GraphicController.Selected.Graphic).Scores[increment - 1].Content = scorebox.Text;
                ((Bracket)GraphicController.Selected.Graphic).Names[increment - 1].Content = namebox.Text;
                increment++;

                namebox = (TextBox)Windows.Main.BracketUI.FindName("N" + increment.ToString());
                scorebox = (TextBox)Windows.Main.BracketUI.FindName("S" + increment.ToString());
                if (i < items.Count) // has data
                {
                    scorebox.Text = items[items.Count - i - 1]["MatchContestant2"]["score"].ToString(); // Contestant2 score
                    namebox.Text = items[items.Count - i - 1]["Contestant2"]["name"].ToString(); // Contestant2 name
                }
                else
                {
                    scorebox.Text = string.Empty;
                    namebox.Text = string.Empty;
                }
                ((Bracket)GraphicController.Selected.Graphic).Scores[increment - 1].Content = scorebox.Text;
                ((Bracket)GraphicController.Selected.Graphic).Names[increment - 1].Content = namebox.Text;
                increment++;
            }
        }

        private static void ParseToornamentBracket(JObject root)
        {

        }

        public class WebClientWithTimeout : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest wr = base.GetWebRequest(address);
                wr.Timeout = 5000; // timeout in milliseconds (ms)
                return wr;
            }
        }
    }
}
