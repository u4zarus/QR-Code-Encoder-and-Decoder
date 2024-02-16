using QR_Manaeste.UserControls;
using System.Windows;
using System.Windows.Input;

namespace QR_Manaeste
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DragGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CreateQRBtn_Click(object sender, RoutedEventArgs e)
        {
            TransitionContentSlide.OnApplyTemplate();
        }

        private void ScanQRBtn_Click(object sender, RoutedEventArgs e)
        {
            TransitionContentSlide2.OnApplyTemplate();
        }
        
        private void OpenManual_Click(object sender, RoutedEventArgs e)
        {
            HelpManualControl manualControl = new HelpManualControl();

            Window manualWindow = new Window
            {
                Title = "User Manual",
                Content = manualControl,
                Width = 600,
                Height = 400,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            manualWindow.ShowDialog();
        }
    }
}
