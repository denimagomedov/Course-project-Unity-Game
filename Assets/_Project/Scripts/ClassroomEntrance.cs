using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public sealed class ClassroomEntrance : MonoBehaviour
{
    [SerializeField] private GameState gameState;

    public event System.Action PlayerEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<FirstPersonController>() == null)
            return;

        gameState.TryEnterClassroom();
        PlayerEntered?.Invoke();
    }
}
