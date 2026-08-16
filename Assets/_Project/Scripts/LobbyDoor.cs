using UnityEngine;

[DisallowMultipleComponent]
public sealed class LobbyDoor : MonoBehaviour
{
    [SerializeField] private GameState gameState;
    [SerializeField] private GameObject doorLeaf;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        IsOpen = false;
        doorLeaf.SetActive(true);
    }

    private void OnEnable()
    {
        gameState.BackpackCollected += Open;
        RefreshState();
    }

    private void Start()
    {
        RefreshState();
    }

    private void OnDisable()
    {
        gameState.BackpackCollected -= Open;
    }

    private void RefreshState()
    {
        if (gameState.HasBackpack)
            Open();
    }

    private void Open()
    {
        if (IsOpen)
            return;

        IsOpen = true;
        doorLeaf.SetActive(false);
    }
}
