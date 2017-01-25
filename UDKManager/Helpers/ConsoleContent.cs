using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ZerO
{
    /// <summary>
    /// Class for outputting information to a fake console window
    /// </summary>
    public class ConsoleContent : INotifyPropertyChanged
    {
        private string _consoleInput = string.Empty;
        private ObservableCollection<string> _consoleOutput = new ObservableCollection<string> { "Starting download . . ." };

        public event PropertyChangedEventHandler PropertyChanged;

        public string ConsoleInput
        {
            get
            {
                return _consoleInput;
            }
            set
            {
                _consoleInput = value;
                OnPropertyChanged("ConsoleOutput");
            }
        }

        public ObservableCollection<string> ConsoleOutput
        {
            get
            {
                return _consoleOutput;
            }
            set
            {
                _consoleOutput = value;
                OnPropertyChanged("ConsoleOutput");
            }
        }

        public void RunCommand()
        {
            ConsoleOutput.Add(ConsoleInput);
            ConsoleInput = string.Empty;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
