using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LobbyModel : NetworkBehaviour
{
    [SyncVar] public int Id;
    [SyncVar] public PlayerRole Role;
}
