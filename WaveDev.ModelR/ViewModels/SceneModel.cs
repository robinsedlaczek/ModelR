using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using System;
using SharpGL.SceneGraph.Transformations;
using GalaSoft.MvvmLight;
using WaveDev.ModelR.Communication;
using WaveDev.ModelR.Messages;
using WaveDev.ModelR.Shared;
using WaveDev.ModelR.Shared.Models;
using System.Linq;
using GalaSoft.MvvmLight.Threading;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections.Generic;

namespace WaveDev.ModelR.ViewModels
{
    public class SceneModel : ViewModelBase
    {
        #region Private Fields

        private ObservableCollection<SceneObjectModel> _objects;
        private ObservableCollection<UserModel> _users;
        private RelayCommand _createTeapotCommand;
        private RelayCommand _createCubeCommand;
        private RelayCommand _createSphereCommand;
        private RelayCommand _createCylinderCommand;
        private RelayCommand _createDiscCommand;
        private RelayCommand _switchToTranslationCommand;
        private RelayCommand _switchToRotationCommand;
        private RelayCommand _switchToScaleCommand;
        private RelayCommand _executeScriptCommand;
        private SceneObjectModel _selectedObject;
        private ModelRHubClientProxy _proxy;
        private UserModel _selectedUser;
        private string _script;

        #endregion

        #region Construction

        public SceneModel()
        {
            WorldAxies = new Axies();
            OrientationGrid = new Grid()
            {
                IsEnabled = true,
            };

            SceneObjectModels = new ObservableCollection<SceneObjectModel>();
            Errors = new ObservableCollection<MessageViewModel>();

            // [RS] Set translation as initial object transformation tool.
            SwitchToTranslationCommand.Execute(null);
        }

        #endregion

        #region Public Members

        public Axies WorldAxies
        {
            get;
            private set;
        }

        public Grid OrientationGrid
        {
            get;
            private set;
        }

        public ObservableCollection<UserModel> UserModels
        {
            get
            {
                return _users;
            }

            private set
            {
                Set(ref _users, value); 
            }
        }

        public UserModel SelectedUser
        {
            get
            {
                return _selectedUser;
            }

            set
            {
                Set<UserModel>(ref _selectedUser, value);
            }
        }

        public ObservableCollection<SceneObjectModel> SceneObjectModels
        {
            get
            {
                return _objects;
            }

            private set
            {
                Set(ref _objects, value);
            }
        }

        public SceneObjectModel SelectedObject
        {
            get
            {
                return _selectedObject;
            }

            set
            {
                Set(ref _selectedObject, value);
            }
        }

        public Action<float,float,float> TransformCurrentObject
        {
            get;
            set;
        }

        public string Script
        {
            get
            {
                return _script;
            }

            set
            {
                Set<string>(ref _script, value);
            }
        }

        public IList<MessageViewModel> Errors
        {
            get;
        }

        #region Commands 

        public ICommand InitializeCommunicationCommand
        {
            get
            {
                return new RelayCommand(async parameter =>
                {
                    try
                    {
                        _proxy = ModelRHubClientProxy.GetInstance();

                        //// TODO: [RS] Handlers should be unregistered somewhere!?
                        _proxy.SceneObjectCreated += model => OnSceneObjectCreated(model);
                        _proxy.SceneObjectTransformed += model => OnSceneObjectTransformed(model);
                        _proxy.UserJoined += model => OnUserJoined(model);
                        _proxy.UserLoggedOff += model => OnUserLoggedOff(model);

                        await LoadUsersAsync();
                        await LoadSceneObjectsAsync();
                    }
                    catch (InvalidOperationException exception)
                    {
                        // [RS] If there are errors during initialisation, the application should be shut down. That's why the
                        //      ExceptionCausedApplicationShutdownMessage message will be send here.
                        Messenger.Default.Send<ExceptionCausedApplicationShutdownMessage>(
                            new ExceptionCausedApplicationShutdownMessage()
                            {
                                Exception = exception,
                                ShowMessageToUser = true
                            });
                    }
                }, () => true);
            }
        }

        public ICommand SwitchToTranslationCommand
        {
            get
            {
                if (_switchToTranslationCommand == null)
                {
                    _switchToTranslationCommand = new RelayCommand(parameter => TransformCurrentObject =
                        async (x, y, z) =>
                            {
                                if (SelectedObject == null)
                                    return;

                                var transformation = GetObjectsLinearTransformation(SelectedObject);

                                transformation.TranslateX += x;
                                transformation.TranslateY += y;
                                transformation.TranslateZ += z;

                                try
                                {
                                    await _proxy.TransformSceneObject(SelectedObject);
                                }
                                catch (UserNotAuthorizedException exception)
                                {
                                    // [RS] If the user is not authorized to transform an object, it will be forbidden at the client, too.
                                    //      So the transformation that has been done above will be undone. 
                                    transformation.TranslateX -= x;
                                    transformation.TranslateY -= y;
                                    transformation.TranslateZ -= z;

                                    var info = string.Format(CultureInfo.CurrentUICulture, "The user '{0}' is not authorized to move scene objects.", exception.UserName);
                                    Messenger.Default.Send<NotAuthorizedForOperationMessage>(new NotAuthorizedForOperationMessage(info));
                                }
                            },
                        () => true);
                }

                return _switchToTranslationCommand;
            }
        }

        public ICommand SwitchToRotationCommand
        {
            get
            {
                if (_switchToRotationCommand == null)
                {
                    _switchToRotationCommand = new RelayCommand(parameter => TransformCurrentObject = 
                        async (x, y, z) =>
                            {
                                if (SelectedObject == null)
                                    return;

                                var transformation = GetObjectsLinearTransformation(SelectedObject);

                                transformation.RotateX += 10 * x;
                                transformation.RotateY += 10 * y;
                                transformation.RotateZ += 10 * z;

                                try
                                {
                                    await _proxy.TransformSceneObject(SelectedObject);
                                }
                                catch (UserNotAuthorizedException exception)
                                {
                                    // [RS] If the user is not authorized to transform an object, it will be forbidden at the client, too.
                                    //      So the transformation that has been done above will be undone. 
                                    transformation.RotateX -= 10 * x;
                                    transformation.RotateY -= 10 * y;
                                    transformation.RotateZ -= 10 * z;

                                    var info = string.Format(CultureInfo.CurrentUICulture, "The user '{0}' is not authorized to rotate scene objects.", exception.UserName);
                                    Messenger.Default.Send<NotAuthorizedForOperationMessage>(new NotAuthorizedForOperationMessage(info));
                                }
                            },
                        () => true);
                }

                return _switchToRotationCommand;
            }
        }

        public ICommand SwitchToScaleCommand
        {
            get
            {
                if (_switchToScaleCommand == null)
                {
                    _switchToScaleCommand = new RelayCommand(parameter => TransformCurrentObject =
                        async (x, y, z) =>
                            {
                                if (SelectedObject == null)
                                    return;

                                var transformation = GetObjectsLinearTransformation(SelectedObject);

                                transformation.ScaleX += x;
                                transformation.ScaleY += y;
                                transformation.ScaleZ += z;

                                try
                                {
                                    await _proxy.TransformSceneObject(SelectedObject);
                                }
                                catch (UserNotAuthorizedException exception)
                                {
                                    // [RS] If the user is not authorized to transform an object, it will be forbidden at the client, too.
                                    //      So the transformation that has been done above will be undone. 
                                    transformation.ScaleX -= x;
                                    transformation.ScaleY -= y;
                                    transformation.ScaleZ -= z;

                                    var info = string.Format(CultureInfo.CurrentUICulture, "The user '{0}' is not authorized to scale scene objects.", exception.UserName);
                                    Messenger.Default.Send<NotAuthorizedForOperationMessage>(new NotAuthorizedForOperationMessage(info));
                                }
                            },
                        () => true);
                }

                return _switchToScaleCommand;
            }
        }

        public ICommand CreateTeapotCommand
        {
            get
            {
                if (_createTeapotCommand == null)
                    _createTeapotCommand = new RelayCommand(async parameter =>
                    {
                        var model = await CreateObjectModel<Teapot>();
                        var teapot = model.SceneElement as Teapot;

                        teapot.Transformation = new LinearTransformation() { TranslateX = 0f, TranslateY = 0.55f, TranslateZ = 0f };
                    }, 
                    () => true);

                return _createTeapotCommand;
            }
        }

        public ICommand CreateCubeCommand
        {
            get
            {
                if (_createCubeCommand == null)
                    _createCubeCommand = new RelayCommand(async parameter => await CreateObjectModel<Cube>(), () => true);

                return _createCubeCommand;
            }
        }

        public ICommand CreateSphereCommand
        {
            get
            {
                if (_createSphereCommand == null)
                    _createSphereCommand = new RelayCommand(async parameter => await CreateObjectModel<Sphere>(), () => true);

                return _createSphereCommand;
            }
        }

        public ICommand CreateCylinderCommand
        {
            get
            {
                if (_createCylinderCommand == null)
                {
                    _createCylinderCommand = new RelayCommand(async parameter => await CreateObjectModel<Cylinder>(), () => true);
                }

                return _createCylinderCommand;
            }
        }

        public ICommand CreateDiskCommand
        {
            get
            {
                if (_createDiscCommand == null)
                {
                    _createDiscCommand = new RelayCommand(async parameter => await CreateObjectModel<Disk>(), () => true);
                }

                return _createDiscCommand;
            }
        }

        public ICommand ExecuteScriptCommand
        {
            get
            {
                if (_executeScriptCommand == null)
                {
                    _executeScriptCommand = new RelayCommand(async parameter => await ExecuteScript(), () => true);
                }

                return _executeScriptCommand;
            }
        }

        #endregion // Commands

        #endregion

        #region Event Handlers

        private void OnUserJoined(UserInfoModel infoModel)
        {
            var userModel = new UserModel(infoModel.UserName, infoModel.Image);

            DispatcherHelper.CheckBeginInvokeOnUI(() => UserModels.Add(userModel));
        }

        private void OnUserLoggedOff(UserInfoModel model)
        {
            var userModel = (from user in UserModels
                             where user.UserName == model.UserName
                             select user).FirstOrDefault();

            DispatcherHelper.CheckBeginInvokeOnUI(() => UserModels.Remove(userModel));
        }

        private void OnSceneObjectCreated(SceneObjectInfoModel infoModel)
        {
            // TODO: [RS] Exception Handling!

            SceneObjectModel model = null;

            switch (infoModel.SceneObjectType)
            {
                case SceneObjectType.Teapot:
                    model = new SceneObjectModel(new Teapot(), infoModel.Id);
                    break;
                case SceneObjectType.Cube:
                    model = new SceneObjectModel(new Cube(), infoModel.Id);
                    break;
                case SceneObjectType.Cylinder:
                    model = new SceneObjectModel(new Cylinder(), infoModel.Id);
                    break;
                case SceneObjectType.Disk:
                    model = new SceneObjectModel(new Disk(), infoModel.Id);
                    break;
                case SceneObjectType.Sphere:
                    model = new SceneObjectModel(new Sphere(), infoModel.Id);
                    break;
                default:
                    break;
            }

            if (infoModel.Transformation != null)
            {
                var transformation = GetObjectsLinearTransformation(model);

                transformation.TranslateX = infoModel.Transformation.TranslateX;
                transformation.TranslateY = infoModel.Transformation.TranslateY;
                transformation.TranslateZ = infoModel.Transformation.TranslateZ;

                transformation.RotateX = infoModel.Transformation.RotateX;
                transformation.RotateY = infoModel.Transformation.RotateY;
                transformation.RotateZ = infoModel.Transformation.RotateZ;

                transformation.ScaleX = infoModel.Transformation.ScaleX;
                transformation.ScaleY = infoModel.Transformation.ScaleY;
                transformation.ScaleZ = infoModel.Transformation.ScaleZ;
            }

            DispatcherHelper.RunAsync(() => SceneObjectModels.Add(model));
        }

        private void OnSceneObjectTransformed(SceneObjectInfoModel model)
        {
            // TODO: [RS] Exception Handling!

            var objectToTransform = (from objectFound in _objects
                                     where objectFound.Id == model.Id
                                     select objectFound).FirstOrDefault();

            if (objectToTransform == null)
                throw new InvalidOperationException(string.Format("Changed scene object ('{0}') cannot be found in the local scene.", model.Id));

            var transformation = GetObjectsLinearTransformation(objectToTransform);

            transformation.TranslateX = model.Transformation.TranslateX;
            transformation.TranslateY = model.Transformation.TranslateY;
            transformation.TranslateZ = model.Transformation.TranslateZ;

            transformation.RotateX = model.Transformation.RotateX;
            transformation.RotateY = model.Transformation.RotateY;
            transformation.RotateZ = model.Transformation.RotateZ;

            transformation.ScaleX = model.Transformation.ScaleX;
            transformation.ScaleY = model.Transformation.ScaleY;
            transformation.ScaleZ = model.Transformation.ScaleZ;
        }

        #endregion

        #region Private Members

        private async Task LoadUsersAsync()
        {
            var users = await _proxy.GetUsers();

            var userModels = from user in users
                             select new UserModel(user.UserName, user.Image);

            UserModels = new ObservableCollection<UserModel>(userModels);
        }

        private async Task LoadSceneObjectsAsync()
        {
            var sceneObjects = await _proxy.GetSceneObjects();

            foreach (var objectInfoModel in sceneObjects.AsParallel())
                OnSceneObjectCreated(objectInfoModel);
        }

        private async Task<SceneObjectModel> CreateObjectModel<T>() 
            where T : SceneElement, new()
        {
            try
            {
                var model = new SceneObjectModel(new T());

                await _proxy.CreateSceneObject(model);

                _objects.Add(model);

                if (_objects.Count == 1)
                    SelectedObject = model;

                return model;
            }
            catch (UserNotAuthorizedException exception)
            {
                var info = string.Format(CultureInfo.CurrentUICulture, "The user '{0}' is not authorized to create scene objects.", exception.UserName);
                Messenger.Default.Send<NotAuthorizedForOperationMessage>(new NotAuthorizedForOperationMessage(info));
            }

            return null;
        }

        private LinearTransformation GetObjectsLinearTransformation(SceneObjectModel model)
        {
            // TODO: [RS] Exception Handling!

            if (model == null)
                throw new ArgumentNullException("model");

            LinearTransformation transformation;

            if (model.Transformation == null)
            {
                transformation = new LinearTransformation();
                model.Transformation = transformation;
            }
            else
            {
                transformation = model.Transformation;
            }

            return transformation;
        }

        private async Task ExecuteScript()
        {
            try
            {
                var script = Script;

                var scriptOptions = ScriptOptions.Default
                    .WithReferences(typeof(SceneElement).Assembly)
                    .WithImports("SharpGL.SceneGraph.Core")
                    .WithImports("SharpGL.SceneGraph.Quadrics");

                var state = await CSharpScript.RunAsync(script, scriptOptions);

                foreach (var variable in state.Variables)
                {
                    var sceneElement = variable.Value as SceneElement;

                    if (sceneElement== null)
                        continue;

                    var model = new SceneObjectModel(sceneElement);
                    _objects.Add(model);

                    SelectedObject = model;

                    // await _proxy.CreateSceneObject(model);
                }

            }
            catch (Exception exception)
        	{
                var message = new MessageViewModel(exception.Message);

                Errors.Add(message);                
            }
        }

        #endregion

    }
}
