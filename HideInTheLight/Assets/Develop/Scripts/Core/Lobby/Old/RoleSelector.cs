using Mirror;

namespace Develop.Scripts.Core.Lobby.Old
{
    public class RoleSelector : NetworkBehaviour
    {
        //    [SerializeField] private LobbyView _lobbyView;
        //    private LobbyManager _lobbyManager;

        //    public static int LocalClientId = -1;


        //    #region Initialize

        //    private void Start()
        //    {
        //        Debug.Log("START");
        //        _lobbyView = FindObjectOfType<LobbyView>();
        //        _lobbyManager = FindObjectOfType<LobbyManager>();

        //        SubscribeEvents();
        //    }

        //    private void SubscribeEvents()
        //    {
        //        _lobbyView.HumanButton.onClick.RemoveAllListeners();
        //        _lobbyView.MonsterButton.onClick.RemoveAllListeners();

        //        _lobbyView.HumanButton.onClick.AddListener(OnHumanSelected);
        //        _lobbyView.MonsterButton.onClick.AddListener(OnMonsterSelected);
        //    }
        //    #endregion

        //    #region MainLogic

        //    public override void OnStartLocalPlayer()
        //    {
        //        base.OnStartLocalPlayer();
        //        RpcUpdateMonsterButtonState(!ContainsMonsterInPlayers());
        //    }

        //    [Command(requiresAuthority = false)]
        //    public void CmdSelectRole(int connectionId,PlayerRole role)
        //    {
        //        if (role == PlayerRole.Monster && _lobbyManager._container.PlayerRolesInfo.Any(p => p.role == PlayerRole.Monster))
        //        {
        //            _lobbyView.Chat.text += $"\n<color=red>Monster already selected!</color>";
        //            RpcUpdateMonsterButtonState(false);
        //            return;
        //        }

        //        if (role == PlayerRole.Monster)
        //        {
        //            RpcUpdateMonsterButtonState(false);
        //        }
        //        if(role == PlayerRole.Human && _lobbyManager._container.PlayerRolesInfo.First(p=>p.index == connectionId).role == PlayerRole.Monster)
        //        {
        //            RpcUpdateMonsterButtonState(true);
        //        }

        //        _lobbyManager._container.PlayerRolesInfo.First(p=>p.index == connectionId).role = role;
        //        FindObjectsOfType<LobbyModel>().First(i => i.Id == connectionId).Role = role;

        //        RpcUpdateRoleSelection(connectionId, role);
        //    }

        //    [ClientRpc]
        //    void RpcUpdateRoleSelection(int connectionId, PlayerRole role)
        //    {
        //        foreach (var player in _lobbyManager.roomSlots)
        //        {
        //            var playerModel = player.GetComponent<LobbyModel>();
        //            if (playerModel.Id == connectionId)
        //            {
        //                _lobbyView.Chat.text += $"\n{playerModel.Name} select role - <color=yellow>{role}</color>";
        //                break;
        //            }
        //        }
        //    }

        //    [ClientRpc]
        //    void RpcUpdateMonsterButtonState(bool isInteractable) => _lobbyView.MonsterButton.interactable = isInteractable;
        //    #endregion

        //    #region Handlers
        //    public void OnHumanSelected()
        //    {
        //        if (!NetworkClient.isConnected)
        //            return;

        //        CmdSelectRole(NetworkClient.localPlayer.gameObject.GetComponent<LobbyModel>().Id,PlayerRole.Human);
        //    }

        //    public void OnMonsterSelected()
        //    {
        //        if (!NetworkClient.isConnected)
        //            return;

        //        CmdSelectRole(NetworkClient.localPlayer.gameObject.GetComponent<LobbyModel>().Id,PlayerRole.Monster);
        //    }
        //    #endregion

        //    public bool ContainsMonsterInPlayers() => _lobbyManager._container.PlayerRolesInfo.Any(p => p.role == PlayerRole.Monster);
        //}
    }
}
