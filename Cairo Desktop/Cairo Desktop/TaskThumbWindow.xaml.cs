using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using CairoDesktop.Interop;
using CairoDesktop.WindowsTasks;

namespace CairoDesktop
{
    /// <summary>
    /// Interaction logic for TaskThumbWindow.xaml
    /// </summary>
    public partial class TaskThumbWindow : Window
    {
        private TaskButton taskButton;
        private bool isClosing;
        private bool isAnimating;
        private IntPtr handle;

        public TaskThumbWindow(TaskButton parent)
        {
            InitializeComponent();

            taskButton = parent;
            DataContext = parent.Window;

            taskButton.SetParentAutoHide(false);

            pnlTitle.Margin = new Thickness(0);
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            // hide from alt-tab
            WindowInteropHelper helper = new WindowInteropHelper(this);
            handle = helper.Handle;
            Shell.HideWindowFromTasks(handle);

            // get anchor point
            Point taskButtonPoint = taskButton.GetThumbnailAnchor();

            if (Configuration.Settings.Instance.TaskbarPosition == 1)
            {
                // taskbar on top
                Top = taskButtonPoint.Y + taskButton.ActualHeight;

                bdrThumb.Style = Application.Current.FindResource("TaskThumbWindowBorderTopStyle") as Style;
                bdrThumbInner.Style = Application.Current.FindResource("TaskThumbWindowInnerBorderTopStyle") as Style;

                bdrTranslate.Y *= -1;

                ToolTipService.SetPlacement(this, System.Windows.Controls.Primitives.PlacementMode.Bottom);
            }
            else
            {
                Top = taskButtonPoint.Y - ActualHeight;
            }

            Left = taskButtonPoint.X - ((ActualWidth - taskButton.ActualWidth) / 2);
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // runs once per frame for the duration of the animation
            if (isAnimating)
            {
            }
            else
            {
                System.Windows.Media.CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isClosing = true;
            taskButton.ThumbWindow = null;
            taskButton.SetParentAutoHide(true);
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!isClosing && !taskButton.btn.ContextMenu.IsOpen && !taskButton.IsMouseOver)
                Close();
        }

        private void bdrThumbInner_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                taskButton.SelectWindow();
                Close();
            }
        }

        private void bdrThumbInner_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            taskButton.ConfigureContextMenu();
            taskButton.btn.ContextMenu.IsOpen = true;
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            isAnimating = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Tasks.Instance.CloseWindow(taskButton.Window);
            Close();
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            if (closeButton.Visibility != Visibility.Visible)
                closeButton.Visibility = Visibility.Visible;
        }
    }
}
