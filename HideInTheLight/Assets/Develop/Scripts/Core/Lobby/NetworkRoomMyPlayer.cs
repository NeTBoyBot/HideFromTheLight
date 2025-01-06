using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomMyPlayer : NetworkRoomPlayer
{

    [Header("Skin settings")]
    public GameObject LobbyPlayer;
    public GameObject LobbyMonster;
    [Space]
    public Button ReadyButton;
    public TMP_Text ReadyButtonText;
    [Space]
    public TMP_Text Nickname;
    public TMP_Text ReadyStateText;


    public override void Start()
    {
        base.Start();

        ReadyButton.onClick.RemoveAllListeners();
        ReadyButton.onClick.AddListener(CmdChangeReadyState);

        if (!isLocalPlayer)
            return;

        ReadyButton.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void RpcSetHumanSkin()
    {
        LobbyMonster.SetActive(false);
        LobbyPlayer.SetActive(true);
    }
    [ClientRpc]
    public void RpcSetMonsterSkin()
    {
        LobbyPlayer.SetActive(false);
        LobbyMonster.SetActive(true);
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        ReadyStateText.text = newReadyState ? "<color=green>Ready</color>" : "Not ready";
        ReadyButtonText.text = newReadyState ? "Cancel" : "Ready";
    }

    public override void NameChanged(string oldName, string newName) 
        => Nickname.text = $"|{newName}|";
}
