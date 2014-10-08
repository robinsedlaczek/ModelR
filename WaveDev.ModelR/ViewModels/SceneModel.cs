using System.Collections.ObjectModel;
using System.Windows.Input;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;

namespace WaveDev.ModelR.ViewModels
{
    public class SceneModel
    {
        private ObservableCollection<ObjectModel> _objects;
        private RelayCommand _createTeapotCommand;
        private RelayCommand _createCubeCommand;
        private RelayCommand _createSphereCommand;
        private RelayCommand _createCylinderCommand;
        private RelayCommand _createDiscCommand;

        public SceneModel()
        {
            _objects = new ObservableCollection<ObjectModel>();

            CreateObjectModel<Axies>();
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
            get;
            set;
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

        private void CreateObjectModel<T>() where T : SceneElement, new()
        {
            var model = new ObjectModel(new T());

            _objects.Add(model);
        }

    }
}
