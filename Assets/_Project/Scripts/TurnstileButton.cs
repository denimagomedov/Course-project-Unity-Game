using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public sealed class TurnstileButton : MonoBehaviour, IInteractable
{
    [SerializeField] private TurnstileAccess access;

    public string InteractionText => "Открыть турникеты";
    public bool CanInteract => access != null && access.isActiveAndEnabled && access.CanOpen;

    public void Interact()
    {
        if (CanInteract)
            access.TryOpen();
    }
}
