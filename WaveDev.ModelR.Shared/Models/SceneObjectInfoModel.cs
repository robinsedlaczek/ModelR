using System;

namespace WaveDev.ModelR.Shared.Models
{
    public class SceneObjectInfoModel
    {
        public SceneObjectInfoModel(Guid id, Guid sceneId)
        {
            Id = id;
            SceneId = sceneId;
        }

        public Guid Id { get; set; }

        public Guid SceneId { get; set; }

        public SceneObjectType SceneObjectType { get; set; }

        public string Name { get; set; }

        public TransformationInfoModel Transformation { get; set; }

    }
}
