using Newtonsoft.Json;

namespace WaveDev.ModelR.Shared.Models
{
    public class UserInfoModel
    {
        public UserInfoModel(string userName, string connectionId, byte[] image)
        {
            UserName = userName;
            ConnectionId = connectionId;
            Image = image;
        }

        [JsonIgnore]
        public string ConnectionId
        {
            get;
            private set;
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
