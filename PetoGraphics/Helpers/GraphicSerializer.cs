using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows;
using System.Windows.Media;
using PetoGraphics.Providers;

namespace PetoGraphics
{
    class GraphicSerializer
    {
        private string folderName;

        public JObject Serialize(List<GraphicController> controllers, string folderName)
        {
            this.folderName = folderName;
            JObject config = new JObject();
            JObject output = new JObject();
            output.Add("width", Windows.Source.container.Width);
            output.Add("height", Windows.Source.container.Height);
            output.Add("background", Windows.Source.canvas.Background.ToString());
            output.Add("fps", Windows.OutputSettings.FPS);
            config.Add("output", output);
            JArray graphics = new JArray();
            foreach(GraphicController controller in controllers)
            {
                graphics.Add(SerializeSingle(controller));
            }
            config.Add("graphics", graphics);
            return config;
        }

        public JObject SerializeSingle(GraphicController controller, bool copy = false)
        {
            JObject graphic = new JObject();
            graphic.Add("id", controller.Id);
            graphic.Add("name", controller.name.Content.ToString());
            if (controller.Graphic == null) // is graphic or controller?
            {
                graphic.Add("type", controller.GetType().Name);
                if (controller.GetType().Name == "Peliliiga")
                {
                    Peliliiga peliliiga = (Peliliiga)controller;
                    graphic.Add("url", peliliiga.Url);
                    graphic.Add("store", peliliiga.Store);
                    graphic.Add("interval", peliliiga.Interval);
                }
                else if (controller.GetType().Name == "Xerberus")
                {
                    Xerberus xerberus = (Xerberus)controller;
                    graphic.Add("url", xerberus.Url);
                    graphic.Add("store", xerberus.Store);
                    graphic.Add("interval", xerberus.Interval);
                    graphic.Add("clientId", Xerberus.ClientId);
                    graphic.Add("clientSecret", Xerberus.ClientSecret);
                }
            }
            else
            {
                Graphic controllerGraphic = controller.Graphic;
                graphic.Add("type", controllerGraphic.GetType().Name);
                graphic.Add("x", controllerGraphic.X);
                graphic.Add("y", controllerGraphic.Y);
                graphic.Add("width", controllerGraphic.GraphicWidth);
                graphic.Add("height", controllerGraphic.GraphicHeight);

                JObject image = new JObject();
                image.Add("isSequence", controllerGraphic.Image.IsSequence);
                image.Add("stretch", controllerGraphic.Image.Stretch.ToString());
                if (controllerGraphic.Image.UriSource != null)
                {
                    if (copy)
                    {
                        image.Add("uri", controllerGraphic.Image.UriSource);
                    }
                    else
                    {
                        if (controllerGraphic.Image.IsSequence)
                        {
                            image.Add("uri", @"Configs\" + folderName + @"\" + Path.GetFileName(Path.GetDirectoryName(controllerGraphic.Image.UriSource)) + @"\" + Path.GetFileName(controllerGraphic.Image.UriSource));
                        }
                        else
                        {
                            image.Add("uri", @"Configs\" + folderName + @"\" + Path.GetFileName(controllerGraphic.Image.UriSource));
                        }
                    }
                }
                graphic.Add("image", image);

                JObject animationIn = new JObject();
                animationIn.Add("style", controllerGraphic.AnimationIn.Style.ToString());
                animationIn.Add("addFade", controllerGraphic.AnimationIn.AddFade);
                animationIn.Add("duration", controllerGraphic.AnimationIn.Duration);
                animationIn.Add("delay", controllerGraphic.AnimationIn.Delay);
                animationIn.Add("ease", controllerGraphic.AnimationIn.Ease);
                animationIn.Add("easePower", controllerGraphic.AnimationIn.EasePower);
                graphic.Add("animationIn", animationIn);

                JObject animationOut = new JObject();
                animationOut.Add("style", controllerGraphic.AnimationOut.Style.ToString());
                animationOut.Add("addFade", controllerGraphic.AnimationOut.AddFade);
                animationOut.Add("duration", controllerGraphic.AnimationOut.Duration);
                animationOut.Add("delay", controllerGraphic.AnimationOut.Delay);
                animationOut.Add("ease", controllerGraphic.AnimationOut.Ease);
                animationOut.Add("easePower", controllerGraphic.AnimationOut.EasePower);
                graphic.Add("animationOut", animationOut);

                if (controllerGraphic.Texts != null)
                {
                    JArray texts = new JArray();
                    foreach (GraphicText graphicText in controllerGraphic.Texts)
                    {
                        JObject text = new JObject();
                        text.Add("content", graphicText.Content);
                        text.Add("x", graphicText.X);
                        text.Add("y", graphicText.Y);
                        text.Add("width", graphicText.Width);
                        text.Add("lineHeight", graphicText.LineHeight);

                        JObject font = new JObject();
                        font.Add("family", graphicText.FontFamily);
                        font.Add("size", graphicText.FontSize);
                        font.Add("weight", graphicText.FontWeight.ToString());
                        font.Add("style", graphicText.FontStyle.ToString());
                        font.Add("color", graphicText.FontColor);
                        font.Add("hasGlow", graphicText.HasGlow);
                        font.Add("glowColor", graphicText.GlowColor);
                        font.Add("align", graphicText.ContentAlign.ToString());
                        text.Add("font", font);

                        texts.Add(text);
                    }
                    graphic.Add("texts", texts);
                }

                if (controllerGraphic.GetType().Name == "Bracket")
                {
                    Bracket controllerBracket = (Bracket)controllerGraphic;
                    JObject bracket = new JObject();
                    bracket.Add("originX", controllerBracket.OriginX);
                    bracket.Add("originY", controllerBracket.OriginY);
                    bracket.Add("width", controllerBracket.BracketWidth);
                    bracket.Add("height", controllerBracket.BracketHeight);
                    bracket.Add("competitors", controllerBracket.Competitors);
                    bracket.Add("offsetX", controllerBracket.OffsetX);
                    bracket.Add("offsetY", controllerBracket.OffsetY);
                    bracket.Add("lineColor", controllerBracket.Color.ToString());
                    bracket.Add("lineThickness", controllerBracket.Thickness);
                    bracket.Add("hideHorizontalLines", controllerBracket.HideHorizontal);
                    bracket.Add("hideVerticalLines", controllerBracket.HideVertical);
                    JObject font = new JObject();
                    font.Add("family", controllerBracket.ExampleLabel.FontFamily);
                    font.Add("size", controllerBracket.ExampleLabel.FontSize);
                    font.Add("weight", controllerBracket.ExampleLabel.FontWeight.ToString());
                    font.Add("style", controllerBracket.ExampleLabel.FontStyle.ToString());
                    font.Add("color", controllerBracket.ExampleLabel.FontColor);
                    font.Add("hasGlow", controllerBracket.ExampleLabel.HasGlow);
                    font.Add("glowColor", controllerBracket.ExampleLabel.GlowColor);
                    font.Add("align", controllerBracket.ExampleLabel.ContentAlign.ToString());
                    bracket.Add("font", font);

                    JArray bracketNames = new JArray();
                    for (int i = 0; i < controllerBracket.Names.Length; i++)
                    {
                        bracketNames.Add(controllerBracket.Names[i].Content.ToString());
                    }
                    bracket.Add("names", bracketNames);
                    JArray bracketScores = new JArray();
                    for (int i = 0; i < controllerBracket.Scores.Length; i++)
                    {
                        bracketScores.Add(controllerBracket.Scores[i].Content.ToString());
                    }
                    bracket.Add("scores", bracketScores);
                    graphic.Add("bracket", bracket);
                }
                else if (controllerGraphic.GetType().Name == "SequencePlayer")
                {
                    image.Add("sequenceInStartFrame", ((SequencePlayer)controllerGraphic).InStartFrame);
                    image.Add("sequenceLoopStartFrame", ((SequencePlayer)controllerGraphic).LoopStartFrame);
                    image.Add("sequenceOutStartFrame", ((SequencePlayer)controllerGraphic).OutStartFrame);
                }
                else if (controllerGraphic.GetType().Name == "ImageSwitcher")
                {
                    JObject imageSwitcher = new JObject();
                    ImageSwitcher controllerImageSwitcher = (ImageSwitcher)controllerGraphic;
                    if (copy)
                    {
                        imageSwitcher.Add("folder", controllerImageSwitcher.Folder);
                        imageSwitcher.Add("activeImage", controllerImageSwitcher.ActiveImage != null ? controllerImageSwitcher.ActiveImage : string.Empty);
                    }
                    else
                    {
                        imageSwitcher.Add("folder", controllerImageSwitcher.Folder != string.Empty ? @"Configs\" + folderName + @"\" + Path.GetFileName(controllerImageSwitcher.Folder) : string.Empty);
                        imageSwitcher.Add("activeImage", controllerImageSwitcher.Folder != string.Empty && controllerImageSwitcher.ActiveImage != null ?
                            @"Configs\" + folderName + @"\" + Path.GetFileName(controllerImageSwitcher.Folder) + @"\" + Path.GetFileName(controllerImageSwitcher.ActiveImage) : string.Empty);
                    }
                    imageSwitcher.Add("stretch", controllerImageSwitcher.SwitcherImage.Stretch.ToString());
                    graphic.Add("imageSwitcher", imageSwitcher);                   
                }
                else if (controllerGraphic.GetType().Name == "ImageSlider")
                {
                    graphic.Add("sliderDuration", ((ImageSlider)controllerGraphic).Duration);
                    graphic.Add("images", JArray.FromObject(((ImageSlider)controllerGraphic).ImageSources));
                }
                else if (controllerGraphic.GetType().Name == "SingleText" || controllerGraphic.GetType().Name == "DoubleText")
                {
                    if (controllerGraphic is SingleText)
                    {
                        graphic.Add("storage", JArray.FromObject(((SingleText)controllerGraphic).TextStorage));
                    }
                    else
                    {
                        graphic.Add("storage", JArray.FromObject(((DoubleText)controllerGraphic).TextStorage));
                    }
                }
                else if (controllerGraphic.GetType().Name == "Media")
                {
                    graphic.Add("source", ((Media)controllerGraphic).Source);
                    graphic.Add("duration", ((Media)controllerGraphic).Duration.TotalMilliseconds);
                    graphic.Add("volume", ((Media)controllerGraphic).Volume);
                    graphic.Add("loop", ((Media)controllerGraphic).Loop);
                }
                else if (controllerGraphic.GetType().Name == "WebSource")
                {
                    graphic.Add("url", ((WebSource)controllerGraphic).Url);
                }
                else if (controllerGraphic.GetType().Name == "Playlist")
                {
                    JArray playlistSources = new JArray();
                    foreach (Tuple<string, TimeSpan> source in ((Playlist)controller.Graphic).Sources)
                    {
                        JObject playlistSource = new JObject();
                        playlistSource.Add("file", source.Item1);
                        playlistSource.Add("duration", source.Item2.TotalMilliseconds);
                        playlistSources.Add(playlistSource);
                    }
                    graphic.Add("sources", playlistSources);
                    graphic.Add("volume", ((Playlist)controllerGraphic).Volume);
                    graphic.Add("loop", ((Playlist)controllerGraphic).Loop);
                }
            }
            JArray children = new JArray();
            foreach (GraphicController child in controller.Children)
            {
                children.Add(SerializeSingle(child, copy));
            }
            graphic.Add("children", children);
            return graphic;
        }

        public List<GraphicController> Deserialize(string json)
        {
            JObject jsonObject = JObject.Parse(json);
            List<GraphicController> rootList = new List<GraphicController>();
            JObject output = JObject.Parse(jsonObject["output"].ToString());
            Windows.OutputSettings.SetWidth(double.Parse(output["width"].ToString()));
            Windows.OutputSettings.SetHeight(double.Parse(output["height"].ToString()));
            Windows.Source.canvas.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(output["background"].ToString()));
            switch (Windows.Source.canvas.Background.ToString())
            {
                case "#FF008000":
                    Windows.OutputSettings.bgColor.Text = "Red";
                    break;

                case "#FFFF0000":
                    Windows.OutputSettings.bgColor.Text = "Blue";
                    break;

                case "#FF00FF00":
                    Windows.OutputSettings.bgColor.Text = "Green";
                    break;

                case "#00FFFFFF":
                    Windows.OutputSettings.bgColor.Text = "None";
                    break;

            }
            Windows.OutputSettings.SetFPS(double.Parse(output["fps"].ToString()), new List<GraphicController>());
            JArray graphics = JArray.Parse(jsonObject["graphics"].ToString());
            foreach (JObject graphic in graphics)
            {
                try
                {
                    rootList.Add(DeserializeSingle(graphic));
                }
                catch
                {
                    CustomMessageBox.Show("Failed to load graphic with type " + graphic["type"]);
                }
            }

            return rootList;
        }

        public GraphicController DeserializeSingle(JObject json, int depth = 0)
        {
            Graphic graphic = null;
            GraphicController controller = null;
            string type = json["type"].ToString();
            switch (type)
            {
                case "Scoreboard":
                    {
                        graphic = new Scoreboard(Windows.Source.canvas);
                        break;
                    }

                case "SingleText":
                    {
                        graphic = new SingleText(Windows.Source.canvas);
                        break;
                    }

                case "DoubleText":
                    {
                        graphic = new DoubleText(Windows.Source.canvas);
                        break;
                    }

                case "RichText":
                    {
                        graphic = new RichText(Windows.Source.canvas);
                        break;
                    }

                case "ImageOnly":
                    {
                        graphic = new ImageOnly(Windows.Source.canvas);
                        break;
                    }

                case "Countdown":
                    {
                        graphic = new Countdown(Windows.Source.canvas);
                        break;
                    }

                case "Bracket":
                    {
                        graphic = new Bracket(Windows.Source.canvas);
                        break;
                    }
                case "Group":
                    {
                        graphic = new Group(Windows.Source.canvas);
                        break;
                    }

                case "SequencePlayer":
                    {
                        graphic = new SequencePlayer(Windows.Source.canvas);
                        break;
                    }

                case "Media":
                    {
                        graphic = new Media(Windows.Source.canvas);
                        break;
                    }

                case "Clock":
                    {
                        graphic = new Clock(Windows.Source.canvas);
                        break;
                    }

                case "ImageSwitcher":
                    {
                        graphic = new ImageSwitcher(Windows.Source.canvas);
                        break;
                    }

                case "ImageSlider":
                    {
                        graphic = new ImageSlider(Windows.Source.canvas);
                        break;
                    }

                case "WebSource":
                    {
                        graphic = new WebSource(Windows.Source.canvas);
                        break;
                    }

                case "Playlist":
                    {
                        graphic = new Playlist(Windows.Source.canvas);
                        break;
                    }

                case "Blank":
                    {
                        controller = new Blank(null);
                        break;
                    }

                case "Peliliiga":
                    {
                        controller = new Peliliiga(null);
                        break;
                    }

                case "Xerberus":
                    {
                        controller = new Xerberus(null);
                        break;
                    }
            }
            if (graphic != null)
            {
                controller = graphic.Controller;
                controller.name.Content = json["name"].ToString();
                graphic.X = double.Parse(json["x"].ToString());
                graphic.Y = double.Parse(json["y"].ToString());
                graphic.GraphicWidth = double.Parse(json["width"].ToString());
                graphic.GraphicHeight = double.Parse(json["height"].ToString());

                JObject image = JObject.Parse(json["image"].ToString());
                graphic.Image.Stretch = (Stretch)Enum.Parse(typeof(Stretch), image["stretch"].ToString());
                if (image["uri"] != null)
                {
                    string uri = image["uri"].ToString();
                    Uri tempuri;
                    if (Uri.TryCreate(uri, UriKind.Absolute, out tempuri))
                    {
                        graphic.Image.UriSource = tempuri.ToString();
                    }
                    else
                    {
                        graphic.Image.UriSource = AppDomain.CurrentDomain.BaseDirectory + uri;
                    }
                    if (graphic.Image.IsSequence)
                    {
                        graphic.Image.SequenceFrames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + Path.GetDirectoryName(graphic.Image.UriSource), "*" + Path.GetExtension(graphic.Image.UriSource)).ToList();
                    }
                }
                graphic.Image.IsSequence = bool.Parse(image["isSequence"].ToString());

                JObject animationIn = JObject.Parse(json["animationIn"].ToString());
                graphic.AnimationIn.Style = (GraphicAnimation.AnimationStyle)Enum.Parse(typeof(GraphicAnimation.AnimationStyle), animationIn["style"].ToString());
                graphic.AnimationIn.AddFade = bool.Parse(animationIn["addFade"].ToString());
                graphic.AnimationIn.Duration = double.Parse(animationIn["duration"].ToString());
                graphic.AnimationIn.Delay = double.Parse(animationIn["delay"].ToString());
                graphic.AnimationIn.Ease = bool.Parse(animationIn["ease"].ToString());
                graphic.AnimationIn.EasePower = double.Parse(animationIn["easePower"].ToString());

                JObject animationOut = JObject.Parse(json["animationOut"].ToString());
                graphic.AnimationOut.Style = (GraphicAnimation.AnimationStyle)Enum.Parse(typeof(GraphicAnimation.AnimationStyle), animationOut["style"].ToString());
                graphic.AnimationOut.AddFade = bool.Parse(animationOut["addFade"].ToString());
                graphic.AnimationOut.Duration = double.Parse(animationOut["duration"].ToString());
                graphic.AnimationOut.Delay = double.Parse(animationOut["delay"].ToString());
                graphic.AnimationOut.Ease = bool.Parse(animationOut["ease"].ToString());
                graphic.AnimationOut.EasePower = double.Parse(animationOut["easePower"].ToString());

                if (graphic.Texts != null)
                {
                    JArray texts = JArray.Parse(json["texts"].ToString());
                    for (int i = 0; i < texts.Count; i++)
                    {
                        graphic.Texts[i].Content = texts[i]["content"].ToString();
                        graphic.Texts[i].X = double.Parse(texts[i]["x"].ToString());
                        graphic.Texts[i].Y = double.Parse(texts[i]["y"].ToString());
                        graphic.Texts[i].Width = double.Parse(texts[i]["width"].ToString());
                        graphic.Texts[i].LineHeight = double.Parse(texts[i]["lineHeight"].ToString());

                        JObject font = JObject.Parse(texts[i]["font"].ToString());
                        graphic.Texts[i].FontFamily = font["family"].ToString();
                        graphic.Texts[i].FontSize = double.Parse(font["size"].ToString());
                        graphic.Texts[i].FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(font["weight"].ToString());
                        graphic.Texts[i].FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(font["style"].ToString());
                        graphic.Texts[i].FontColor = font["color"].ToString();
                        graphic.Texts[i].HasGlow = bool.Parse(font["hasGlow"].ToString());
                        graphic.Texts[i].GlowColor = font["glowColor"].ToString();
                        graphic.Texts[i].ContentAlign = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), font["align"].ToString());
                    }
                }

                if (type == "Bracket")
                {
                    Bracket bracket = (Bracket)graphic;
                    JObject jsonBracket = JObject.Parse(json["bracket"].ToString());
                    bracket.OriginX = double.Parse(jsonBracket["originX"].ToString());
                    bracket.OriginY = double.Parse(jsonBracket["originY"].ToString());
                    bracket.BracketWidth = double.Parse(jsonBracket["width"].ToString());
                    bracket.BracketHeight = double.Parse(jsonBracket["height"].ToString());
                    bracket.Competitors = int.Parse(jsonBracket["competitors"].ToString());
                    bracket.OffsetX = double.Parse(jsonBracket["offsetX"].ToString());
                    bracket.OffsetY = double.Parse(jsonBracket["offsetY"].ToString());
                    bracket.Color = new BrushConverter().ConvertFromString(jsonBracket["lineColor"].ToString()) as Brush;
                    bracket.Thickness = double.Parse(jsonBracket["lineThickness"].ToString());
                    bracket.HideHorizontal = bool.Parse(jsonBracket["hideHorizontalLines"].ToString());
                    bracket.HideVertical = bool.Parse(jsonBracket["hideVerticalLines"].ToString());

                    JObject jsonBracketFont = JObject.Parse(jsonBracket["font"].ToString());
                    bracket.ExampleLabel.FontFamily = jsonBracketFont["family"].ToString();
                    bracket.ExampleLabel.FontSize = double.Parse(jsonBracketFont["size"].ToString());
                    bracket.ExampleLabel.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(jsonBracketFont["weight"].ToString());
                    bracket.ExampleLabel.FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(jsonBracketFont["style"].ToString());
                    bracket.ExampleLabel.FontColor = jsonBracketFont["color"].ToString();
                    bracket.ExampleLabel.HasGlow = bool.Parse(jsonBracketFont["hasGlow"].ToString());
                    bracket.ExampleLabel.GlowColor = jsonBracketFont["glowColor"].ToString();
                    bracket.ExampleLabel.ContentAlign = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), jsonBracketFont["align"].ToString());

                    JArray bracketNames = JArray.Parse(jsonBracket["names"].ToString());
                    for (int i = 0; i < bracketNames.Count; i++)
                    {
                        bracket.Names[i].Content = bracketNames[i].ToString();
                    }
                    JArray bracketScores = JArray.Parse(jsonBracket["scores"].ToString());
                    for (int i = 0; i < bracketScores.Count; i++)
                    {
                        bracket.Scores[i].Content = bracketScores[i].ToString();
                    }
                    bracket.Redraw();
                }
                else if (type == "SequencePlayer")
                {
                    SequencePlayer sequence = (SequencePlayer)graphic;
                    sequence.InStartFrame = int.Parse(image["sequenceInStartFrame"].ToString());
                    sequence.LoopStartFrame = int.Parse(image["sequenceInStartFrame"].ToString());
                    sequence.OutStartFrame = int.Parse(image["sequenceInStartFrame"].ToString());
                }
                else if (type == "ImageSwitcher")
                {
                    ImageSwitcher imageSwitcher = (ImageSwitcher)graphic;
                    JObject jsonImageSwitcher = JObject.Parse(json["imageSwitcher"].ToString());
                    string folder = jsonImageSwitcher["folder"].ToString();
                    if (folder != string.Empty)
                    {
                        Uri tempuri;
                        if (Uri.TryCreate(folder, UriKind.Absolute, out tempuri))
                        {
                            imageSwitcher.Folder = folder.ToString();
                        }
                        else
                        {
                            imageSwitcher.Folder = AppDomain.CurrentDomain.BaseDirectory + folder;
                        }
                    }
                    string activeImage = jsonImageSwitcher["activeImage"].ToString();
                    if (activeImage != string.Empty)
                    {
                        Uri tempuri;
                        if (Uri.TryCreate(folder, UriKind.Absolute, out tempuri))
                        {
                            imageSwitcher.ActiveImage = activeImage;
                        }
                        else
                        {
                            imageSwitcher.ActiveImage = AppDomain.CurrentDomain.BaseDirectory + activeImage;
                        }
                    }
                    imageSwitcher.SwitcherImage.Stretch = (Stretch)Enum.Parse(typeof(Stretch), jsonImageSwitcher["stretch"].ToString());
                }
                else if (type == "ImageSlider")
                {
                    ImageSlider imageSlider = (ImageSlider)graphic;
                    imageSlider.Duration = int.Parse(json["sliderDuration"].ToString());
                    imageSlider.ImageSources = JArray.Parse(json["images"].ToString()).ToObject<List<string>>();
                }
                else if (type == "SingleText" || type == "DoubleText")
                {
                    if (graphic is SingleText)
                    {
                        ((SingleText)graphic).TextStorage = JArray.Parse(json["storage"].ToString()).ToObject<string[]>();
                    }
                    else
                    {
                        ((DoubleText)graphic).TextStorage = JArray.Parse(json["storage"].ToString()).ToObject<string[]>();
                    }
                }
                else if (type == "Media")
                {
                    ((Media)graphic).Source = json["source"].ToString();
                    double duration = double.Parse(json["duration"].ToString());
                    if (duration > 0)
                    {
                        ((Media)graphic).Duration = TimeSpan.FromMilliseconds(duration);
                        controller.info.Content = string.Format("Duration: {0:00}:{1:00}", (((Media)graphic).Duration.Hours * 60) + ((Media)graphic).Duration.Minutes, ((Media)graphic).Duration.Seconds);
                    }
                    ((Media)graphic).Volume = double.Parse(json["volume"].ToString());
                    ((Media)graphic).Loop = bool.Parse(json["loop"].ToString());
                }
                else if (type == "WebSource")
                {
                    ((WebSource)graphic).Url = json["url"].ToString();
                }
                else if (type == "Playlist")
                {
                    JArray playlistSources = JArray.Parse(json["sources"].ToString());
                    foreach (JObject source in playlistSources)
                    {
                        ((Playlist)graphic).Sources.Add(Tuple.Create(source["file"].ToString(), TimeSpan.FromMilliseconds(double.Parse(source["duration"].ToString()))));
                    }
                    ((Playlist)graphic).Volume = double.Parse(json["volume"].ToString());
                    ((Playlist)graphic).Loop = bool.Parse(json["loop"].ToString());
                    if (((Playlist)graphic).Sources.Count == 0)
                    {
                        controller.info.Content = string.Empty;
                    }
                    else
                    {
                        TimeSpan total = TimeSpan.Zero;
                        foreach (Tuple<string, TimeSpan> source in ((Playlist)graphic).Sources)
                        {
                            total = total.Add(source.Item2);
                        }
                        controller.info.Content = string.Format("Duration: {0:00}:{1:00}", (total.Hours * 60) + total.Minutes, total.Seconds);
                    }
                }
            }
            else if (controller != null)
            {
                controller.name.Content = json["name"].ToString();
                if (type == "Peliliiga")
                {
                    Peliliiga peliliiga = (Peliliiga)controller;
                    peliliiga.Url = json["url"].ToString();
                    peliliiga.Store = json["store"].ToString();
                    peliliiga.Interval = double.Parse(json["interval"].ToString());
                }
                else if (type == "Xerberus")
                {
                    Xerberus xerberus = (Xerberus)controller;
                    xerberus.Url = json["url"].ToString();
                    xerberus.Store = json["store"].ToString();
                    xerberus.Interval = double.Parse(json["interval"].ToString());
                    Xerberus.ClientId = json["clientId"].ToString();
                    Xerberus.ClientSecret = json["clientSecret"].ToString();
                }
            }
            else
            {
                throw new Exception("Controller with type " + json["type"] + " not found");
            }
            controller.Id = int.Parse(json["id"].ToString());
            JArray children = (JArray)json["children"];
            if (children.Count > 0)
            {
                controller.childrenVisibleArrow.Visibility = Visibility.Visible;
            }
            foreach (JObject child in children)
            {
                GraphicController childController = DeserializeSingle(child, depth + 1);
                childController.ControllerParent = controller;
                GraphicController.rootList.Remove(childController);
                controller.Children.Add(childController);
            }
            if (depth > 0)
            {
                controller.Expanded = false;
            }
            return controller;
        }
    }
}
