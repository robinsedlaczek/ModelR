using System;
using System.Collections.Generic;

namespace WaveDev.ModelR.Shared.Models
{
    public class SceneInfoModel
    {
        public SceneInfoModel(Guid id)
        {
            Id = id;

            SceneObjectInfoModels= new List<SceneObjectInfoModel>();
            UserInfoModels = new List<UserInfoModel>();
        }

        public Guid Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public IList<SceneObjectInfoModel> SceneObjectInfoModels
        {
            get;
            private set;
        }

        public IList<UserInfoModel> UserInfoModels
        {
            get;
            private set;
        }
    }
}
