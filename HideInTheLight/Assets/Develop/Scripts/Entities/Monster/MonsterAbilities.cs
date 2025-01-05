using Mirror;
using UnityEngine;

public class MonsterAbilities : NetworkBehaviour
{
    [SerializeField] private MeshRenderer _materializedForm;
    [SerializeField] private GameObject _unMaterializedForm;
    [SyncVar] public bool Materialized = true;

    private MonsterPresenter _presetner;

    private void Start()
    {
        _presetner = GetComponent<MonsterPresenter>();
    }

    private void Update()
    {
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
    public void CmdMaterialize()
    {
        RpcMaterialize();
        _presetner.SetUnmanterializeState(false);
    }
    [Command]
    public void CmdUnMaterialize()
    {
        RpcUnMaterialize();
        _presetner.SetUnmanterializeState(true);
    }

    [ClientRpc]
    private void RpcMaterialize()
    {
        _unMaterializedForm.SetActive(false);

        _materializedForm.enabled = true;
        Materialized = true;

    }
    [ClientRpc]
    private void RpcUnMaterialize()
    {
        _unMaterializedForm.SetActive(true);

        _materializedForm.enabled = false;
        Materialized = false;
    }
}
