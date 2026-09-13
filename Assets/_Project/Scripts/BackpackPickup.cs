using UnityEngine;

[DisallowMultipleComponent]
public sealed class BackpackPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private GameState gameState;
    [SerializeField] private GameHUD hud;
    [SerializeField] private BackpackCabinet cabinet;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupClip;

    public string InteractionText => "Забрать рюкзак";
    public bool CanInteract => isActiveAndEnabled && gameState.PuzzleSolved &&
        cabinet.IsOpen && !gameState.HasBackpack;

    private void Start()
    {
        if (gameState.HasBackpack)
            gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (!CanInteract || !gameState.TryCollectBackpack())
            return;

        hud.ShowMessage("Рюкзак получен", Color.white);
        audioSource.PlayOneShot(pickupClip);
        gameObject.SetActive(false);
    }
}
