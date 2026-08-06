using UnityEngine;

[DisallowMultipleComponent]
public sealed class KeycardPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private GameState gameState;
    [SerializeField] private GameHUD hud;

    public string InteractionText => "Взять ключ-карту";
    public bool CanInteract => isActiveAndEnabled && gameState != null && !gameState.HasKeycard;

    public void Interact()
    {
        if (!CanInteract || !gameState.TryCollectKeycard())
            return;

        hud.ShowMessage("Ключ-карта получена", Color.white);
        gameObject.SetActive(false);
    }
}
