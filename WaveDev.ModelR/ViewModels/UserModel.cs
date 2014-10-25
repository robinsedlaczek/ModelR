using System;
using GalaSoft.MvvmLight;

namespace WaveDev.ModelR.ViewModels
{
    public class UserModel : ViewModelBase
    {
        #region Private Fields

        private Guid _id;
        private string _userName;

        #endregion

        #region Construction

        public UserModel(string userName, byte[] image, Guid? id = null)
        {
            if (id == null)
                Id = Guid.NewGuid();
            else
                Id = id.Value;

            UserName = userName;
            Image = image;
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

        public byte[] Image
        {
            get;
            private set;
        }

        #endregion
    }
}
