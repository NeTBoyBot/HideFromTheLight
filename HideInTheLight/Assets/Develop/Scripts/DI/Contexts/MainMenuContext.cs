using Develop.Scripts.Core.Lobby;
using DI;
using UnityEngine;

public class MainMenuContext : SceneContext
{
    [SerializeField] private LobbyModel _lobbyModel;
    [SerializeField] private LobbyView _lobbyView;
    public override void RegisterDependencies()
    {
        RegisterFromInstance(_lobbyModel);
        RegisterFromInstance(_lobbyView);
    }
}
