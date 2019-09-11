using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PetoGraphics.UI.Content
{
    /// <summary>
    /// Interaction logic for RichTextUI.xaml
    /// </summary>
    public partial class RichTextUI : UserControl
    {
        public RichTextUI()
        {
            InitializeComponent();
            GraphicController.OnSelectedChanged += OnSelectedChanged;
        }

        private void OnSelectedChanged(object sender, GraphicController selected)
        {
            if (selected != null && selected.Graphic is RichText)
            {
                Visibility = Visibility.Visible;
                FlowDocument doc = new FlowDocument();
                doc.Blocks.Add(new Paragraph(new Run(selected.Graphic.Texts[0].Content)));
                text.Document = doc;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void Text_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GraphicController.Selected != null && GraphicController.Selected.Graphic is RichText)
            {
                TextRange textRange = new TextRange(text.Document.ContentStart, text.Document.ContentEnd);
                GraphicController.Selected.Graphic.Texts[0].Content = textRange.Text;
            }
        }
    }
}
