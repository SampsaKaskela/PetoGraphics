using PetoGraphics.Helpers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetoGraphics
{
    /// <summary>
    /// Interaction logic for FontPicker.xaml
    /// </summary>
    public partial class FontPicker : Window
    {
        public FontPicker()
        {
            InitializeComponent();
            List<string> fontlist = new List<string>();
            foreach (FontFamily font in Fonts.SystemFontFamilies)
            {
                fontlist.Add(font.ToString());
            }
            fontlist.Sort();
            fonts.ItemsSource = fontlist;
        }

        public GraphicText Text { get; set; }

        public bool BracketRedraw { get; set; } = false;

        private void FontPicker_Loaded(object sender, RoutedEventArgs e)
        {
            fonts.Text = Text.FontFamily;
            size.Text = Text.FontSize.ToString();
            color.Text = Text.FontColor;
            example.FontFamily = new FontFamily(Text.FontFamily);
            example.FontWeight = Text.FontWeight;
            example.FontStyle = Text.FontStyle;
            BrushConverter converter = new BrushConverter();
            example.Foreground = (Brush)converter.ConvertFromString(Text.FontColor);
        }

        private void Fonts_Changed(object sender, SelectionChangedEventArgs e)
        {
            example.FontFamily = new FontFamily(fonts.SelectedItem.ToString());
            Text.FontFamily = fonts.SelectedItem.ToString();
            if (BracketRedraw)
            {
                ((Bracket)GraphicController.Selected.Graphic).Redraw();
            }
        }

        private void Size_LostFocus(object sender, RoutedEventArgs e)
        {
            Validators.PositiveInteger(size, (num) =>
            {
                Text.FontSize = num;
                if (BracketRedraw)
                {
                    ((Bracket)GraphicController.Selected.Graphic).Redraw();
                }
            });
        }

        private void Bold_Click(object sender, RoutedEventArgs e)
        {
            if (example.FontWeight == FontWeights.Bold)
            {
                example.FontWeight = FontWeights.Normal;
                Text.FontWeight = FontWeights.Normal;
            }
            else
            {
                example.FontWeight = FontWeights.Bold;
                Text.FontWeight = FontWeights.Bold;
            }
            if (BracketRedraw)
            {
                ((Bracket)GraphicController.Selected.Graphic).Redraw();
            }
        }

        private void Style_Changed(object sender, RoutedEventArgs e)
        {
            if (example.FontStyle == FontStyles.Italic)
            {
                example.FontStyle = FontStyles.Normal;
                Text.FontStyle = FontStyles.Normal;
            }
            else
            {
                example.FontStyle = FontStyles.Italic;
                Text.FontStyle = FontStyles.Italic;
            }
            if (BracketRedraw)
            {
                ((Bracket)GraphicController.Selected.Graphic).Redraw();
            }
        }

        private void Color_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                example.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color.Text));
                Text.FontColor = color.Text;
                if (BracketRedraw)
                {
                    ((Bracket)GraphicController.Selected.Graphic).Redraw();
                }
            }
            catch { };
        }
    }
}
