using Develop.Scripts.Core.Lobby;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterView : NetworkBehaviour
{
    [HideInInspector] public Camera Camera;
    [HideInInspector] public AudioListener AudioListener;
    [HideInInspector] public Animator Animator;

    public GameObject HUD;
    public Slider HPSlider;
    public TMP_Text HPSliderText;

    [Header("Menu settings")]
    public Image MenuPanel;

    public void Initialize()
    {
        Animator = GetComponentInChildren<Animator>();
        Camera = GetComponentInChildren<Camera>();
        AudioListener = GetComponentInChildren<AudioListener>();
    }

    [Command(requiresAuthority = false)]
    public void UpdateHealth(float currentHealth)
    {
        HPSlider.value = currentHealth / 100f;
        RpcUpdateHealth(currentHealth);
    }
    [ClientRpc]
    private void RpcUpdateHealth(float currentHealth)
    {
        HPSlider.value = currentHealth / 100f;
        HPSliderText.text = currentHealth.ToString();
    }

    public void ToggleMenu(PlayerIdentification identification)
    {
        bool isMenuActive = MenuPanel.gameObject.activeInHierarchy;

        MenuPanel.gameObject.SetActive(!isMenuActive);

        identification.ChangeCursorState(!isMenuActive, !isMenuActive ? CursorLockMode.Confined : CursorLockMode.Locked);
    }

    public void PlayAnimation(string animName) => Animator.Play(animName);
    public void PlayWalkAnimation(float moveSpeed) => Animator.SetFloat("MoveSpeed", moveSpeed);

}
