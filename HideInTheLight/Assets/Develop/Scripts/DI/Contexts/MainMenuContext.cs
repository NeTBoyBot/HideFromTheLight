using Develop.Scripts.Core.Lobby;
using DI;   

public class MainMenuContext : SceneContext
{
    public override void RegisterDependencies()
    {
       RegisterFromScene<LobbyView>();
    }
}
