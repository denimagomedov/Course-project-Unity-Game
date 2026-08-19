using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public sealed class FirstChaseTrigger : MonoBehaviour
{
    [SerializeField] private ChaseEnemy enemy;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CharacterController player;
    [SerializeField] private ClassroomEntrance classroomEntrance;
    [SerializeField] private GameState gameState;
    [SerializeField] private Vector3 crossingDirection = Vector3.back;

    private bool approaching;
    private bool classroomVisited;

    public bool HasTriggered { get; private set; }
    public bool HasCaughtPlayer { get; private set; }
    public event System.Action PlayerCaught;

    private void OnEnable()
    {
        classroomEntrance.PlayerEntered += EnterClassroom;
        enemy.PlayerCaught += HandlePlayerCaught;
    }

    private void OnDisable()
    {
        classroomEntrance.PlayerEntered -= EnterClassroom;
        enemy.PlayerCaught -= HandlePlayerCaught;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != player)
            return;

        approaching = Vector3.Dot(player.transform.position - transform.position,
            crossingDirection.normalized) < 0f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other != player || !approaching || HasTriggered || classroomVisited ||
            gameState.HasEnteredClassroom || !gameState.HasKeycard)
            return;

        if (Vector3.Dot(player.transform.position - transform.position,
                crossingDirection.normalized) < 0f)
            return;

        HasTriggered = true;
        enemy.Appear(player, classroomEntrance.GetComponent<BoxCollider>(), spawnPoint);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == player)
            approaching = false;
    }

    private void EnterClassroom()
    {
        classroomVisited = true;
        approaching = false;
        enemy.Stop();
    }

    private void HandlePlayerCaught()
    {
        if (HasCaughtPlayer || classroomVisited)
            return;

        HasCaughtPlayer = true;
        PlayerCaught?.Invoke();
    }
}
