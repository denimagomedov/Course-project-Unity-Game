using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public sealed class LobbySafeZone : MonoBehaviour
{
    [SerializeField] private CharacterController player;

    private BoxCollider area;

    public bool HasEntered { get; private set; }
    public event Action PlayerEntered;

    private void Awake()
    {
        area = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (!HasEntered && Physics.ComputePenetration(area, transform.position, transform.rotation,
                player, player.transform.position, player.transform.rotation, out _, out _))
            Enter();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == player)
            Enter();
    }

    private void Enter()
    {
        if (HasEntered)
            return;

        HasEntered = true;
        PlayerEntered?.Invoke();
    }
}
