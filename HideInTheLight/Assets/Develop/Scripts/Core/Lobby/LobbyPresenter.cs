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
        public override void Start()
        {
            base.Start();
            SubscribeEvents();
        }
        private void SubscribeEvents()
        {
            _model.OnNameAssigned += TargetRpcShowLobbyGUI;
        }

        #endregion

        #region NetworkManager


        //Вызывается раньше, чем OnServerReady
        public override void OnRoomServerConnect(NetworkConnectionToClient conn)
        {
            base.OnRoomServerConnect(conn);
        }

        public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
        {
            Debug.Log($"Disconnected : {conn.connectionId}");
            var roomPlayer = conn.identity.GetComponent<NetworkRoomPlayer>();

            if(roomPlayer != null && roomPlayer.RoleName == "Monster")
            {
                _model.RpcMonsterSelectBtnSetActive(true);
            }

            base.OnRoomServerDisconnect(conn);
        }

        //Вызывается раньше, чем OnServerAddPlayer
        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);

            _model.TargetRpcLobbyPanelSetActive(conn, true);
            _model.TargetRpcSetupInput(conn);
        }


        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            var roomPlayer = conn.identity.GetComponent<NetworkRoomPlayer>();

            Debug.Log($"<color=green>OnServerAddPlayer</color> | id = <color=cyan>{conn.connectionId}</color>");
            _view.RpcSetSpawnPosition(roomPlayer);

            _model.TargetRpcNicknamePanelSetActive(conn, true);
            _model.TargetRpcSetupInput(conn);
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            var roomPlayerInfo = roomPlayer.GetComponent<NetworkRoomPlayer>();
            var gamePlayerInfo = gamePlayer.GetComponent<PlayerIdentification>();

            gamePlayerInfo.Initialize(roomPlayerInfo.Name, roomPlayerInfo.index, roomPlayerInfo.RoleName);

            if(gamePlayerInfo.PlayerRole == PlayerRole.Monster)
            {
                Debug.Log($"Player {gamePlayerInfo.PlayerName} is monster!");
            }

            Debug.Log($"<color=green>OnRoomServerSceneLoadedForPlayer</color> | Player <color=yellow>{gamePlayerInfo.PlayerName}</color> successfully loaded into the game!");

            return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
        }
        #endregion

        #region Handlers
        public void TargetRpcShowLobbyGUI(string nickname, NetworkConnectionToClient conn) => _model.TargetRpcLobbyPanelSetActive(conn, true);
        #endregion
    }
}
