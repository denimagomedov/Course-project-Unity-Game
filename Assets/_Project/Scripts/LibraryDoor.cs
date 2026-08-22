using UnityEngine;

[DisallowMultipleComponent]
public sealed class LibraryDoor : MonoBehaviour
{
    [SerializeField] private GameObject doorLeaf;
    [SerializeField] private Transform hinge;
    [SerializeField] private CharacterController player;
    [SerializeField, Min(0.1f)] private float openingDuration = 0.5f;

    private BoxCollider leafCollider;
    private bool openingRequested;
    private float angle;

    public bool IsOpen => angle >= 90f;
    public bool IsOpening => openingRequested && !IsOpen;

    private void Awake()
    {
        angle = 0f;
        openingRequested = false;
        hinge.localRotation = Quaternion.identity;
        leafCollider = doorLeaf.GetComponent<BoxCollider>();
        doorLeaf.SetActive(true);
    }

    public void Open()
    {
        openingRequested = true;
    }

    private void Update()
    {
        if (!IsOpening)
            return;

        float nextAngle = Mathf.MoveTowards(angle, 90f, 90f * Time.deltaTime / openingDuration);
        for (float sample = angle; sample < nextAngle;)
        {
            sample = Mathf.Min(sample + 1f, nextAngle);
            Quaternion rotation = hinge.parent.rotation * Quaternion.Euler(0f, sample, 0f);
            Vector3 position = hinge.position + rotation * doorLeaf.transform.localPosition;
            if (Physics.ComputePenetration(leafCollider, position, rotation,
                    player, player.transform.position, player.transform.rotation, out _, out _))
                return;
        }

        angle = nextAngle;
        hinge.localRotation = Quaternion.Euler(0f, angle, 0f);
    }
}
