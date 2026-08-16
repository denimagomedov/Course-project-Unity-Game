using UnityEngine;

[DisallowMultipleComponent]
public sealed class GreyboxExitTrigger : MonoBehaviour
{
    [SerializeField] private GameState gameState;
    [SerializeField] private CharacterController player;
    [SerializeField] private GameObject message;

    private bool reached;

    private void Awake()
    {
        reached = false;
        message.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (reached || other != player || !gameState.HasKeycard || !gameState.HasBackpack)
            return;

        reached = true;
        message.SetActive(true);
    }
}
