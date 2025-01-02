using Develop.Scripts.Core.Lobby;
using DI;
using UnityEngine;

public class MainMenuContext : SceneContext
{
    [SerializeField] private LobbyView _lobbyView;
    [SerializeField] private LobbyModel _lobbyModel;
    public override void RegisterDependencies()
    {
        RegisterFromInstance(_lobbyView);
        RegisterFromInstance(_lobbyModel);
    }
}
