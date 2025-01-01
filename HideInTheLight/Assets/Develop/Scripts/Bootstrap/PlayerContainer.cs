using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerContainer : MonoBehaviour
{
    public readonly Dictionary<int, PlayerRole> PlayerRoles = new();
    [field:SerializeField] public List<PlayerInfo> PlayerRolesInfo = new();
}

[Serializable]
public class PlayerInfo
{
    public int index;
    public PlayerRole role;
}
