using System.Collections.Generic;
using System.ComponentModel;

namespace SemaphoreTask
{
    public class VM : INotifyPropertyChanged
    {
        public VM()
        {
            CreatedThreads = WorkingThreads = ThreadsInSemaphore = new List<string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string o)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(o));
        }
        List<string> createdThreads;
        public List<string> CreatedThreads {
            get
            {
                return createdThreads;
            }
            set
            {
                createdThreads = value;
                OnPropertyChanged("CreatedThreads");
            }
        }

        List<string> workingThreads;
        public List<string> WorkingThreads
        {
            get
            {
                return workingThreads;
            }
            set
            {
                workingThreads = value;
                OnPropertyChanged("WorkingThreads");
            }
        }

        List<string> threadsInSemaphore;
        public List<string> ThreadsInSemaphore
        {
            get
            {
                return threadsInSemaphore;
            }
            set
            {
                threadsInSemaphore = value;
                OnPropertyChanged("ThreadsInSemaphore");
            }
        }

        string selectedThread;
        public string SelectedThread
        {
            get
            {
                return selectedThread;
            }
            set
            {
                selectedThread = value;
                OnPropertyChanged("SelectedThread");
            }
        }
    }
}
