using System.Windows;

namespace PetoGraphics
{
    public partial class Source : Window
    {
        public bool Editing { get; set; } = false; 

        public Source()
        {
            InitializeComponent();
        }

        public void ToggleEditing()
        {
            if (Editing)
            {
                DisableEditing();
                Editing = false;
            }
            else
            {
                EnableEditing();
                Editing = true;
            }
        }

        private void EnableEditing()
        {
            foreach (GraphicController controller in GraphicController.rootList)
            {
                if (controller.Graphic != null)
                {
                    controller.Graphic.editor.Visibility = Visibility.Visible;
                }
                EnableEditing(controller);
            }
        }

        private void EnableEditing(GraphicController controller)
        {
            foreach (GraphicController child in controller.Children)
            {
                if (child.Graphic != null)
                {
                    child.Graphic.editor.Visibility = Visibility.Visible;
                }
                EnableEditing(child);
            }
        }

        private void DisableEditing()
        {
            foreach (GraphicController controller in GraphicController.rootList)
            {
                if (controller.Graphic != null)
                {
                    controller.Graphic.editor.Visibility = Visibility.Collapsed;
                }
                DisableEditing(controller);
            }
        }

        private void DisableEditing(GraphicController controller)
        {
            foreach (GraphicController child in controller.Children)
            {
                if (child.Graphic != null)
                {
                    child.Graphic.editor.Visibility = Visibility.Collapsed;
                }
                DisableEditing(child);
            }
        }

        private void Source_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
