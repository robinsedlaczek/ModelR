using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using WaveDev.ModelR.Models;
using System;
using System.Globalization;

namespace WaveDev.ModelR.Server
{
    public class ModelRHub : Hub
    {
        #region Private Fields

        private IList<SceneInfoModel> _scenes;

        #endregion

        #region Constructor

        public ModelRHub()
        {
            _scenes = new List<SceneInfoModel>
            {
                new SceneInfoModel { Name = "Scene 1", Description = "The first default scene at the server." },
                new SceneInfoModel { Name = "Scene 2", Description = "Just another scene." }
            };

        }

        #endregion

        #region Public Overrides

        public override Task OnConnected()
        {
            Console.WriteLine(string.Format(CultureInfo.CurrentUICulture, "Client '{0}' connected.", Context.ConnectionId));
            
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine(string.Format(CultureInfo.CurrentUICulture, "Client '{0}' disconnected.", Context.ConnectionId));

            return base.OnDisconnected(stopCalled);
        }

        #endregion

        #region Public Hub Methods

        //[Authorize(Users = "Robin")]
        public IEnumerable<SceneInfoModel> GetAvailableScenes()
        {
            return _scenes;
        }

        #endregion

    }
}
