using UnityEngine;

[DisallowMultipleComponent]
public sealed class GreyboxExitTrigger : MonoBehaviour
{
    [SerializeField] private GameState gameState;
    [SerializeField] private CharacterController player;
    [SerializeField] private GameScreenController screens;

    private bool reached;

    private void Awake()
    {
        reached = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (reached || other != player || !gameState.HasKeycard || !gameState.HasBackpack)
            return;

        reached = true;
        screens.ShowEnding();
    }
}
