﻿using System;
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

namespace WaveDev.ModelR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields

        private SceneModel _model;
        private Point _lastPosition;
        private bool _leftButtonDown;
        private Point _positionDelta;

        #endregion

        #region Constrcution

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
            gl.Translate(-5.0f, -5.0f, -10.0f);

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

            _lastPosition = e.GetPosition(OpenGLControl);
        }

        private void OnOpenGLControlMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Arrow; ;
            Mouse.Capture(null);

            _leftButtonDown = false;

            _positionDelta = new Point(0, 0);
        }

        private void OnOpenGLControlMouseMove(object sender, MouseEventArgs e)
        {
            if (_leftButtonDown)
            {
                var position = e.GetPosition(OpenGLControl);

                _positionDelta = new Point(position.X - _lastPosition.X, position.Y - _lastPosition.Y);

                Debug.WriteLine("Position-Delta: {0}, {1}", _positionDelta.X, _positionDelta.Y);

                _model.TransformCurrentObject((float)_positionDelta.X / 100.0f, (float)_positionDelta.Y / 100.0f, 0.0f);

                _lastPosition = position;
            }
        }

        private void OnRadialMenuItemClick(object sender, EventArgs e)
        {
            MenuPopup.IsOpen = false;
            MenuPopup.InvalidateVisual();
        }

        #endregion
    }
}
