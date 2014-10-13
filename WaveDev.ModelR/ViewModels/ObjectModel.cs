using System;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Transformations;

namespace WaveDev.ModelR.ViewModels
{
    public class ObjectModel : ViewModelBase
    {
        private LinearTransformation _transformation;

        public ObjectModel(SceneElement sceneElement)
        {
            if (sceneElement == null)
                throw new ArgumentNullException("sceneElement");

            SceneElement = sceneElement;
        }

        public SceneElement SceneElement
        {
            get;
            set;
        }

        public LinearTransformation Transformation
        {
            get
            {
                return _transformation;
            }

            set
            {
                var objectSpace = SceneElement as IHasObjectSpace;

                if (objectSpace != null)
                    objectSpace.Transformation = value;

                Set(ref _transformation, value);
            }
        }

        public string Name
        {
            get
            {
                return SceneElement.Name;
            }

            set
            {
                SceneElement.Name = value;

                RaisePropertyChanged();
            }
        }

        public BitmapImage Image
        {
            get
            {
                BitmapImage image = null;

                if (SceneElement is Cube)
                {
                    var uri = new Uri("/WaveDev.ModelR;component/Images/Cube.png", UriKind.Relative);
                    image = new BitmapImage(uri);
                }
                else if (SceneElement is Teapot)
                {
                    var uri = new Uri("/WaveDev.ModelR;component/Images/Teapot.png", UriKind.Relative);
                    image = new BitmapImage(uri);
                }
                else if (SceneElement is Sphere)
                {
                    var uri = new Uri("/WaveDev.ModelR;component/Images/Sphere.png", UriKind.Relative);
                    image = new BitmapImage(uri);
                }
                else if (SceneElement is Cylinder)
                {
                    var uri = new Uri("/WaveDev.ModelR;component/Images/Cylinder.png", UriKind.Relative);
                    image = new BitmapImage(uri);
                }
                else if (SceneElement is Disk)
                {
                    var uri = new Uri("/WaveDev.ModelR;component/Images/Disk.png", UriKind.Relative);
                    image = new BitmapImage(uri);
                }

                return image;
            }
        }

    }
}
