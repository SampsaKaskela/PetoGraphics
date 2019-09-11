using System.IO;
using System.Windows.Controls;

namespace PetoGraphics
{
    public class ImageSwitcher : Graphic
    {
        private string folder = string.Empty;

        public ImageSwitcher(Canvas sourceCanvas) : base(sourceCanvas)
        {
            Controller.name.Content = "ImageSwitcher";

            SwitcherImage = new GraphicImage(container);

            GraphicHeight = 500;
            GraphicWidth = 500;
            X = 200;
            Y = 200;
        }

        public override double GraphicWidth
        {
            get { return base.GraphicWidth; }
            set
            {
                base.GraphicWidth = value;
                SwitcherImage.Width = value;
            }
        }

        public override double GraphicHeight
        {
            get { return base.GraphicHeight; }
            set
            {
                base.GraphicHeight = value;
                SwitcherImage.Height = value;
            }
        }

        public GraphicImage SwitcherImage { get; private set; }

        public string Folder
        {
            get { return folder; }
            set
            {
                folder = value;
                if (Directory.Exists(folder))
                {
                    Images = Directory.GetFiles(value, "*.*", SearchOption.AllDirectories);
                }
                else
                {
                    Images = new string[0];
                }
            }
        }

        public string[] Images { get; private set; } = new string[0];

        public string ActiveImage
        {
            get { return SwitcherImage.UriSource; }
            set { SwitcherImage.UriSource = value; }
        }
    }
}
