using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace SemaphoreTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Semaphore s1 = new Semaphore(3,3,"My Semophore");
        private const string MutexName = "MUTEX_SINGLEINSTANCEANDNAMEDPIPE";
        private bool _firstApplicationInstance;
        private Mutex _mutexApplication;
        static VM VM;
        static Dictionary<int, EventWaitHandle> waitHandles = new Dictionary<int, EventWaitHandle>();
        static List<string> ct = new List<string>();
        static List<string> wt = new List<string>();
        static List<string> wts = new List<string>();
        static string selected;
        public MainWindow()
        {
            InitializeComponent();
            VM = new VM();
            DataContext = VM;
        }

        private void ThreadCreateBtn(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(SomeMethod, s1);
        }

        private void CT_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VM.SelectedThread != null)
            {
                selected = VM.SelectedThread;
                

                ct.Remove(ct.Find(x => x.CompareTo(selected) == 0));
                VM.CreatedThreads = null;
                VM.CreatedThreads = ct;

                wt.Add(selected);
                VM.WorkingThreads = null;
                VM.WorkingThreads = wt;

                int.TryParse(selected.Split(' ')[1], out int i);
                waitHandles[i].Set();
                waitHandles.Remove(i);
            }
        }

        static void SomeMethod(object obj)
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            App.Current.Dispatcher.Invoke(delegate
            {
                ct.Add($"Thread {id}");
                VM.CreatedThreads = null;
                VM.CreatedThreads = ct;
            });
            waitHandles.Add(Thread.CurrentThread.ManagedThreadId, new AutoResetEvent(false));
            waitHandles[Thread.CurrentThread.ManagedThreadId].WaitOne();


            Semaphore s = obj as Semaphore;
            bool stop = false;
            while (!stop)
            {
                if (s.WaitOne(500))
                {
                    int id2 = Thread.CurrentThread.ManagedThreadId;
                    try
                    {
                        App.Current.Dispatcher.Invoke(delegate
                        {
                            wt.Remove(wt.Find(x => x.CompareTo($"Thread {id2}") == 0));
                            VM.WorkingThreads = null;
                            VM.WorkingThreads = wt;


                            wts.Add($"Thread {id2}");
                            VM.ThreadsInSemaphore = null;
                            VM.ThreadsInSemaphore = wts;
                        });
                    }
                    finally
                    {
                        Thread.Sleep(4000);
                        App.Current.Dispatcher.Invoke(delegate
                        {
                            wts.Remove(wts.Find(x => x.CompareTo($"Thread {id2}") == 0));
                            VM.ThreadsInSemaphore = null;
                            VM.ThreadsInSemaphore = wts;
                        });
                        stop = true;
                        s.Release();
                    }
                }
            }
        }


        private bool IsApplicationFirstInstance()
        {
            if (_mutexApplication == null)
            {
                _mutexApplication = new Mutex(true, MutexName, out _firstApplicationInstance);
            }
            return _firstApplicationInstance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsApplicationFirstInstance())
            {
                MessageBox.Show("There is open instance of this app, please close it before", "Close other Instance", MessageBoxButton.OK, MessageBoxImage.Error,MessageBoxResult.Yes,MessageBoxOptions.ServiceNotification);
                Application.Current.Shutdown();
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            if (_mutexApplication != null)
            {
                _mutexApplication.Dispose();
            }
        }
    }
}
