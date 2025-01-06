using DI;
using Mirror;
using UnityEngine;
public enum PlayerRole
{
    None,
    Human,
    Monster
}
namespace Develop.Scripts.Core.Lobby
{
    public class LobbyPresenter : NetworkRoomManager
    {
        [Inject] private LobbyView _view;
        [Inject] private LobbyModel _model;


        #region Initialize
        private void SubscribeEvents()
        {
            _model.OnNameAssigned += TargetRpcShowLobbyGUI;
            _model.OnRoleSelected += _view.ChangePlayerInRoom;
        }

        #endregion

        #region Connecting
        /// <summary>
        /// Вызывается раньше, чем OnServerReady, на текущий момент самый ранний метод с параметром conn
        /// </summary>
        /// <param name="conn"></param>
        public override void OnRoomServerConnect(NetworkConnectionToClient conn)
        {
            base.OnRoomServerConnect(conn);

            Debug.Log("ON ROOM SERVER CONNECTED");

            if (!TryInitializeLobby())
                return;

            _model.EnableCursor();

            if (_model.HasMonsterInRoom())
            {
                _model.RpcMonsterSelectBtnSetActive(false);
            }
        }
        #endregion

        #region Disconnecting
        public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
        {
            Debug.Log($"Room Disconnected ID : {conn.connectionId}");

            var roomPlayer = conn.identity.GetComponent<NetworkRoomMyPlayer>();

            if (roomPlayer != null && roomPlayer.RoleName == "Monster")
            {
                _model.RpcMonsterSelectBtnSetActive(true);
            }

            base.OnRoomServerDisconnect(conn);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            Debug.Log($"Server Disconnected ID : {conn.connectionId}");

            base.OnServerDisconnect(conn);
        }
        #endregion

        #region Lobby/Server main
        /// <summary>
        /// Вызывается раньше, чем OnServerAddPlayer и при каждом переходе на любую из сцен
        /// </summary>
        /// <param name="conn"></param>

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            Debug.Log("ON SERVER READY");
            base.OnServerReady(conn);

            if (!TryInitializeLobby())
                return;

            SubscribeEvents();

            _model.TargetRpcLobbyPanelSetActive(conn, true);
            _model.TargetRpcSetupInput(conn);

            if (_model.HasMonsterInRoom())
            {
                _model.RpcMonsterSelectBtnSetActive(false);
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            if (!TryInitializeLobby())
                return;

            SubscribeEvents();

            var roomPlayer = conn.identity.GetComponent<NetworkRoomMyPlayer>();

            Debug.Log($"<color=green>OnServerAddPlayer</color> | id = <color=cyan>{conn.connectionId}</color>");

            _view.RpcSetSpawnPosition(roomPlayer);
            _view.ChangePlayerInRoom(conn, PlayerRole.Human);

            _model.TargetRpcNicknamePanelSetActive(conn, true);
            _model.TargetRpcSetupInput(conn);
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            var roomPlayerInfo = roomPlayer.GetComponent<NetworkRoomMyPlayer>();
            var gamePlayerInfo = gamePlayer.GetComponent<PlayerIdentification>();

            gamePlayerInfo.Initialize(roomPlayerInfo.Name, roomPlayerInfo.index, roomPlayerInfo.RoleName);

            Debug.Log($"<color=green>OnRoomServerSceneLoadedForPlayer</color> | Player <color=yellow>{gamePlayerInfo.PlayerName}</color> successfully loaded into the game!");

            return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
        }
        #endregion

        #region Handlers
        public void TargetRpcShowLobbyGUI(string nickname, NetworkConnectionToClient conn) => _model.TargetRpcLobbyPanelSetActive(conn, true);

        private bool TryInitializeLobby()
        {
            if (!LobbyInitialized())
            {
                Debug.LogWarning("Model or View has null, lobby not initialized!");
                return false;
            }
            return true;
        }
        public bool LobbyInitialized() => _model != null && _view != null;
        #endregion
    }
}
