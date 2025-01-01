using Develop.Scripts.Bootstrap;
using Mirror;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCustomize : NetworkBehaviour
{
    [Header("Nickname settings")]
    [SerializeField] private Image _playerCutomizePanel;
    [SerializeField] private Button _playerAcceptButton;
    [SerializeField] private TMP_InputField NicknameField;
    [Header("Other settings")]
    [SerializeField] private Image _lobbyPanel;

    private NetworkConnectionToClient _connection;
    private LobbyModel _playerModel;
    private NetworkRoomPlayer _roomPlayer;
    private LobbyManager _lobby;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isLocalPlayer || isServer)
        {
            CusomizePanelSetActive(true);
            LobbyPanelSetActive(false);
        }
    }

    public void Initialize(NetworkConnectionToClient conn, LobbyModel playerModel, NetworkRoomPlayer roomPlayer, LobbyManager lobby)
    {
        _connection = conn;
        _playerModel = playerModel;
        _roomPlayer = roomPlayer;
        _lobby = lobby;

        _playerAcceptButton.onClick.RemoveAllListeners();
        _playerAcceptButton.onClick.AddListener(AcceptNicknameButton);

        Debug.Log("SUCCESSFULLY INITIALIZE PLAYER CUSOMIZE");
    }

    [Command(requiresAuthority = false)]
    private void CmdSetNickname(string nickname) => RpcUpdateNickname(nickname);

    [ClientRpc]
    private void RpcUpdateNickname(string nickname)
    {
        if (_playerModel != null)
            _playerModel.Name = nickname;
        if (_roomPlayer != null)
            _roomPlayer.SetName(nickname);
    }

    public void AcceptNicknameButton()
    {
        if (_lobby == null || string.IsNullOrEmpty(NicknameField.text) || NicknameField.text.Length < 3)
            return;

        var player = _lobby._container.PlayerRolesInfo.First(p => p.index == _connection.connectionId);
        if (player == null)
        {
            Debug.Log("Player is null!");
            return;
        }

        CmdSetNickname(NicknameField.text);

        CusomizePanelSetActive(false);
        LobbyPanelSetActive(true);

        if (isLocalPlayer)
            _lobby.EnableLobbyGUI();
    }

    public void CusomizePanelSetActive(bool value) => _playerCutomizePanel.gameObject.SetActive(value);
    public void LobbyPanelSetActive(bool value) => _lobbyPanel.gameObject.SetActive(value);
}
