using System;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider))]
public sealed class ChaseEnemy : MonoBehaviour
{
    public enum ChaseState { Inactive, Appear, Chase, PlayerCaught, Stop }

    [SerializeField, Min(0f)] private float movementDelay = 0.75f;
    [SerializeField, Min(0.1f)] private float speed = 4.8f;
    [SerializeField, Min(0.1f)] private float acceleration = 16f;
    [SerializeField, Min(1f)] private float angularSpeed = 540f;
    [SerializeField, Min(0.1f)] private float contactRadius = 0.35f;
    [SerializeField, Min(0.2f)] private float contactHeight = 1.8f;
    [SerializeField] private LayerMask contactObstacles = ~0;

    private NavMeshAgent agent;
    private CapsuleCollider contact;
    private CharacterController player;
    private BoxCollider safeArea;
    private float chaseStartsAt;

    public ChaseState State { get; private set; }
    public event Action PlayerCaught;

    public void Appear(CharacterController target, BoxCollider safety, Transform spawn)
    {
        if (State != ChaseState.Inactive)
            return;

        agent = GetComponent<NavMeshAgent>();
        contact = GetComponent<CapsuleCollider>();
        player = target;
        safeArea = safety;
        if (PlayerIsSafe())
        {
            Stop();
            return;
        }

        if (!NavMesh.SamplePosition(spawn.position, out NavMeshHit hit, 0.5f, agent.areaMask))
        {
            Debug.LogError("Chase spawn is outside the baked NavMesh.", this);
            Stop();
            return;
        }

        transform.SetPositionAndRotation(hit.position, spawn.rotation);
        contact.radius = contactRadius;
        contact.height = Mathf.Max(contactHeight, contactRadius * 2f);
        contact.center = Vector3.up * (contact.height * 0.5f);
        contact.isTrigger = true;
        contact.enabled = false;
        agent.enabled = false;
        gameObject.SetActive(true);
        State = ChaseState.Appear;
        chaseStartsAt = Time.time + movementDelay;
    }

    private void Update()
    {
        if (State != ChaseState.Appear && State != ChaseState.Chase)
            return;

        if (PlayerIsSafe())
        {
            Stop();
            return;
        }

        if (State == ChaseState.Appear)
        {
            if (Time.time < chaseStartsAt)
                return;

            agent.speed = speed;
            agent.acceleration = acceleration;
            agent.angularSpeed = angularSpeed;
            agent.enabled = true;
            contact.enabled = true;
            State = ChaseState.Chase;
        }

        if (agent.isOnNavMesh)
            agent.SetDestination(player.transform.position);
    }

    private void LateUpdate()
    {
        if (State != ChaseState.Chase)
            return;

        if (PlayerIsSafe())
        {
            Stop();
            return;
        }

        if (!Physics.ComputePenetration(contact, transform.position, transform.rotation,
                player, player.transform.position, player.transform.rotation, out _, out _))
            return;

        Vector3 origin = transform.TransformPoint(contact.center);
        Vector3 destination = player.transform.TransformPoint(player.center);
        Vector3 separation = destination - origin;
        foreach (RaycastHit hit in Physics.RaycastAll(origin, separation.normalized,
                     separation.magnitude, contactObstacles, QueryTriggerInteraction.Ignore))
        {
            if (!hit.transform.IsChildOf(player.transform) && !hit.transform.IsChildOf(transform))
                return;
        }

        State = ChaseState.PlayerCaught;
        DisableEnemy();
        PlayerCaught?.Invoke();
    }

    private bool PlayerIsSafe()
    {
        return player != null && safeArea != null &&
            Physics.ComputePenetration(safeArea, safeArea.transform.position, safeArea.transform.rotation,
                player, player.transform.position, player.transform.rotation, out _, out _);
    }

    public void Stop()
    {
        if (State != ChaseState.PlayerCaught)
            State = ChaseState.Stop;
        DisableEnemy();
    }

    private void DisableEnemy()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        if (contact == null)
            contact = GetComponent<CapsuleCollider>();
        agent.enabled = false;
        contact.enabled = false;
        gameObject.SetActive(false);
    }
}
