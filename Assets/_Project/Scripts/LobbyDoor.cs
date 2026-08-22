using UnityEngine;

[DisallowMultipleComponent]
public sealed class LobbyDoor : MonoBehaviour
{
    [SerializeField] private SafeRoomDoor safeRoomDoor;

    public bool IsOpen => safeRoomDoor.IsOpen;
}
