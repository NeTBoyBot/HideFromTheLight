using Develop.Scripts.Core.Lobby;
using Develop.Scripts.Services.Abstractions;
using Develop.Scripts.Services.Behaviours;
using DI;
using UnityEngine;

namespace Develop.Scripts.Contexts
{
    public class HTLProjectContext : ProjectContext
    {
        public override void RegisterDependencies()
        {
            Register<ISceneLoader, SceneLoader>(false);
        }
    }
}
