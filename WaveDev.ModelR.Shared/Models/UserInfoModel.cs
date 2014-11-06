namespace WaveDev.ModelR.Shared.Models
{
    public class UserInfoModel
    {
        public UserInfoModel(string userName, byte[] image)
        {
            UserName = userName;
            Image = image;
        }

        public string UserName
        {
            get;
            private set;
        }

        public byte[] Image
        {
            get;
            private set;
        }
    }
}
