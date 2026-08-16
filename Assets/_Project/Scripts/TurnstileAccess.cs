using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public sealed class TurnstileAccess : MonoBehaviour
{
    [SerializeField] private GameState gameState;
    [SerializeField] private GameObject[] barriers;
    [SerializeField] private Renderer[] indicators;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip grantedClip;

    private MaterialPropertyBlock indicatorProperties;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        IsOpen = false;
        indicatorProperties = new MaterialPropertyBlock();
        foreach (GameObject barrier in barriers)
            barrier.SetActive(true);

        SetIndicator(new Color(0.85f, 0.15f, 0.08f));
    }

    private void OnTriggerEnter(Collider other)
    {
        TryOpen(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryOpen(other);
    }

    private void TryOpen(Collider other)
    {
        if (IsOpen || !gameState.HasKeycard || !gameState.HasBackpack ||
            other.GetComponentInParent<FirstPersonController>() == null)
            return;

        IsOpen = true;
        SetIndicator(new Color(0.12f, 0.85f, 0.3f));
        audioSource.PlayOneShot(grantedClip);
        foreach (GameObject barrier in barriers)
            barrier.SetActive(false);
    }

    private void SetIndicator(Color color)
    {
        foreach (Renderer indicator in indicators)
        {
            indicator.GetPropertyBlock(indicatorProperties);
            indicatorProperties.SetColor("_BaseColor", color);
            indicator.SetPropertyBlock(indicatorProperties);
        }
    }
}
