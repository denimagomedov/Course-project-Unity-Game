using UnityEngine;

[DisallowMultipleComponent]
public sealed class BackpackCabinet : MonoBehaviour
{
    [SerializeField] private GameState gameState;
    [SerializeField] private GameObject frontPanel;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        IsOpen = false;
        frontPanel.SetActive(true);
    }

    private void OnEnable()
    {
        gameState.PuzzleCompleted += Open;
        RefreshState();
    }

    private void Start()
    {
        RefreshState();
    }

    private void OnDisable()
    {
        gameState.PuzzleCompleted -= Open;
    }

    private void RefreshState()
    {
        if (gameState.PuzzleSolved)
            Open();
    }

    private void Open()
    {
        if (IsOpen)
            return;

        IsOpen = true;
        frontPanel.SetActive(false);
    }
}
