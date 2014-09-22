using System.Windows;
using System.Windows.Input;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;

namespace WaveDev.ModelR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private float _rotation = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnOpenGLControlDraw(object sender, OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Move Left And Into The Screen
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -6.0f);


            gl.Rotate(_rotation, 0.0f, 1.0f, 0.0f);

            Teapot tp = new Teapot();
            tp.Draw(gl, 14, 1, OpenGL.GL_FILL);

            _rotation += 3.0f;
        }

        private void OnOpenGLControlInitialized(object sender, OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);

            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 0.0f, 5.0f, 10.0f, 1.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        private void OnOpenGLControlMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (RadialMenu.Visibility == Visibility.Hidden || RadialMenu.Visibility == Visibility.Collapsed)
            {
                RadialMenu.Visibility = Visibility.Visible;
                RadialMenu.IsOpen = true;
            }
            else
            {
                RadialMenu.IsOpen = false;
                RadialMenu.Visibility = Visibility.Hidden;
            }
        }

        private void OnOpenGLControlMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RadialMenu.Visibility = Visibility.Hidden;

        }
    }
}
