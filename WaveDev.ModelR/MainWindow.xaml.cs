using System;
using System.Windows;
using System.Windows.Input;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Transformations;
using WaveDev.ModelR.ViewModels;

namespace WaveDev.ModelR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SceneModel _model;
        private Point _lastPosition;
        private bool _leftButtonDown;
        private Point _positionDelta;

        public MainWindow()
        {
            InitializeComponent();

            _model = new SceneModel();

            DataContext = _model;

            MenuPopup.Loaded += (s, ee) =>
            {
                MenuPopup.IsOpen = true;
                MenuPopup.IsOpen = false;
            };
        }

        private void OnOpenGlControlDraw(object sender, OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Move Left And Into The Screen
            gl.LoadIdentity();
            gl.Rotate(20.0f, 1.0f, 0.0f, 0.0f);
            gl.Rotate(-30.0f, 0.0f, 1.0f, 0.0f);
            gl.Translate(-5.0f, -5.0f, -10.0f);

            foreach (var model in _model.SceneObjectModels)
            {
                var renderable = model.SceneElement as IRenderable;

                if (renderable != null)
                    renderable.Render(gl, RenderMode.Design);


                var polygon = model.SceneElement as Polygon;

                if (polygon != null && _leftButtonDown)
                {
                    //polygon.PushObjectSpace(gl);

                    var transformation = new LinearTransformation();

                    transformation.TranslateX = (float)_positionDelta.X / 10.0f;
                    transformation.TranslateY = (float)_positionDelta.Y / 10.0f;

                    transformation.Transform(gl);

                    //polygon.PopObjectSpace(gl);
                }

            }
        }

        private void OnOpenGlControlInitialized(object sender, OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);

            var globalAmbient = new[] { 0.5f, 0.5f, 0.5f, 1.0f };
            var light0Position = new[] { 0.0f, 5.0f, 10.0f, 1.0f };
            var light0Ambient = new[] { 0.2f, 0.2f, 0.2f, 1.0f };
            var light0Diffuse = new[] { 0.3f, 0.3f, 0.3f, 1.0f };
            var light0Specular = new[] { 0.8f, 0.8f, 0.8f, 1.0f };
            var lightAmbient = new[] { 0.2f, 0.2f, 0.2f, 1.0f };

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lightAmbient);
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, globalAmbient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0Position);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0Ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0Diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0Specular);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        private void OnOpenGlControlMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);

            MenuPopup.HorizontalOffset = position.X - (MenuPopup.RenderSize.Width / 2);
            MenuPopup.VerticalOffset = position.Y - (MenuPopup.RenderSize.Height / 2);
            MenuPopup.IsOpen = true;
            MenuPopup.InvalidateVisual();
        }

        private void OnMenuPopupClosed(object sender, EventArgs e)
        {
            MenuPopup.IsOpen = false;
        }

        private void OnOpenGLControlMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _leftButtonDown = true;
        }

        private void OnOpenGLControlMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _leftButtonDown = false;
        }

        private void OnOpenGLControlMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(OpenGLControl);

            _positionDelta = new Point(position.X - _lastPosition.X, position.Y - _lastPosition.Y);

            _lastPosition = position;
        }
    }
}
