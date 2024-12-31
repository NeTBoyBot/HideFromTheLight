using DI;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Develop.Scripts.Core.Lobby
{
    public class LobbyView : NetworkBehaviour
    {
        [SerializeField] public Button HumanButton, MonsterButton;
        [SerializeField] public TMP_Text Chat;
    }
}

