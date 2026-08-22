using UnityEngine;

[DisallowMultipleComponent]
public sealed class SafeRoomDoor : MonoBehaviour
{
    public enum RoomKind { Classroom, Lobby }

    [SerializeField] private RoomKind roomKind;
    [SerializeField] private GameState gameState;
    [SerializeField] private ChaseEnemy enemy;
    [SerializeField] private CharacterController player;
    [SerializeField] private Transform hinge;
    [SerializeField] private BoxCollider leaf;
    [SerializeField, Min(0.1f)] private float movementDuration = 0.5f;
    [SerializeField, Min(0f)] private float clearance = 0.12f;

    private FirstPersonController controller;
    private bool entered;
    private bool closedAfterEntry;
    private float angle;

    public bool IsOpen => angle <= -90f;
    public bool IsClosed => angle >= 0f;

    private void Awake()
    {
        controller = player.GetComponent<FirstPersonController>();
        angle = roomKind == RoomKind.Classroom ? -90f : 0f;
        hinge.localRotation = Quaternion.Euler(0f, angle, 0f);
        leaf.gameObject.SetActive(true);
        leaf.enabled = true;
    }

    private void OnEnable()
    {
        enemy.SafetyReached += EnterRoom;
    }

    private void OnDisable()
    {
        enemy.SafetyReached -= EnterRoom;
    }

    private void EnterRoom()
    {
        entered = true;
    }

    private void Update()
    {
        if (controller.IsDefeated)
            return;

        bool open = roomKind == RoomKind.Classroom
            ? !entered || gameState.HasBackpack && (closedAfterEntry || !enemy.gameObject.activeSelf)
            : gameState.HasBackpack && !entered;
        float target = open ? -90f : 0f;

        if (angle != target)
        {
            Vector3 center = transform.InverseTransformPoint(player.transform.TransformPoint(player.center));
            float radius = player.radius + player.skinWidth + clearance;
            if (!open && center.x + radius >= 0f)
                return;

            float width = leaf.size.z * leaf.transform.localScale.z;
            float halfThickness = leaf.size.x * leaf.transform.localScale.x * 0.5f;
            if (center.x + radius >= -width - halfThickness && center.x - radius <= halfThickness &&
                center.z + radius >= -halfThickness && center.z - radius <= width + halfThickness)
                return;

            angle = Mathf.MoveTowards(angle, target, 90f * Time.deltaTime / movementDuration);
            hinge.localRotation = Quaternion.Euler(0f, angle, 0f);
        }

        if (entered && IsClosed && !closedAfterEntry)
        {
            closedAfterEntry = true;
            enemy.Stop();
        }
    }

    public Vector3 KeepEnemyOutside(Vector3 destination, float radius)
    {
        Vector3 local = transform.InverseTransformPoint(destination);
        local.x = Mathf.Max(local.x, radius + clearance);
        return transform.TransformPoint(local);
    }
}
