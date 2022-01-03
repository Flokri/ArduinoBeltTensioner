using System;
using BeltTensionerTest.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Input;

namespace BeltTensionerTest.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region instances
        private static bool _continue;

        private SerialMessages _serial;
        private ObservableCollection<string> _availablePorts = new();

        private Task _readTask;

        private string _selectedPort;

        private static SerialPort _serialPort;

        private ICommand _clearCommand;
        private ICommand _refreshCommand;
        private ICommand _connectCommand;
        private ICommand _closeConnectionCommand;
        #endregion

        #region ctor
        public MainViewModel()
        {
            Serial = SerialMessages.Instance;

            Serial.Messages.Add("Serial output..");

            AvailablePorts.CollectionChanged += (s, e) => { OnPropertyChanged("AvailablePorts"); };

            _serialPort = new SerialPort();
        }
        #endregion

        #region privates
        private void AddElement() { }

        private void ClearElements() => Serial.ClearMessages();

        private void GetAvailablePorts() =>
            AvailablePorts = new ObservableCollection<string>(SerialPort.GetPortNames());

        private void Connect()
        {
            if (string.IsNullOrEmpty(SelectedPort))
            {
                Serial.Messages.Add("Select correct port to connect...");
                return;
            }

            if (_serialPort.IsOpen) return;

            _serialPort.PortName = SelectedPort;
            _serialPort.BaudRate = 115200;

            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _serialPort.Open();

            ClearElements();

            _readTask = new Task(Read);
            _continue = true;
            _readTask.Start();
        }

        private void CloseConnection()
        {
            try
            {
                if (!_serialPort.IsOpen) return;

                _continue = false;

                _readTask.Wait();
                _readTask.Dispose();

                _serialPort.Close();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }
        #endregion

        #region static
        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    SerialMessages.Instance.Messages.Add(_serialPort.ReadLine());
                }
                catch (TimeoutException) { }
            }
        }
        #endregion

        #region properties
        public ObservableCollection<string> AvailablePorts
        {
            get => _availablePorts;
            set
            {
                _availablePorts = value;
                OnPropertyChanged();
            }
        }

        public SerialMessages Serial
        {
            get => _serial;
            set
            {
                _serial = value;
                OnPropertyChanged();
            }
        }

        public string SelectedPort
        {
            get => _selectedPort;
            set
            {
                _selectedPort = value;
                OnPropertyChanged();
            }
        }

        public ICommand ClearCommand => _clearCommand ??= new CommandHandler(ClearElements, true);
        public ICommand RefreshCommand => _refreshCommand ??= new CommandHandler(GetAvailablePorts, true);
        public ICommand ConnectCommand => _connectCommand ??= new CommandHandler(Connect, true);
        public ICommand CloseConnectionCommand => _closeConnectionCommand ??= new CommandHandler(CloseConnection, true);
        #endregion

        #region property changed
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}