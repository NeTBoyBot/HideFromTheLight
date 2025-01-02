using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MonsterView : NetworkBehaviour
{
    [Header("Player Components")]
    public Camera Camera;
    public AudioListener AudioListener;
    public CharacterController CharacterController;
}
