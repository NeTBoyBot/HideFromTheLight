using Mirror;
using UnityEngine;

public class MonsterAbilities : NetworkBehaviour
{
    [SerializeField] private GameObject _materializedForm;
    [SerializeField] private GameObject _unMaterializedForm;
    [SyncVar] public bool Materialized = true;

    private MonsterPresenter _presetner;

    private void Start()
    {
        _presetner = GetComponent<MonsterPresenter>();
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeMaterializationState(); 
        }
    }

    public void ChangeMaterializationState()
    {
        if (Materialized)
        {
            CmdUnMaterialize();
        }
        else
        {
            CmdMaterialize();
        }
    }

    [Command]
    public async void CmdMaterialize()
    {
        bool canMaterialized = await _presetner.ToggleMaterializationForm(false);
        if (!canMaterialized) 
            return;

        RpcMaterialize();
    }
    [Command]
    public async void CmdUnMaterialize()
    {
        bool canUnMaterialized = await _presetner.ToggleMaterializationForm(true);
        if (!canUnMaterialized)
            return;

        RpcUnMaterialize();
    }

    [ClientRpc]
    private void RpcMaterialize()
    {
        _unMaterializedForm.SetActive(false);

        _materializedForm.SetActive(true);
        Materialized = true;

    }
    [ClientRpc]
    private void RpcUnMaterialize()
    {
        _unMaterializedForm.SetActive(true);

        _materializedForm.SetActive(false);
        Materialized = false;
    }
}
