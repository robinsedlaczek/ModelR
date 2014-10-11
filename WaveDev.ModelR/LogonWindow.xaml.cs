using System.Windows;
using WaveDev.ModelR.ViewModels;

namespace WaveDev.ModelR
{
    /// <summary>
    /// Interaction logic for LogonWindow.xaml
    /// </summary>
    public partial class LogonWindow : Window
    {
        #region Private Fields

        #endregion

        #region Construction

        public LogonWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Members

        private void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

    }
}
