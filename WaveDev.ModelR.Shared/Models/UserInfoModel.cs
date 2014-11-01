using System;
using System.Drawing;
using System.Linq;

namespace WaveDev.ModelR.Shared.Models
{
    public class UserInfoModel
    {
        public UserInfoModel(string userName, byte[] image)
        {
            UserName = userName;
            Image = image;

            var random = new Random(Constants.UserColors.Count());
            var colorIndex = random.Next(0, 9);

            Color = Constants.UserColors[colorIndex];
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

        public Color Color
        {
            get;
            private set;
        }
    }
}
