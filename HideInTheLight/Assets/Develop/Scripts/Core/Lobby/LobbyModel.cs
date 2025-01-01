using Mirror;

public class LobbyModel : NetworkBehaviour
{
    [SyncVar] public string Name;
    [SyncVar] public int Id;
    [SyncVar] public PlayerRole Role;
}
