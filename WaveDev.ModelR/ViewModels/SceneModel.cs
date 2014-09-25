using System.Collections.ObjectModel;
using System.Windows.Input;
using SharpGL.SceneGraph.Primitives;

namespace WaveDev.ModelR.ViewModels
{
    public class SceneModel
    {
        private ObservableCollection<ObjectModel> _objects;
        private RelayCommand _createTeapotCommand;
        private RelayCommand _createCubeCommand;

        public ObservableCollection<ObjectModel> SceneObjectModels
        {
            get
            {
                if (_objects == null)
                    _objects = new ObservableCollection<ObjectModel>();

                return _objects;
            }
        }

        public ICommand CreateTeapotCommand
        {
            get
            {
                if (_createTeapotCommand == null)
                    _createTeapotCommand = new RelayCommand(CreateTeapot, () => true);

                return _createTeapotCommand;
            }
        }

        public ICommand CreateCubeCommand
        {
            get
            {
                if (_createCubeCommand == null)
                    _createCubeCommand = new RelayCommand(CreateCube, () => true);

                return _createCubeCommand;
            }
        }

        private void CreateCube()
        {
            var cube = new Cube();
            var model = new ObjectModel(cube);

            _objects.Add(model);
        }

        private void CreateTeapot()
        {
            var teapot = new Teapot();
            var model = new ObjectModel(teapot);

            _objects.Add(model);
        }
    }
}
