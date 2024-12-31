using Develop.Scripts.Services.Abstractions;
using Develop.Scripts.Services.Behaviours;
using DI;

namespace Develop.Scripts.Contexts
{
    public class HTLProjectContext : ProjectContext
    {
        public override void RegisterDependencies()
        {
            RegisterSceneLoader();
            //Other dependencies;
        }

        private void RegisterSceneLoader() => Register<ISceneLoader, SceneLoader>(false);
    }
}
