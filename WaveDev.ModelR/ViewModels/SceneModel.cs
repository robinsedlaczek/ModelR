using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using System;
using SharpGL.SceneGraph.Transformations;
using WaveDev.ModelR.Annotations;
using GalaSoft.MvvmLight;

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
                    _switchToRotationCommand = new RelayCommand(parameter => TransformCurrentObject = 
                        (x, y, z) =>
                            {
                                if (SelectedObject == null)
                                    return;

                                var transformation = GetCurrentObjectsLinearTransformation();

                                transformation.RotateX += 10 * x;
                                transformation.RotateY += 10 * y;
                                transformation.RotateZ += 10 * z;
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

        #region Private Fields

        private ObjectModel CreateObjectModel<T>() where T : SceneElement, new()
        {
            var model = new ObjectModel(new T());

            _objects.Add(model);

            if (_objects.Count == 1)
                SelectedObject = model;

            return model;
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
