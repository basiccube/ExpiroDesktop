using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Threading;
using System.Data.OleDb;
using System.Data;
using System.Windows.Media;
using System.ComponentModel;
using System.Diagnostics;
using CairoDesktop.Common.Logging;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace CairoDesktop.Common
{
    public class SearchHelper : DependencyObject
    {
        static CSearchManager cManager;
        static ISearchQueryHelper cHelper;
        //static OleDbConnection cConnection;
        //static WaitCallback workerCallback;
        static bool ToAbortFlag = false;
        static bool IsWorkingFlag = false;

        static int MAX_RESULT = 8;

        class SearchObjectState
        {
            public string SearchString;
            public ManualResetEvent Reset;

            public SearchObjectState(string searchStr)
            {
                SearchString = searchStr;
                Reset = new ManualResetEvent(false);
            }

            public SearchObjectState()
            {
                SearchString = string.Empty;
                Reset = new ManualResetEvent(false);
            }
        }
        static SearchObjectState searchObjState = new SearchObjectState();

        static SearchHelper()
        {
            m_results = new ThreadSafeObservableCollection<SearchResult>();
            cManager = new CSearchManager();
        }

        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register("SearchText", typeof(string), typeof(SearchHelper), new UIPropertyMetadata(default(string),
            new PropertyChangedCallback(OnSearchTextChanged)));

        static void OnSearchTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //change
            ToAbortFlag = IsWorkingFlag;

            // removed async due to result duplication
            /*if (workerCallback == null)
                workerCallback = new WaitCallback(doSearch);
                */
            searchObjState.SearchString = e.NewValue.ToString();
            /*if (searchObjState.SearchString.Length > 2)
            {
                ThreadPool.QueueUserWorkItem(workerCallback, searchObjState);
            }*/
            doSearch(searchObjState);
        }

        static void doSearch(object state)
        {
            // check if user wants to show file extensions. always show on windows < 8 due to property missing
            string displayNameColumn = "System.ItemNameDisplayWithoutExtension";
            RegistryKey hideFileExt = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", false);
            object hideFileExtValue = hideFileExt?.GetValue("HideFileExt");
            if ((hideFileExtValue != null && hideFileExtValue.ToString() == "0") || !Interop.Shell.IsWindows8OrBetter)
                displayNameColumn = "System.ItemNameDisplay";

            SearchObjectState sos = (SearchObjectState)state;
            if (sos.SearchString == string.Empty)
            {
                m_results.Clear();
                sos.Reset.Set();
                return;
            }
            cHelper = cManager.GetCatalog("SYSTEMINDEX").GetQueryHelper();
            cHelper.QuerySelectColumns = "\"" + displayNameColumn + "\",\"System.ItemUrl\",\"System.ItemPathDisplay\",\"System.DateModified\"";
            cHelper.QueryMaxResults = MAX_RESULT;
            cHelper.QuerySorting = "System.Search.Rank desc";

            OleDbConnection cConnection;

            try
            {
                using (cConnection = new OleDbConnection(
                            cHelper.ConnectionString))
                {
                    cConnection.Open();
                    using (OleDbCommand cmd = new OleDbCommand(
                            cHelper.GenerateSQLFromUserQuery(
                                sos.SearchString
                            ),
                            cConnection))
                    {
                        if (cConnection.State == ConnectionState.Open)
                        {
                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                m_results.Clear();
                                IsWorkingFlag = true;
            
                                while (!reader.IsClosed && reader.Read())
                                {
                                    if (ToAbortFlag)
                                        break;

                                    SearchResult result = new SearchResult() { Name = reader[0].ToString(), Path = reader[1].ToString(), PathDisplay = reader[2].ToString(), DateModified = reader[3].ToString() };

                                    if (result.Name.EndsWith(".lnk"))
                                        result.Name = result.Name.Substring(0, result.Name.Length - 4); // Windows always hides this regardless of setting, so do it

                                    m_results.Add(result);
                                    
                                }

                                IsWorkingFlag = false;
                            }
                        }

                    }  
                }
            }
            catch (Exception ex)
            {
                CairoLogger.Instance.Error("Error in doSearch.",ex);
            }

            sos.Reset.Set();
            ToAbortFlag = false;

        }
        public string SearchText
        {
            get { return GetValue(SearchTextProperty).ToString(); }
            set { SetValue(SearchTextProperty, value); }
        }

        static ThreadSafeObservableCollection<SearchResult> m_results;
        public ReadOnlyObservableCollection<SearchResult> Results
        {
            get { return new ReadOnlyObservableCollection<SearchResult>(m_results); }
        }
    }

    public class SearchResult : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string PathDisplay { get; set; }
        public string DateModified { get; set; }
        public string DateModifiedDisplay { get { return String.Format(Localization.DisplayString.sSearch_LastModified, DateModified); } }

        public ImageSource Icon
        {
            get
            {
                if (icon == null && !_iconLoading)
                {
                    _iconLoading = true;

                    Task.Factory.StartNew(() =>
                    {
                        string iconPath = Path.Substring(Path.IndexOf(':') + 1).Replace("/", "\\");
                        Icon = IconImageConverter.GetImageFromAssociatedIcon(iconPath, IconSize.Large);
                        _iconLoading = false;
                    }, CancellationToken.None, TaskCreationOptions.None, Interop.Shell.IconScheduler);
                }

                return icon;
            }
            set
            {
                icon = value;
                OnPropertyChanged("Icon");
            }
        }

        private bool _iconLoading = false;
        private ImageSource icon { get; set; }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// This Event is raised whenever a property of this object has changed. Necesary to sync state when binding.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        [DebuggerNonUserCode]
        private void OnPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        #endregion
    }
}
