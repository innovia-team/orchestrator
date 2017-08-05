using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Syncfusion.Windows.Shared;

namespace HitBtcOrchestrator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ChromelessWindow, INotifyPropertyChanged
    {
        private MainWindowViewModel _viewModel;

        public MainWindowViewModel ViewModel
        {
            get => _viewModel;
            set { _viewModel = value; OnPropertyChanged(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
