using UnityEngine;

[DisallowMultipleComponent]
public sealed class LibraryDoor : MonoBehaviour
{
    [SerializeField] private GameObject doorLeaf;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        IsOpen = false;
        doorLeaf.SetActive(true);
    }

    public void Open()
    {
        if (IsOpen)
            return;

        IsOpen = true;
        doorLeaf.SetActive(false);
    }
}
