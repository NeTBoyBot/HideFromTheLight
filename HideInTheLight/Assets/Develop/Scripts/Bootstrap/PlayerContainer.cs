using System;
using Mirror;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


public class PlayerContainer : MonoBehaviour
{
    public readonly Dictionary<int, PlayerRole> PlayerRoles = new();
    [field:SerializeField] public List<PlayerInfo> PlayerRolesInfo = new();
}

[Serializable]
public class PlayerInfo
{
    public uint index;
    public PlayerRole role;
}
