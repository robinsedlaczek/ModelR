using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using SharpGL.SceneGraph.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WaveDev.ModelR.Communication;
using WaveDev.ModelR.ViewModels;

namespace WaveDev.ModelR.Scripting
{
    internal class ScriptingManager
    {
        internal static async Task ExecuteScript(string scriptCode, bool forceExecution)
        {
            var sceneModel = ViewModelLocator.Scene;

            try
            {
                var references = new Assembly[]
                {
                    // [RS] To use SharpGL types.
                    typeof(SceneElement).Assembly, 
                    // [RS] To use types from the UI, e.g. the view models.
                    typeof(SceneModel).Assembly
                };

                var imports = new string[]
                {
                    "SharpGL.SceneGraph.Core",
                    "SharpGL.SceneGraph.Quadrics",
                    "SharpGL.SceneGraph.Transformations",
                    "WaveDev.ModelR.ViewModels"
                };

                var scriptOptions = ScriptOptions.Default
                    .WithReferences(references)
                    .WithImports(imports);

                var context = new ScriptingContext(sceneModel.UserModels, sceneModel.SceneObjectModels);
                var globals = new ScriptingGlobals(context);

                var assemblyLoader = new InteractiveAssemblyLoader();

                var script = CSharpScript.Create(scriptCode, scriptOptions, typeof(ScriptingGlobals), assemblyLoader);
                var diagnostics = script.Compile();

                if (!forceExecution && diagnostics.Count() > 0)
                    return;

                var state = await script.RunAsync(globals);

                await ProcessScriptResult(sceneModel, state);
            }
            catch (Exception exception)
            {
                var message = new MessageViewModel(exception.Message);

                sceneModel.Errors.Add(message);
            }
        }

        private static async Task ProcessScriptResult(SceneModel sceneModel, ScriptState<object> state)
        {
            foreach (var variable in state.Variables)
            {
                if (variable.Value is SceneObjectModel)
                {
                    var model = variable.Value as SceneObjectModel;
                    sceneModel.SceneObjectModels.Add(model);
                    sceneModel.SelectedObject = model;
                    await ModelRHubClientProxy.GetInstance().CreateSceneObject(model);
                }
            }
        }
    }
}
