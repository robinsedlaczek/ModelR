using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System;
using System.Globalization;
using WaveDev.ModelR.Shared.Models;
using System.Security.Claims;
using System.Drawing;
using System.IO;

namespace WaveDev.ModelR.Server
{
    public class ModelRHub : Hub
    {
        #region Private Fields

        private static IDictionary<Guid, SceneInfoModel> s_scenes; 

        #endregion

        #region Constructor

        public ModelRHub()
        {
            if (s_scenes == null)
            {
                s_scenes = new Dictionary<Guid, SceneInfoModel>();

                var scene = new SceneInfoModel(Guid.NewGuid()) { Name = "Scene 1", Description = "The first default scene at the server." };
                s_scenes.Add(scene.Id, scene);

                scene = new SceneInfoModel(Guid.NewGuid()) {Name = "Scene 2", Description = "Just another scene."};
                s_scenes.Add(scene.Id, scene);
            }
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
            return s_scenes.Values.ToList();
        }

        [Authorize]
        public void JoinSceneEditorGroup(Guid sceneId)
        {
            var identity = (ClaimsIdentity)Context.User.Identity;

            var imageUri = (from claim in identity.Claims
                            where claim.Type == "ProfileImageUri"
                            select claim).FirstOrDefault().Value;

            var image = ImageToByte(Image.FromFile(imageUri));
            var userInfo = new UserInfoModel(identity.Name, image);

            var scene = s_scenes[sceneId];
            scene.UserInfoModels.Add(userInfo);

            Groups.Add(Context.ConnectionId, sceneId.ToString());
            var group = Clients.Group(sceneId.ToString());

            if (group != null)
                group.UserJoined(userInfo);
        }

        [Authorize]
        public void CreateSceneObject(SceneObjectInfoModel sceneObject)
        {
            if (sceneObject == null)
                throw new ArgumentException("No object model passed to the server. Parameter 'sceneObject' must not be 'null'.");

            if (!s_scenes.ContainsKey(sceneObject.SceneId))
                throw new InvalidOperationException(string.Format("Scene with id '{0}' does not exist.", sceneObject.SceneId));

            var scene = s_scenes[sceneObject.SceneId];
            scene.SceneObjectInfoModels.Add(sceneObject);

            Clients.OthersInGroup(scene.Id.ToString()).SceneObjectCreated(sceneObject);
        }
        
        [Authorize]
        public void TransformSceneObject(SceneObjectInfoModel sceneObject)
        {
            if (sceneObject == null)
                throw new ArgumentException("No object model passed to the server. Parameter 'sceneObject' must not be 'null'.");

            var scene = s_scenes[sceneObject.SceneId];

            var sceneObjectFound = (from model in scene.SceneObjectInfoModels
                                    where model.Id == sceneObject.Id
                                    select model).FirstOrDefault();

            sceneObjectFound.Transformation = sceneObject.Transformation;

            Clients.OthersInGroup(scene.Id.ToString()).SceneObjectTransformed(sceneObjectFound);
        }

        [Authorize]
        public IEnumerable<UserInfoModel> GetUsers()
        {
            var users = 
                from scene in s_scenes.Values
                where scene.UserInfoModels.Any(user => user.UserName == Context.User.Identity.Name)
                select scene.UserInfoModels;

            return users.FirstOrDefault();
        }

        [Authorize]
        public IEnumerable<SceneObjectInfoModel> GetSceneObjects()
        {
            var objects = 
                from scene in s_scenes.Values
                where scene.UserInfoModels.Any(user => user.UserName == Context.User.Identity.Name)
                select scene.SceneObjectInfoModels;

            return objects.FirstOrDefault();
        }

        #endregion

        #region Private Members

        public static byte[] ImageToByte(Image image)
        {
            byte[] byteArray = new byte[0];

            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }

            return byteArray;
        }

        #endregion
    }
}
