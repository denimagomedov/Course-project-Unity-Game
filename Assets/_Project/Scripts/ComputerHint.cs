using UnityEngine;

[DisallowMultipleComponent]
public sealed class ComputerHint : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject screenContent;

    public bool IsOn { get; private set; }
    public string InteractionText => "Включить компьютер";
    public bool CanInteract => isActiveAndEnabled && !IsOn;

    private void Awake()
    {
        IsOn = false;
        screenContent.SetActive(false);
    }

    public void Interact()
    {
        if (!CanInteract)
            return;

        IsOn = true;
        screenContent.SetActive(true);
    }
}
