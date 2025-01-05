using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Develop.Scripts.Entities.Player
{
    public class PlayerView : NetworkBehaviour
    {
        [Header("Player Components")]
        public Camera Camera;
        public AudioListener AudioListener;
        public CharacterController CharacterController;

        [Header("Menu settings")]
        public Image MenuPanel;


        public void ToggleMenu()
        {
            bool isMenuActive = MenuPanel.gameObject.activeInHierarchy;

            MenuPanel.gameObject.SetActive(!isMenuActive);
        }
    }
}