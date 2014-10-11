using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Transformations;
using WaveDev.ModelR.ViewModels;
using SharpGL.WPF;

namespace WaveDev.ModelR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private SceneModel _model;
        private bool _leftButtonDown;
        private double[] _lastPosition;
        private float[] _positionDelta;
        private bool _leftAltKeyPressed;

        #endregion

        #region Constrcution

        public MainWindow()
        {
            InitializeComponent();

            _lastPosition = new[] { 0.0, 0.0, 0.0 };
            _positionDelta = new[] { 0f, 0f, 0f };
            _model = new SceneModel();

            DataContext = _model;

            MenuPopup.Loaded += (s, ee) =>
            {
                MenuPopup.IsOpen = true;
                MenuPopup.IsOpen = false;
            };
        }

        #endregion

        #region Event Handler

        private void OnOpenGlControlDraw(object sender, OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Move Left And Into The Screen
            gl.LoadIdentity();
            gl.Rotate(20.0f, 1.0f, 0.0f, 0.0f);
            gl.Rotate(-30.0f, 0.0f, 1.0f, 0.0f);
            gl.Translate(-7.0f, -5.0f, -13.0f);

            _model.WorldAxies.Render(gl, RenderMode.Design);

            gl.PushMatrix();
            gl.Rotate(90d, 1d, 0d, 0d);
            _model.OrientationGrid.Render(gl, RenderMode.Design);
            gl.PopMatrix();

            foreach (var model in _model.SceneObjectModels)
            {
                var context = model.SceneElement as IHasOpenGLContext;

                if (context != null && context.CurrentOpenGLContext == null)
                    context.CreateInContext(gl);
                
                var renderable = model.SceneElement as IRenderable;
                var transformable = model.SceneElement as IHasObjectSpace;

                if (transformable != null)
                    transformable.PushObjectSpace(gl);

                if (renderable != null)
                    renderable.Render(gl, RenderMode.Design);

                if (transformable != null)
                    transformable.PopObjectSpace(gl);
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
            var position = PointToScreen(e.GetPosition(this));

            MenuPopup.HorizontalOffset = position.X - (RadialMenu.RenderSize.Width / 2);
            MenuPopup.VerticalOffset = position.Y - (RadialMenu.RenderSize.Height / 2);
            MenuPopup.IsOpen = true;
            MenuPopup.InvalidateVisual();
        }

        private void OnMenuPopupClosed(object sender, EventArgs e)
        {
            MenuPopup.IsOpen = false;
        }

        private void OnOpenGLControlMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.None;
            var capture = Mouse.Capture(OpenGLControl);

            _leftButtonDown = true;
            _lastPosition = ConvertMousePositionToSceneCoordinates((OpenGLControl)sender, e.GetPosition(OpenGLControl).X, e.GetPosition(OpenGLControl).Y, 0.0f);
        }

        private void OnOpenGLControlMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Arrow; ;
            Mouse.Capture(null);

            _leftButtonDown = false;
            _positionDelta = new[] { 0f, 0f, 0f };
        }

        private void OnOpenGLControlMouseMove(object sender, MouseEventArgs e)
        {
            if (_leftButtonDown)
            {
                var coordinates = ConvertMousePositionToSceneCoordinates((OpenGLControl)sender, e.GetPosition(OpenGLControl).X, e.GetPosition(OpenGLControl).Y, 0f);

                if (_leftAltKeyPressed)
                    _positionDelta = new[] { (float)(coordinates[0] - _lastPosition[0]), (float)(coordinates[1] - _lastPosition[1]), 0.0f };
                else
                    _positionDelta = new[] { (float)(coordinates[0] - _lastPosition[0]), 0.0f, (float)(coordinates[1] - _lastPosition[1]) };

                _model.TransformCurrentObject(100 * _positionDelta[0], 100 * _positionDelta[1], 100 * _positionDelta[2]);
                _lastPosition = coordinates;
            }
        }

        private void OnRadialMenuItemClick(object sender, EventArgs e)
        {
            MenuPopup.IsOpen = false;
            MenuPopup.InvalidateVisual();
        }

        private void OnOpenGLControlKeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt)
                _leftAltKeyPressed = true;
        }

        private void OnOpenGLControlKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt)
                _leftAltKeyPressed = false;
        }

        #endregion

        #region Private Methods

        private double[] ConvertMousePositionToSceneCoordinates(OpenGLControl control, double x, double y, double z)
        {
            if (control == null)
                return new[] { 0d, 0d, 0d };

            var gl = control.OpenGL;
            var position = new[] { x, y, z };
            var coordinates = gl.UnProject(position[0], position[1], position[2]);

            return coordinates;
        }

        #endregion
    }
}
