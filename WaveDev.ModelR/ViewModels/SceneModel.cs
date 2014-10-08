using System.Collections.ObjectModel;
using System.Windows.Input;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using System;
using SharpGL.SceneGraph.Transformations;

namespace WaveDev.ModelR.ViewModels
{
    public class SceneModel
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

        #endregion

        #region Construction

        public SceneModel()
        {
            _objects = new ObservableCollection<ObjectModel>();

            CreateObjectModel<Axies>();

            // Set translation as initial object transformation tool.
            SwitchToTranslationCommand.Execute(null);
        }

        #endregion

        #region Public Members

        public ObservableCollection<ObjectModel> SceneObjectModels
        {
            get
            {
                return _objects;
            }
        }

        public ObjectModel SelectedObject
        {
            get;
            set;
        }

        public Action<float,float,float> TransformCurrentObject
        {
            get;
            set;
        }

        public ICommand SwitchToTranslationCommand
        {
            get
            {
                if (_switchToTranslationCommand == null)
                {
                    _switchToTranslationCommand = new RelayCommand(
                        () => TransformCurrentObject = (x, y, z) =>
                            {
                                if (SelectedObject == null)
                                    return;

                                var transformation = GetCurrentObjectsLinearTransformation();

                                transformation.TranslateX += x;
                                transformation.TranslateY += y;
                                transformation.TranslateZ += z;
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
                    _switchToRotationCommand = new RelayCommand(
                        () => TransformCurrentObject = (x, y, z) =>
                            {
                                if (SelectedObject == null)
                                    return;

                                var transformation = GetCurrentObjectsLinearTransformation();

                                transformation.RotateX += x;
                                transformation.RotateY += y;
                                transformation.RotateZ += z;
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
                    _switchToScaleCommand = new RelayCommand(
                        () => TransformCurrentObject = (x, y, z) =>
                            {
                                if (SelectedObject == null)
                                    return;

                                var transformation = GetCurrentObjectsLinearTransformation();

                                transformation.ScaleX += x;
                                transformation.ScaleY += y;
                                transformation.ScaleZ += z;
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
                    _createTeapotCommand = new RelayCommand(CreateObjectModel<Teapot>, () => true);

                return _createTeapotCommand;
            }
        }

        public ICommand CreateCubeCommand
        {
            get
            {
                if (_createCubeCommand == null)
                    _createCubeCommand = new RelayCommand(CreateObjectModel<Cube>, () => true);

                return _createCubeCommand;
            }
        }

        public ICommand CreateSphereCommand
        {
            get
            {
                if (_createSphereCommand == null)
                    _createSphereCommand = new RelayCommand(CreateObjectModel<Sphere>, () => true);

                return _createSphereCommand;
            }
        }

        public ICommand CreateCylinderCommand
        {
            get
            {
                if (_createCylinderCommand == null)
                    _createCylinderCommand = new RelayCommand(CreateObjectModel<Cylinder>, () => true);

                return _createCylinderCommand;
            }
        }

        public ICommand CreateDiscCommand
        {
            get
            {
                if (_createDiscCommand == null)
                    _createDiscCommand = new RelayCommand(CreateObjectModel<Disk>, () => true);

                return _createDiscCommand;
            }
        }

        #endregion

        #region Private Fields

        private void CreateObjectModel<T>() where T : SceneElement, new()
        {
            var model = new ObjectModel(new T());

            _objects.Add(model);
        }

        private LinearTransformation GetCurrentObjectsLinearTransformation()
        {
            LinearTransformation transformation;

            if (SelectedObject.Transformation == null)
            {
                transformation = new LinearTransformation();
                SelectedObject.Transformation = transformation;
            }
            else
            {
                transformation = SelectedObject.Transformation;
            }

            return transformation;
        }

        #endregion

    }
}
