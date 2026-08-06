using UnityEngine;

[DisallowMultipleComponent]
public sealed class KeycardReader : MonoBehaviour, IInteractable
{
    [SerializeField] private GameState gameState;
    [SerializeField] private LibraryDoor door;
    [SerializeField] private GameHUD hud;
    [SerializeField] private Renderer indicator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deniedClip;
    [SerializeField] private AudioClip grantedClip;
    [SerializeField, Min(0.1f)] private float accessDelay = 0.45f;
    [SerializeField, Min(0.1f)] private float denialCooldown = 0.3f;

    private MaterialPropertyBlock indicatorProperties;
    private float completesAt;
    private bool pendingGrant;

    public bool IsProcessing { get; private set; }
    public string InteractionText => gameState != null && gameState.HasKeycard
        ? "Использовать карту" : "Проверить доступ";
    public bool CanInteract => isActiveAndEnabled && gameState != null && door != null &&
        !IsProcessing && !door.IsOpen;

    private void Awake()
    {
        indicatorProperties = new MaterialPropertyBlock();
        IsProcessing = false;
        pendingGrant = false;
        SetIndicator(new Color(0.8f, 0.55f, 0.12f));
    }

    public void Interact()
    {
        if (!CanInteract)
            return;

        IsProcessing = true;
        pendingGrant = gameState.HasKeycard;
        completesAt = Time.time + (pendingGrant ? accessDelay : denialCooldown);

        if (!pendingGrant)
            ShowAccessResult(false);
    }

    private void Update()
    {
        if (!IsProcessing || Time.time < completesAt)
            return;

        IsProcessing = false;

        if (!pendingGrant)
            return;

        pendingGrant = false;
        ShowAccessResult(true);
        door.Open();
    }

    private void OnDisable()
    {
        IsProcessing = false;
        pendingGrant = false;
    }

    private void ShowAccessResult(bool granted)
    {
        SetIndicator(granted ? new Color(0.12f, 0.85f, 0.3f) : new Color(0.95f, 0.12f, 0.08f));
        hud.ShowMessage(granted ? "ACCESS GRANTED" : "ACCESS DENIED",
            granted ? new Color(0.45f, 1f, 0.55f) : new Color(1f, 0.5f, 0.45f));
        audioSource.Stop();
        audioSource.PlayOneShot(granted ? grantedClip : deniedClip);
    }

    private void SetIndicator(Color color)
    {
        indicator.GetPropertyBlock(indicatorProperties);
        indicatorProperties.SetColor("_BaseColor", color);
        indicator.SetPropertyBlock(indicatorProperties);
    }
}
