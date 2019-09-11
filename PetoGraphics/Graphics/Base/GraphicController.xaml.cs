using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PetoGraphics
{
    public partial class GraphicController : UserControl
    {
        public static GraphicController copy;
        public static bool dragging = false;
        public static DispatcherTimer holdTimer;
        public static Point mouseStartPoint;
        public static GraphicController potentialParent = null;
        public static List<GraphicController> rootList = new List<GraphicController>(10);

        private static Random random = new Random();
        private static GraphicController selected;

        private bool active = true;
        private bool expanded = true;

        public GraphicController(Graphic graphic)
        {
            InitializeComponent();
            Graphic = graphic;
            int count = ControllerHelpers.GetVisibleControllerCount(rootList);
            Margin = new Thickness(0, count * 46, 0, 0);
            Windows.Main.controllerContainer.Children.Add(this);
            Panel.SetZIndex(this, 0);
            do
            {
                Id = random.Next(1, 10000);
            } while (HasControllerWithId(rootList));

            if (graphic is Media || graphic is Playlist)
            {
                button.ClearValue(BackgroundProperty);
            }

            rootList.Add(this);
            ControllerHelpers.CheckResize();
        }

        public static event EventHandler<GraphicController> OnSelectedChanged;

        public static GraphicController Selected
        {
            get { return selected; }
            set
            {
                // Remove focus so value is updated before change
                if (Keyboard.FocusedElement is TextBox || Keyboard.FocusedElement is RichTextBox)
                {
                    Keyboard.FocusedElement.RaiseEvent(new RoutedEventArgs(LostFocusEvent, Keyboard.FocusedElement));
                }
                if (selected != null)
                {
                    selected.grid.ClearValue(BackgroundProperty);
                    selected.border.ClearValue(BorderBrushProperty);
                    Panel.SetZIndex(selected, 0);
                }
                selected = value;
                if (selected != null)
                {
                    selected.border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF51"));
                    selected.grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4C00A425"));
                    Panel.SetZIndex(selected, 1);
                }
                OnSelectedChanged?.Invoke(null, selected);
            }
        }

        public int Id { get; set; }

        public GraphicController ControllerParent { get; set; }

        public Graphic Graphic { get; private set; }

        public string ShowText { get; set; } = "Show";

        public string HideText { get; set; } = "Hide";

        public List<GraphicController> Children { get; } = new List<GraphicController>();

        public bool Active
        {
            get { return active; }
            set
            {
                if (value != active)
                {
                    ToggleGraphic(null, null);
                }
                active = value;
            }
        }

        public bool Expanded
        {
            get { return expanded; }
            set
            {
                expanded = value;
                if (value)
                {
                    ShowChildren();
                }
                else
                {
                    HideChildren();
                }
                ControllerHelpers.RepositionControls(rootList);
                ControllerHelpers.CheckResize();
            }
        }

        // Dragging now
        public static void Dragging_Tick(object sender, EventArgs e)
        {
            dragging = true;
            Selected.Opacity = 0.7;
            Selected.IsHitTestVisible = false;
            mouseStartPoint = Mouse.GetPosition(Windows.Main.controllerContainer);
            holdTimer.Stop();
        }

        public bool CalculatePosition(List<GraphicController> list, GraphicController parentcontroller)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != this)
                {
                    if (Margin.Top < list[i].Margin.Top)
                    {
                        int previousindex = list.IndexOf(this);
                        RemovePreviousParent();
                        if (i > list.Count)
                        {
                            i = list.Count;
                        }
                        if (previousindex > 0 && previousindex < i)
                        {
                            list.Insert(i - 1, this);
                        }
                        else
                        {
                            list.Insert(i, this);
                        }
                        ControllerParent = parentcontroller;
                        return true;
                    }
                    if (list[i].expanded && CalculatePosition(list[i].Children, list[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Copy()
        {
            GraphicSerializer serializer = new GraphicSerializer();
            JObject copy = serializer.SerializeSingle(this, true);
            serializer.DeserializeSingle(copy);
            ControllerHelpers.CheckResize();
            ControllerHelpers.RepositionControls(rootList);
        }

        // Clear all controls so they can be freed
        public void Delete()
        {
            if (Graphic != null)
            {
                Graphic.Remove();
            }
            int parentIndex = 0;
            if (ControllerParent == null)
            {
                parentIndex = rootList.IndexOf(this);
            }
            else
            {
                parentIndex = ControllerParent.Children.IndexOf(this);
            }
            foreach (GraphicController child in Children)
            {
                child.ControllerParent = ControllerParent;
                if (child.ControllerParent != null)
                {
                    child.ControllerParent.Children.Insert(parentIndex, child);
                    if (ControllerParent.expanded)
                    {
                        child.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        child.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    rootList.Insert(parentIndex, child);
                }
                parentIndex++;
            }
            Children.Clear();
            grid.Children.Clear();
            Windows.Main.controllerContainer.Children.Remove(this);
            if (ControllerParent == null)
            {
                rootList.Remove(this);
            }
            else
            {
                ControllerParent.Children.Remove(this);
                if (ControllerParent.Children.Count == 0)
                {
                    ControllerParent.childrenHiddenArrow.Visibility = Visibility.Collapsed;
                    ControllerParent.childrenVisibleArrow.Visibility = Visibility.Collapsed;
                }
            }
            if (copy == this)
            {
                copy = null;
            }
            ControllerHelpers.CheckResize();
            ControllerHelpers.RepositionControls(rootList);
        }

        public bool HasChildren(GraphicController controller)
        {
            foreach (GraphicController child in Children)
            {
                if (child == controller)
                {
                    return true;
                }
                else if (child.HasChildren(controller))
                {
                    return true;
                }
            }
            return false;
        }

        public void HideChildren()
        {
            if (Children.Count == 0)
            {
                childrenHiddenArrow.Visibility = Visibility.Collapsed;
                childrenVisibleArrow.Visibility = Visibility.Collapsed;
                return;
            }
            childrenVisibleArrow.Visibility = Visibility.Collapsed;
            childrenHiddenArrow.Visibility = Visibility.Visible;
            foreach (GraphicController child in Children)
            {
                child.Visibility = Visibility.Collapsed;
                child.HideChildren();
            }
        }

        public void RemovePreviousParent()
        {
            if (ControllerParent == null)
            {
                rootList.Remove(this);
            }
            else
            {
                ControllerParent.Children.Remove(this);
                if (ControllerParent.Children.Count == 0)
                {
                    ControllerParent.childrenVisibleArrow.Visibility = Visibility.Collapsed;
                    ControllerParent.childrenHiddenArrow.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void SetParent()
        {
            // This can cause problems if user is trying to add this controller as child of one of it's children so don't allow that
            if (HasChildren(potentialParent))
            {
                return;
            }
            RemovePreviousParent();
            ControllerParent = potentialParent;
            if (ControllerParent == null)
            {
                rootList.Add(this);
            }
            else
            {
                ControllerParent.border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1B1B1C"));
                ControllerParent.Children.Add(this);
                if (ControllerParent.expanded)
                {
                    ControllerParent.childrenVisibleArrow.Visibility = Visibility.Visible;
                }
                else
                {
                    ControllerParent.childrenHiddenArrow.Visibility = Visibility.Visible;
                }
                potentialParent = null;
            }
        }

        public void ShowChildren()
        {
            if (Children.Count == 0)
            {
                childrenHiddenArrow.Visibility = Visibility.Collapsed;
                childrenVisibleArrow.Visibility = Visibility.Collapsed;
                return;
            }
            childrenHiddenArrow.Visibility = Visibility.Collapsed;
            childrenVisibleArrow.Visibility = Visibility.Visible;
            foreach (GraphicController child in Children)
            {
                child.Visibility = Visibility.Visible;
                if (child.expanded)
                {
                    child.ShowChildren();
                }
            }
        }

        public void ToggleChilren()
        {
            foreach (GraphicController child in Children)
            {
                if (child.active == active)
                {
                    child.Active = !active;
                }
            }
        }

        public void ToggleGraphic(object sender, RoutedEventArgs e)
        {
            ToggleChilren();
            if (active)
            {
                button.Content = ShowText;
                button.ClearValue(BackgroundProperty);
                active = false;
                if (Graphic != null)
                {
                    Graphic.Hide();
                }
            }
            else
            {
                button.Content = HideText;
                button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF51"));
                active = true;
                if (Graphic != null)
                {
                    Graphic.Show();
                }
            }
        }

        private void ChildrenHideArrow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Expanded = false;
        }

        private void ChildrenToggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Expanded = true;
        }

        private void Controller_MouseEnter(object sender, MouseEventArgs e)
        {
            if (dragging && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                potentialParent = this;
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F00FF3A"));
            }
        }

        private void Controller_MouseLeave(object sender, MouseEventArgs e)
        {
            if (dragging && this != Selected)
            {
                if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && potentialParent == this)
                {
                    potentialParent = null;
                }
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1B1B1C"));
            }
        }

        // Start drag timer
        private void Controller_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Selected = this;
            holdTimer.Start();
        }

        private bool HasControllerWithId(List<GraphicController> list)
        {
            foreach(GraphicController child in list)
            {
                if (child.Id == Id || HasControllerWithId(child.Children))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
