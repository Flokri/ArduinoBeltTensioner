using BeltTensionerTest.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BeltTensionerTest
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region instances
        private MainViewModel _viewModel;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        #region property changed handler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) { }
    }
}