using System.Collections.Generic;
using System.Collections.ObjectModel;
using WaveDev.ModelR.ViewModels;

namespace WaveDev.ModelR.Scripting
{
    public class ScriptingContext
    {
        public ScriptingContext(ObservableCollection<UserModel> users, ObservableCollection<SceneObjectModel> sceneObjects)
        {
            Users = users;
            SceneObjects = sceneObjects;
        }

        public ObservableCollection<UserModel> Users { get; }

        public ObservableCollection<SceneObjectModel> SceneObjects { get; }
    }
}