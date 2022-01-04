using BeltTensionerTest.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private int _lof = 0;
        private int _rof = 180;
        private int _tMax = 180;

        private int _fol = 0;
        private int _for = 0;

        private ICommand _clearCommand;
        private ICommand _refreshCommand;
        private ICommand _connectCommand;
        private ICommand _closeConnectionCommand;
        private ICommand _setOffsetCommand;
        private ICommand _setForceCommand;
        private ICommand _setDefaultCommand;
        private ICommand _setMaxForceCommand;
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
            //_serialPort.BaudRate = 256000;

            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            try
            {
                _serialPort.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Access Denied");
            }

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

        private void SetOffset()
        {
            try
            {
                if (!_serialPort.IsOpen) return;

                Serial.ClearMessages();

                var step = new byte[]
                {
                    (byte)(0x40 | ((0xE0 & Lof) >> 2)),
                    (byte)(0x7F & Lof),

                    (byte)(0x41 | ((0xE0 & Rof) >> 2)),
                    (byte)(0x7F & Rof),

                    (byte)(0x44 | ((0xE0 & TMax) >> 2)),
                    (byte)(0x7F & TMax),
                };

                var chars = new List<char>();

                foreach (var item in step)
                {
                    chars.Add((char)item);
                }

                var values = new string(chars.ToArray());

                Write(step);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        private void SetForce()
        {
            try
            {
                if (!_serialPort.IsOpen) return;

                Serial.ClearMessages();

                var step = new byte[]
                {
                    (byte)(0x42 | ((0xE0 & Fol) >> 2)),
                    (byte)(0x7F & Fol),

                    (byte)(0x43 | ((0xE0 & For) >> 2)),
                    (byte)(0x7F & For),
                };

                Write(step);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        private void SetDefault()
        {
            try
            {
                if (!_serialPort.IsOpen) return;

                Serial.ClearMessages();

                var step = new byte[]
                {
                    (byte)(0x42 | ((0xE0 & 0) >> 2)),
                    (byte)(0x7F & 0),

                    (byte)(0x43 | ((0xE0 & 0) >> 2)),
                    (byte)(0x7F & 0),
                };

                Write(step);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        private void SetMaxForce()
        {
            try
            {
                if (!_serialPort.IsOpen) return;

                Serial.ClearMessages();

                var step = new byte[]
                {
                    (byte)(0x42 | ((0xE0 & 180) >> 2)),
                    (byte)(0x7F & 180),

                    (byte)(0x43 | ((0xE0 & 180) >> 2)),
                    (byte)(0x7F & 180),
                };

                Write(step);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        private void Write(byte[] msg)
        {
            try
            {
                if (!_serialPort.IsOpen) return;

                var chars = new List<char>();

                foreach (var item in msg)
                {
                    chars.Add((char)item);
                }

                var outputArray = Encoding.Latin1.GetChars(msg);
                var values = new string(outputArray);

                //_serialPort.Write(msg, 0, msg.Length);

                _serialPort.Write(values);
            }
            catch (TimeoutException) { }
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

        public int Lof
        {
            get => _lof;
            set
            {
                _lof = value;
                OnPropertyChanged();
            }
        }

        public int Rof
        {
            get => _rof;
            set
            {
                _rof = value;
                OnPropertyChanged();
            }
        }

        public int TMax
        {
            get => _tMax;
            set
            {
                _tMax = value;
                OnPropertyChanged();
            }
        }

        public int Fol
        {
            get => _fol;
            set
            {
                _fol = value;
                OnPropertyChanged();
            }
        }

        public int For
        {
            get => _for;
            set
            {
                _for = value;
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
        public ICommand SetOffsetCommand => _setOffsetCommand ??= new CommandHandler(SetOffset, true);
        public ICommand SetForceCommand => _setForceCommand ??= new CommandHandler(SetForce, true);
        public ICommand SetDefaultCommand => _setDefaultCommand ??= new CommandHandler(SetDefault, true);
        public ICommand SetMaxForceCommand => _setMaxForceCommand ??= new CommandHandler(SetMaxForce, true);
        #endregion

        #region property changed
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}