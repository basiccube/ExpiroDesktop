using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using CairoDesktop.Common;
using CairoDesktop.Interop;

namespace CairoDesktop
{
    public partial class MenuExtraSearch : UserControl
    {
        public bool _isPrimaryScreen;
        private static bool isSearchHotkeyRegistered;

        public MenuExtraSearch(MenuBar menuBar)
        {
            InitializeComponent();

            _isPrimaryScreen = menuBar.Screen.Primary;

            SetupSearch();
        }

        private void SetupSearch()
        {
            CommandBindings.Add(new CommandBinding(CustomCommands.OpenSearchResult));

            // Show the search button only if the service is running
            if (WindowsServices.QueryStatus("WSearch") == ServiceStatus.Running)
            {
                SetSearchProvider();
            }
            else
            {
                CairoSearchMenu.Visibility = Visibility.Collapsed;
                DispatcherTimer searchcheck = new DispatcherTimer(DispatcherPriority.Background, Dispatcher)
                {
                    Interval = new TimeSpan(0, 0, 5)
                };
                searchcheck.Tick += searchcheck_Tick;
                searchcheck.Start();
            }
        }

        private void OnShowSearchHotkey(HotKey hotKey)
        {
            ToggleSearch();
        }

        private void searchcheck_Tick(object sender, EventArgs e)
        {
            if (WindowsServices.QueryStatus("WSearch") == ServiceStatus.Running)
            {
                SetSearchProvider();
                CairoSearchMenu.Visibility = Visibility.Visible;
                (sender as DispatcherTimer).Stop();
            }
            else
            {
                CairoSearchMenu.Visibility = Visibility.Collapsed;
            }
        }

        private void SetSearchProvider()
        {
            // TODO: Remove
        }

        private void btnViewResults_Click(object sender, RoutedEventArgs e)
        {
            Shell.StartProcess("search:query=" + searchStr.Text);
        }

        private void searchStr_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Shell.StartProcess("search:query=" + searchStr.Text);
            }
        }

        private void searchStr_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (CairoSearchMenu.IsKeyboardFocusWithin)
            {
                e.Handled = true;
            }
        }

        private void btnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchStr.Clear();
            FocusSearchBox(sender, e);
        }

        public void FocusSearchBox(object sender, RoutedEventArgs e)
        {
            searchStr.Dispatcher.BeginInvoke(new Action(FocusSearchBox), DispatcherPriority.Render);
        }

        private void FocusSearchBox()
        {
            searchStr.Focusable = true;
            searchStr.Focus();
            Keyboard.Focus(searchStr);
        }

        internal void ToggleSearch()
        {
            CairoSearchMenu.IsSubmenuOpen = !CairoSearchMenu.IsSubmenuOpen;
        }
    }
}