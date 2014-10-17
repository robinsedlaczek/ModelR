using System;

namespace WaveDev.ModelR.Shared.Models
{
    public class SceneObjectInfoModel
    {
        public SceneObjectInfoModel(Guid id, Guid sceneId, SceneObjectType sceneObjectType)
        {
            Id = id;
            SceneId = sceneId;
            SceneObjectType = sceneObjectType;
        }

        public Guid Id { get; set; }

        public Guid SceneId { get; set; }

        public SceneObjectType SceneObjectType { get; set; }

        public string Name { get; set; }

    }
}
