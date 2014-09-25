using System.Collections.ObjectModel;
using System.Windows.Input;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;

namespace WaveDev.ModelR.ViewModels
{
    public class DesignTimeModel
    {
        private ObservableCollection<SceneElement> _objects;

        public DesignTimeModel()
        {
            LoadDesignTimeModel();    
        }

        public static DesignTimeModel DataContext
        {
            get
            {
                return new DesignTimeModel();
            }
        }

        private void LoadDesignTimeModel()
        {
            var cube = new Cube() {Name = "Cube 1"};
            SceneObjectModels.Add(cube);

            var teapot = new Teapot() {Name = "Teapot 1"};
            SceneObjectModels.Add(teapot);

            var sphere = new Sphere() {Name = "Sphere 1"};
            SceneObjectModels.Add(sphere);
        }

        public ObservableCollection<SceneElement> SceneObjectModels
        {
            get
            {
                if (_objects == null)
                {
                    _objects = new ObservableCollection<SceneElement>();

                    LoadDesignTimeModel();
                }

                return _objects;
            }
        }

    }
}
