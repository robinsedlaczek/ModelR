using GalaSoft.MvvmLight.Messaging;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using WaveDev.ModelR.Messages;
using WaveDev.ModelR.ViewModels;

namespace WaveDev.ModelR.Views
{
    /// <summary>
    /// Interaction logic for LogonWindow.xaml
    /// </summary>
    public partial class LogonWindow : Window
    {
        #region Private Fields

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        #endregion

        #region Construction

        public LogonWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<ExceptionCausedApplicationShutdownMessage>(this, message => OnExceptionCausedApplicationShutdown(message));

            UserNameTextBox.Focus();
        }

        #endregion

        #region Event Handlers

        private void OnExceptionCausedApplicationShutdown(ExceptionCausedApplicationShutdownMessage message)
        {
            Xceed.Wpf.Toolkit.MessageBox.Show(message.Exception.Message, "ModelR - Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            HideCloseButton();
        }

        private void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Private Members

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void HideCloseButton()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        #endregion
    }
}
