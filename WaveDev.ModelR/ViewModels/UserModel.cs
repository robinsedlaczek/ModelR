using System;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Transformations;

namespace WaveDev.ModelR.ViewModels
{
    public class UserModel : ViewModelBase
    {
        #region Private Fields

        private Guid _id;
        private string _userName;

        #endregion

        #region Construction

        public UserModel(string userName, Guid? id = null)
        {
            if (id == null)
                Id = Guid.NewGuid();
            else
                Id = id.Value;

            UserName = UserName;
        }

        #endregion

        #region Public Members

        public string UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                Set(ref _userName, value);
            }
        }

        public Guid Id
        {
            get
            {
                return _id;
                
            }

            set
            {
                Set(ref _id, value);
                
            } 
            
        }

        public BitmapImage Image
        {
            get
            {
                BitmapImage image = null;

                var uri = new Uri("/WaveDev.ModelR;component/Images/User.png", UriKind.Relative);
                image = new BitmapImage(uri);

                return image;
            }
        }

        #endregion
    }
}
