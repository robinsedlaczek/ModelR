using System;
using System.Windows.Media.Imaging;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Transformations;

namespace WaveDev.ModelR.ViewModels
{
    public class ObjectModel
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
                _transformation = value;

                var polygon = SceneElement as Polygon;
                if (polygon != null)
                    polygon.Transformation = _transformation;
                else
                {
                    var quadric = SceneElement as Quadric;
                    //quadric.Transformation = _transformation;
                }

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
