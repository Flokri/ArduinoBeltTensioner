using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BeltTensionerTest.Model
{
    public class SerialMessages : INotifyPropertyChanged
    {
        #region instances
        private static SerialMessages _instance;

        private ObservableCollection<string> _messages = new();
        #endregion

        #region ctor
        private SerialMessages()
        {
            Messages.CollectionChanged += (sender, args) => { OnPropertyChanged("Messages"); };
        }
        #endregion

        #region public
        public void ClearMessages() => Messages.Clear();
        #endregion

        #region properties
        public static SerialMessages Instance => _instance ??= new SerialMessages();

        public ObservableCollection<string> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region property changed
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}