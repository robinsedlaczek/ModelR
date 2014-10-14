using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System;
using System.Globalization;
using WaveDev.ModelR.Server.Security;
using WaveDev.ModelR.Shared.Models;

namespace WaveDev.ModelR.Server
{
    public class ModelRHub : Hub
    {
        #region Private Fields

        private IDictionary<Guid, SceneInfoModel> _scenes;

        #endregion

        #region Constructor

        public ModelRHub()
        {
            _scenes = new Dictionary<Guid, SceneInfoModel>();

            var scene = new SceneInfoModel(Guid.NewGuid()) { Name = "Scene 1", Description = "The first default scene at the server." };
            _scenes.Add(scene.Id, scene);

            scene = new SceneInfoModel(Guid.NewGuid()) { Name = "Scene 2", Description = "Just another scene." };
            _scenes.Add(scene.Id, scene);
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

        public IEnumerable<SceneInfoModel> GetAvailableScenes()
        {
            return _scenes.Values.ToList();
        }

        [ModelRAuthorize]
        public void JoinSceneEditorGroup(Guid sceneId)
        {
            Groups.Add(Context.ConnectionId, sceneId.ToString());
        }

        [ModelRAuthorize]
        public void CreateSceneObject(SceneObjectInfoModel sceneObject)
        {
            if (sceneObject == null)
                throw new ArgumentException("No object model passed to the server. Parameter 'sceneObject' must not be 'null'.");

            if (!_scenes.ContainsKey(sceneObject.SceneId))
                throw new InvalidOperationException(string.Format("Scene with id '{0}' does not exist.", sceneObject.SceneId));

            var scene = _scenes[sceneObject.SceneId];
            scene.SceneObjectInfoModels.Add(sceneObject);

            Clients.OthersInGroup(scene.Id.ToString()).SceneObjectCreated(sceneObject);
        }
        
        #endregion

    }
}
