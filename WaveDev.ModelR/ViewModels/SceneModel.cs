using System.Collections.ObjectModel;
using System.Windows.Input;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using System;
using SharpGL.SceneGraph.Transformations;
using GalaSoft.MvvmLight;
using WaveDev.ModelR.Communication;
using WaveDev.ModelR.Shared;
using WaveDev.ModelR.Shared.Models;
using System.Linq;
using GalaSoft.MvvmLight.Threading;

namespace WaveDev.ModelR.ViewModels
{
    public class SceneModel : ViewModelBase
    {
        #region Private Fields

        private ObservableCollection<ObjectModel> _objects;
        private RelayCommand _createTeapotCommand;
        private RelayCommand _createCubeCommand;
        private RelayCommand _createSphereCommand;
        private RelayCommand _createCylinderCommand;
        private RelayCommand _createDiscCommand;
        private RelayCommand _switchToTranslationCommand;
        private RelayCommand _switchToRotationCommand;
        private RelayCommand _switchToScaleCommand;
        private ObjectModel _selectedObject;
        private ModelRHubClientProxy _proxy;

        #endregion

        #region Construction

        public SceneModel()
        {
            _objects = new ObservableCollection<ObjectModel>();

            WorldAxies = new Axies();
            OrientationGrid = new Grid()
            {
                IsEnabled = true,
            };

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

        public ObservableCollection<ObjectModel> SceneObjectModels
        {
            get
            {
                return _objects;
            }
        }

        public ObjectModel SelectedObject
        {
            get
            {
                return _selectedObject;
            }

            set
            {
                Set<ObjectModel>(ref _selectedObject, value);
            }
        }

        public Action<float,float,float> TransformCurrentObject
        {
            get;
            set;
        }

        public ICommand InitializeCommunicationCommand
        {
            get
            {
                return new RelayCommand(parameter =>
                {
                    try
                    {
                        _proxy = ModelRHubClientProxy.GetInstance();

                        // TODO: [RS] Don't forget to unregister event handler somewehre.
                        _proxy.SceneObjectCreated += model => OnSceneObjectCreated(model);
                        _proxy.SceneObjectTransformed += model => OnSceneObjectTransformed(model);
                    }
                    catch (InvalidOperationException exception)
                    {
                        // TODO: [RS] Use MVVM Light here! Open message box from window, not here in the model!
                        //MessageBox.Show(exception.InnerException.InnerException.Message, "ModelR - Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        System.Windows.Application.Current.Shutdown();
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
                        (x, y, z) =>
                            {
                                if (SelectedObject == null)
                                    return;

                                var transformation = GetObjectsLinearTransformation(SelectedObject);

                                transformation.TranslateX += x;
                                transformation.TranslateY += y;
                                transformation.TranslateZ += z;

                                _proxy.TransformSceneObject(SelectedObject);
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
                        (x, y, z) =>
                            {
                                if (SelectedObject == null)
                                    return;

                                var transformation = GetObjectsLinearTransformation(SelectedObject);

                                transformation.RotateX += 10 * x;
                                transformation.RotateY += 10 * y;
                                transformation.RotateZ += 10 * z;

                                _proxy.TransformSceneObject(SelectedObject);
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
                        (x, y, z) =>
                            {
                                if (SelectedObject == null)
                                    return;

                                var transformation = GetObjectsLinearTransformation(SelectedObject);

                                transformation.ScaleX += x;
                                transformation.ScaleY += y;
                                transformation.ScaleZ += z;

                                _proxy.TransformSceneObject(SelectedObject);
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
                    _createTeapotCommand = new RelayCommand(parameter =>
                    {
                        var model = CreateObjectModel<Teapot>();
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
                    _createCubeCommand = new RelayCommand(parameter => CreateObjectModel<Cube>(), () => true);

                return _createCubeCommand;
            }
        }

        public ICommand CreateSphereCommand
        {
            get
            {
                if (_createSphereCommand == null)
                    _createSphereCommand = new RelayCommand(parameter => CreateObjectModel<Sphere>(), () => true);

                return _createSphereCommand;
            }
        }

        public ICommand CreateCylinderCommand
        {
            get
            {
                if (_createCylinderCommand == null)
                {
                    _createCylinderCommand = new RelayCommand(parameter =>
                    {
                        var model = CreateObjectModel<Cylinder>();
                        var cylinder = model.SceneElement as Cylinder;

                        cylinder.TopRadius = 0.5d;
                        cylinder.BaseRadius = 0.5d;
                        cylinder.Height = 2d;
                        cylinder.Slices = 20;
                        cylinder.Stacks = 20;
                    }, () => true);
                }

                return _createCylinderCommand;
            }
        }

        public ICommand CreateDiskCommand
        {
            get
            {
                if (_createDiscCommand == null)
                    _createDiscCommand = new RelayCommand(parameter => CreateObjectModel<Disk>(), () => true);

                return _createDiscCommand;
            }
        }

        #endregion

        #region Event Handlers

        private void OnSceneObjectCreated(SceneObjectInfoModel infoModel)
        {
            ObjectModel model = null;

            switch (infoModel.SceneObjectType)
            {
                case SceneObjectType.Teapot:
                    model = new ObjectModel(new Teapot(), infoModel.Id);
                    break;
                case SceneObjectType.Cube:
                    model = new ObjectModel(new Cube(), infoModel.Id);
                    break;
                case SceneObjectType.Cylinder:
                    model = new ObjectModel(new Cylinder(), infoModel.Id);
                    break;
                case SceneObjectType.Disk:
                    model = new ObjectModel(new Disk(), infoModel.Id);
                    break;
                case SceneObjectType.Sphere:
                    model = new ObjectModel(new Sphere(), infoModel.Id);
                    break;
                default:
                    break;
            }

            DispatcherHelper.RunAsync(() => _objects.Add(model));
        }

        private void OnSceneObjectTransformed(SceneObjectInfoModel model)
        {
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

        private ObjectModel CreateObjectModel<T>() where T : SceneElement, new()
        {
            var model = new ObjectModel(new T());

            _objects.Add(model);

            if (_objects.Count == 1)
                SelectedObject = model;

            _proxy.CreateSceneObject(model);

            return model;
        }

        private LinearTransformation GetObjectsLinearTransformation(ObjectModel model)
        {
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

        #endregion

    }
}
