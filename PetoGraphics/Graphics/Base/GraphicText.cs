using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Effects;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using PetoGraphics.Providers;

namespace PetoGraphics
{
    public class GraphicText
    {
        private string inputContent = "";
        private Label label = new Label();

        public GraphicText(Grid container)
        {
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;
            label.HorizontalContentAlignment = HorizontalAlignment.Left;
            label.VerticalContentAlignment = VerticalAlignment.Top;
            label.Height = 1000;
            label.Foreground = Brushes.White;
            label.FontFamily = new FontFamily("Roboto");
            DropShadowEffect effect = new DropShadowEffect();
            effect.BlurRadius = 10;
            effect.Color = Colors.White;
            effect.ShadowDepth = 0;
            effect.Opacity = 0;
            label.Effect = effect;
            container.Children.Add(label);
            Store.Instance.OnUpdate += StoreChanged;
        }

        public string ComputedContent
        {
            get
            {
                if (Rich)
                {
                    return ((AccessText)label.Content).Text;
                }
                else
                {
                    return label.Content.ToString();
                };
            }
        }

        public string Content
        {
            get { return inputContent; }
            set
            {
                inputContent = value;
                ComputeContent(value, Store.Instance.Data);
            }
        }

        public HorizontalAlignment ContentAlign
        {
            get { return label.HorizontalContentAlignment; }
            set
            {
                label.HorizontalContentAlignment = value;
                if (Rich)
                {
                    switch(value)
                    {
                        case HorizontalAlignment.Left:
                            {
                                ((AccessText)label.Content).TextAlignment = TextAlignment.Left;
                                break;
                            }

                        case HorizontalAlignment.Right:
                            {
                                ((AccessText)label.Content).TextAlignment = TextAlignment.Right;
                                break;
                            }

                        case HorizontalAlignment.Center:
                            {
                                ((AccessText)label.Content).TextAlignment = TextAlignment.Center;
                                break;
                            }
                    }
                }
            }
        }

        public string FontColor
        {
            get { return label.Foreground.ToString(); }
            set
            {
                try
                {
                    BrushConverter converter = new BrushConverter();
                    label.Foreground = (Brush)converter.ConvertFromString(value);
                }
                catch
                {
                    CustomMessageBox.Show("Not valid Hex.");
                }
            }
        }

        public string FontFamily
        {
            get { return label.FontFamily.ToString(); }
            set { label.FontFamily = new FontFamily(value); }
        }

        public double FontSize
        {
            get { return label.FontSize; }
            set { label.FontSize = value; }
        }

        public FontStyle FontStyle
        {
            get { return label.FontStyle; }
            set { label.FontStyle = value; }
        }

        public FontWeight FontWeight
        {
            get { return label.FontWeight; }
            set { label.FontWeight = value; }
        }

        public string GlowColor
        {
            get { return ((DropShadowEffect)label.Effect).Color.ToString(); }
            set
            {
                try
                {
                    ((DropShadowEffect)label.Effect).Color = (Color)ColorConverter.ConvertFromString(value);
                }
                catch
                {
                    CustomMessageBox.Show("Not valid Hex.");
                }
            }
        }

        public bool HasGlow
        {
            get { return ((DropShadowEffect)label.Effect).Opacity == 1; }
            set
            {
                if (value)
                {
                    ((DropShadowEffect)label.Effect).Opacity = 1;
                }
                else
                {
                    ((DropShadowEffect)label.Effect).Opacity = 0;
                }
            }
        }

        public double LineHeight
        {
            get
            {
                if (Rich)
                {
                    return ((AccessText)label.Content).LineHeight;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (Rich)
                {
                    ((AccessText)label.Content).LineHeight = value;
                }
            }
        }

        public string Name
        {
            get { return label.Name; }
            set { label.Name = value; }
        }

        public bool Rich
        {
            get { return label.Content is AccessText; }
            set
            {
                if (value)
                {
                    AccessText accesstext = new AccessText();
                    accesstext.TextWrapping = TextWrapping.Wrap;
                    label.Content = accesstext;
                }
                else
                {
                    label.Content = "";
                }
            }
        }

        public Visibility Visibility
        {
            get { return label.Visibility; }
            set { label.Visibility = value; }
        }

        public double Width
        {
            get { return label.Width; }
            set { label.Width = value; }
        }

        public double X
        {
            get { return label.Margin.Left; }
            set { label.Margin = new Thickness(value, label.Margin.Top, 0, 0); }
        }

        public double Y
        {
            get { return label.Margin.Top; }
            set { label.Margin = new Thickness(label.Margin.Left, value, 0, 0); }
        }

        private void ComputeContent(string text, JObject data)
        {
            if (text == null)
            {
                label.Content = string.Empty;
                return;
            }
            string computedText = text;
            MatchCollection matches = Regex.Matches(computedText, @"\$\{(.*?)\}");
            foreach (Match match in matches)
            {
                JToken storeValue = data.SelectToken(match.Groups[1].Value);
                computedText = computedText.Replace(match.Value, storeValue != null ? storeValue.ToString() : "");
            }
            if (Rich)
            {
                ((AccessText)label.Content).Text = computedText;
            }
            else
            {
                label.Content = computedText;
            }
        }

        private void StoreChanged(object sender, JObject data)
        {
            ComputeContent(inputContent, data);
        }
    }
}
