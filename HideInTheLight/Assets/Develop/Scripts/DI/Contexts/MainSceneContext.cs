using Develop.Scripts.Entities.Player;
using DI;
using UnityEngine;

public class MainSceneContext : SceneContext
{
    public override void RegisterDependencies()
    {
        PlayerRegister();
    }
   
    private void PlayerRegister() => Register<PlayerModel>(false);
}
