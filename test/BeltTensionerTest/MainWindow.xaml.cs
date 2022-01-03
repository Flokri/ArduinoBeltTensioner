using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BeltTensionerTest
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region instances
        private ObservableCollection<string> _serialMessages;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Init();
        }

        #region privates
        private void Init()
        {
            SerialMessages = new ObservableCollection<string>
            {
                "message 1",
                "message 2"
            };
        }
        #endregion

        #region properties
        public ObservableCollection<string> SerialMessages
        {
            get => _serialMessages;
            set
            {
                _serialMessages = value;
                OnPropertyChanged();
            }
        }
        #endregion


        #region property changed handler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            SerialMessages.Add("test");
        }
    }
}