using DI;
using Mirror;
using UnityEngine;

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
            _view.OnNameAssigned += ShowLobbyGUI;
        }

        #endregion

        #region NetworkManager

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            Debug.Log($"<color=green>OnServerAddPlayer</color> | id = <color=cyan>{conn.connectionId}</color>");

            //var roomPlayer = conn.identity.gameObject.GetComponent<NetworkRoomPlayer>();
            //Debug.Log($"RoomPlayer object = {roomPlayer}", roomPlayer);

            _view.TargetRpcNicknamePanelSetActive(conn, true);
            _view.TargetRpcSetupInput(conn);
        }


        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            var roomPlayerInfo = roomPlayer.GetComponent<NetworkRoomPlayer>();
            var gamePlayerInfo = gamePlayer.GetComponent<PlayerIdentification>();

            gamePlayerInfo.Initialize(roomPlayerInfo.Name, roomPlayerInfo.index);

            Debug.Log($"<color=green>OnRoomServerSceneLoadedForPlayer</color> | Player <color=yellow>{gamePlayerInfo.PlayerName}</color> successfully loaded into the game!");

            return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
        }
        #endregion

        #region Handlers
        public void ShowLobbyGUI(string nickname, NetworkConnectionToClient conn) => _view.TargetRpcLobbyPanelSetActive(conn, true);
        #endregion
    }
}
