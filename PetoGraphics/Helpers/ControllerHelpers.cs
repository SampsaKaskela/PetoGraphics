using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;

namespace PetoGraphics
{
    class ControllerHelpers
    {
        public static GraphicController FindPreviousVisibleFrom(GraphicController controller)
        {
            int index = 0;
            if (controller.ControllerParent == null)
            {
                index = GraphicController.rootList.IndexOf(controller);
                if (index != 0)
                {
                    // Move to previous controller that is visible
                    if (GraphicController.rootList[index - 1].Children.Count > 0 && GraphicController.rootList[index - 1].Expanded)
                    {
                        return FindLastVisible(GraphicController.rootList[index - 1].Children);
                    }
                    else
                    {
                        return GraphicController.rootList[index - 1];
                    }
                }
                // Controller is first element in tree
                return null;
            }
            else
            {
                index = controller.ControllerParent.Children.IndexOf(controller);
                if (index == 0)
                {
                    return controller.ControllerParent;
                }
                else
                {
                    return controller.ControllerParent.Children[index - 1];
                }
            }
        }

        public static GraphicController FindNextVisibleFrom(GraphicController controller, bool ignoreChildren = false)
        {
            int index = 0;
            if (controller.ControllerParent == null)
            {
                index = GraphicController.rootList.IndexOf(controller);
            }
            else
            {
                index = controller.ControllerParent.Children.IndexOf(controller);
            }
            if (!ignoreChildren && controller.Expanded && controller.Children.Count > 0)
            {
                return controller.Children[0];
            }
            else
            {
                if (controller.ControllerParent == null)
                {
                    if (index < GraphicController.rootList.Count - 1)
                    {
                        return GraphicController.rootList[index + 1];
                    }
                }
                else
                {
                    if (index < controller.ControllerParent.Children.Count - 1)
                    {
                        return controller.ControllerParent.Children[index + 1];
                    }
                    return FindNextVisibleFrom(controller.ControllerParent, true);
                }
            }
            // Controller is last element in tree
            return null;
        }

        public static GraphicController FindLastVisible(List<GraphicController> list)
        {
            if (list.Count > 0)
            {
                if (list[list.Count - 1].Children.Count > 0)
                {
                    if (list[list.Count - 1].Expanded && list[list.Count - 1] != GraphicController.Selected)
                    {
                        return FindLastVisible(list[list.Count - 1].Children);
                    }
                }
                return list[list.Count - 1];
            }
            return null;
        }

        // Repositions controls to proper locations
        public static int[] RepositionControls(List<GraphicController> list, int index = 0, int deep = 0, int hidden = 0, bool visible = true)
        {
            for (int i = 0; i < list.Count; i++)
            {
                // Set Graphic Z-indexes
                if (list[i].Graphic != null)
                {
                    Panel.SetZIndex(list[i].Graphic, index);
                }

                // Calcuate and set position of current controller if it is visible
                if (!visible)
                {
                    hidden++;
                }
                else
                {
                    list[i].Margin = new Thickness(deep * 10, (index - hidden) * 46, 0, 0);
                }
                index++;

                // Position children if they are visible
                int[] array = null;
                if (!visible || !list[i].Expanded)
                {
                    array = RepositionControls(list[i].Children, index, deep + 1, hidden, false);
                }
                else
                {
                    array = RepositionControls(list[i].Children, index, deep + 1, hidden, true);
                }

                // Update values
                index = array[0];
                hidden = array[1];
            }
            return new int[2] { index, hidden };
        }

        public static int GetVisibleControllerCount(List<GraphicController> list)
        {
            int count = 0;
            foreach (GraphicController controller in list)
            {
                count++;
                if (controller.Expanded)
                {
                    count += GetVisibleControllerCount(controller.Children);
                }
            }
            return count;
        }

        public static void CheckResize()
        {
            if (GetVisibleControllerCount(GraphicController.rootList) > 13)
            {
                SetControllerWidth(GraphicController.rootList, 582);
            }
            else
            {
                SetControllerWidth(GraphicController.rootList, 600);
            }
        }

        private static void SetControllerWidth(List<GraphicController> list, double width, int deep = 0)
        {
            foreach (GraphicController controller in list)
            {
                controller.border.Width = width - deep * 10;
                SetControllerWidth(controller.Children, width, deep + 1);
            }
        }
    }
}
