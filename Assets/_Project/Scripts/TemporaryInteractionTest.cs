using UnityEngine;

[DisallowMultipleComponent]
public sealed class TemporaryInteractionTest : MonoBehaviour, IInteractable
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color firstColor = new Color(0.12f, 0.55f, 0.85f);
    [SerializeField] private Color secondColor = new Color(1f, 0.55f, 0.12f);

    private MaterialPropertyBlock properties;
    private bool secondState;

    public string InteractionText => "Проверить взаимодействие";
    public bool CanInteract => isActiveAndEnabled && targetRenderer != null &&
        targetRenderer.enabled && targetRenderer.gameObject.activeInHierarchy;

    private void Awake()
    {
        properties = new MaterialPropertyBlock();
        ApplyColor();
    }

    public void Interact()
    {
        if (!CanInteract)
            return;

        secondState = !secondState;
        ApplyColor();
    }

    private void ApplyColor()
    {
        if (targetRenderer == null)
            return;

        targetRenderer.GetPropertyBlock(properties);
        properties.SetColor("_BaseColor", secondState ? secondColor : firstColor);
        targetRenderer.SetPropertyBlock(properties);
    }
}
