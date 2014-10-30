using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WaveDev.ModelR.Communication;

namespace WaveDev.ModelR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Protected Overrides

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherHelper.Initialize();

            Exit += OnApplicationExit;

            base.OnStartup(e);
        }

        #endregion

        #region Event Handler 

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            // [RS] Finally logoff and dispose the proxy, if it has not been done yet.
            var proxy = ModelRHubClientProxy.GetInstance(string.Empty, false);

            if (proxy != null)
            {
                proxy.Logoff();
                proxy.Dispose();

                proxy = null;
            }
        }

        #endregion
    }
}
