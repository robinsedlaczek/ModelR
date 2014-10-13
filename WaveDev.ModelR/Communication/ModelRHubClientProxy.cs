using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace WaveDev.ModelR.Communication
{
    internal static class ModelRHubClientProxy
    {
        #region Private Fields

        private static HubConnection s_connection;
        private static IHubProxy s_proxy;

        #endregion

        #region Private Properties

        private static HubConnection ModelRHubConnection
        {
            get
            {
                if (s_connection == null)
                    InitSignalRConnection();

                return s_connection;
            }
        }

        private static IHubProxy ModelRHubProxy
        {
            get
            {
                if (s_proxy == null)
                    InitSignalRProxy().Wait();

                return s_proxy;
            }
        }

        #endregion

        #region Private Methods

        private static async void InitSignalRConnection()
        {
            s_connection = new HubConnection("http://localhost:8082/")
            {
                TraceLevel = TraceLevels.All,
                TraceWriter = Console.Out
            };

            await InitSignalRProxy();
        }

        private static async Task InitSignalRProxy()
        {
            s_proxy = ModelRHubConnection.CreateHubProxy("ModelRHub");

            await ModelRHubConnection.Start();
        }

        #endregion
    }
}
