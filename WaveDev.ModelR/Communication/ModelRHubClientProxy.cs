using System;
using Microsoft.AspNet.SignalR.Client;
using System.Collections.Generic;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using WaveDev.ModelR.Shared.Models;
using System.Globalization;
using WaveDev.ModelR.ViewModels;
using Xceed.Wpf.Toolkit;
using WaveDev.ModelR.Shared;

namespace WaveDev.ModelR.Communication
{
    internal class ModelRHubClientProxy
    {
        #region Private Fields

        private static ModelRHubClientProxy s_instance;

        private HubConnection _connection;
        private IHubProxy _proxy;
        private IEnumerable<SceneInfoModel> _cachedScenes;
        private Guid _sceneId;

        #endregion

        #region Delegates

        public delegate void SceneObjectCreatedEventHandler(SceneObjectInfoModel infoModel);
        public delegate void SceneObjectTransformedEventHandler(SceneObjectInfoModel infoModel);

        #endregion

        #region Events

        public event SceneObjectCreatedEventHandler SceneObjectCreated;
        public event SceneObjectTransformedEventHandler SceneObjectTransformed;

        #endregion

        #region Static Members

        public static ModelRHubClientProxy GetInstance(string url = Constants.ModelRServerUrl)
        {
            if (s_instance == null || String.Compare(s_instance.ServerUrl, url, StringComparison.Ordinal) != 0)
                s_instance = new ModelRHubClientProxy(url);

            return s_instance;
        }

        #endregion

        #region Construction

        private ModelRHubClientProxy(string url)
        {
            ServerUrl = url;

            ConnectToServer();
        }

        #endregion

        #region Public Members

        public string ServerUrl
        {
            get;
            set;
        }
        
        public void Login(string user, string password, Guid sceneId)
        {
            try
            {
                _connection.Stop();

                var authenticationToken = string.Format("User={0} Password={1}", user, password);
                _connection.Headers.Add("ModelRAuthToken", authenticationToken);

                _connection.Start().Wait();

                // [RS] Don't do it async, because we have to wait if user is authorized to join the scene. If not,
                //      the UserNotAuthorizedException will be thrown. The client code has to shutdown the application.
                _proxy.Invoke("JoinSceneEditorGroup", sceneId);

                _sceneId = sceneId;
            }
            catch (Exception exception)
            {
                throw new UserNotAuthorizedException();

                throw;
            }
        }

        public IEnumerable<SceneInfoModel> Scenes
        {
            get
            {
                if (_cachedScenes == null)
                    _cachedScenes = _proxy.Invoke<IEnumerable<SceneInfoModel>>("GetAvailableScenes").Result;

                return _cachedScenes;
            }
        }

        public async void CreateSceneObject(ObjectModel sceneObject)
        {
            var type = SceneObjectType.Light;

            if (sceneObject.SceneElement is Teapot)
                type = SceneObjectType.Teapot;
            else if (sceneObject.SceneElement is Cube)
                type = SceneObjectType.Cube;
            else if (sceneObject.SceneElement is Cylinder)
                type = SceneObjectType.Cylinder;
            else if (sceneObject.SceneElement is Disk)
                type = SceneObjectType.Disk;
            else if (sceneObject.SceneElement is Sphere)
                type = SceneObjectType.Sphere;

            var infoModel = new SceneObjectInfoModel(sceneObject.Id, _sceneId) { SceneObjectType = type };

            await _proxy.Invoke("CreateSceneObject", infoModel);
        }

        public async void TransformSceneObject(ObjectModel sceneObject)
        {
            var infoModel = new SceneObjectInfoModel(sceneObject.Id, _sceneId);

            infoModel.Transformation = new TransformationInfoModel()
            {
                TranslateX = sceneObject.Transformation.TranslateX,
                TranslateY = sceneObject.Transformation.TranslateY,
                TranslateZ = sceneObject.Transformation.TranslateZ,
                RotateX = sceneObject.Transformation.RotateX,
                RotateY = sceneObject.Transformation.RotateY,
                RotateZ = sceneObject.Transformation.RotateZ,
                ScaleX = sceneObject.Transformation.ScaleX,
                ScaleY = sceneObject.Transformation.ScaleY,
                ScaleZ = sceneObject.Transformation.ScaleZ
            };

            await _proxy.Invoke("TransformSceneObject", infoModel);
        }

        #endregion

        #region Private Methods

        private void ConnectToServer()
        {
            try
            {
                _connection = new HubConnection(ServerUrl)
                {
                    TraceLevel = TraceLevels.All,
                    TraceWriter = Console.Out
                };

                _proxy = _connection.CreateHubProxy(Constants.ModelRHubName);

                _proxy.On<SceneObjectInfoModel>("SceneObjectCreated", infoModel => SceneObjectCreated(infoModel));
                _proxy.On<SceneObjectInfoModel>("SceneObjectTransformed", infoModel => SceneObjectTransformed(infoModel));

                // TODO: [RS] Method cannot be async here, because it is called from the construtor.
                _connection.Start().Wait();
            }
            catch (AggregateException exception)
            {
                Exception nextException;
                var error = exception.Message;

                foreach (var innerException in exception.InnerExceptions)
                {
                    nextException = innerException;

                    while (nextException != null)
                    {
                        error = string.Format(CultureInfo.CurrentUICulture, "{0}{1}{2}", error, Environment.NewLine, nextException.Message);
                        nextException = nextException.InnerException;
                    }

                }

                throw new InvalidOperationException(error, exception);
            }
        }

        #endregion
    }
}
