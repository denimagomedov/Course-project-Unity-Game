using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public sealed class SecondChaseTrigger : MonoBehaviour
{
    [SerializeField] private ChaseEnemy enemy;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CharacterController player;
    [SerializeField] private LobbySafeZone safeZone;
    [SerializeField] private GameState gameState;
    [SerializeField] private GameOverController gameOver;
    [SerializeField] private Vector3 crossingDirection = Vector3.forward;

    private bool approaching;

    public bool HasTriggered { get; private set; }
    public bool HasCompleted { get; private set; }
    public bool HasCaughtPlayer { get; private set; }
    public event Action PlayerCaught;

    private void OnEnable()
    {
        safeZone.PlayerEntered += EnterLobby;
        enemy.PlayerCaught += HandlePlayerCaught;
    }

    private void OnDisable()
    {
        safeZone.PlayerEntered -= EnterLobby;
        enemy.PlayerCaught -= HandlePlayerCaught;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == player)
            approaching = Vector3.Dot(player.transform.position - transform.position,
                crossingDirection.normalized) < 0f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other != player || !approaching || HasTriggered || HasCompleted ||
            safeZone.HasEntered || !gameState.HasBackpack || gameOver.IsDefeated)
            return;

        if (Vector3.Dot(player.transform.position - transform.position,
                crossingDirection.normalized) < 0f)
            return;

        HasTriggered = true;
        enemy.Appear(player, safeZone.GetComponent<BoxCollider>(), spawnPoint);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == player)
            approaching = false;
    }

    private void EnterLobby()
    {
        HasCompleted = true;
        approaching = false;
        enemy.ReachSafety();
    }

    private void HandlePlayerCaught()
    {
        if (HasCompleted || HasCaughtPlayer || safeZone.HasEntered || gameOver.IsDefeated)
            return;

        HasCaughtPlayer = true;
        HasCompleted = true;
        PlayerCaught?.Invoke();
    }
}
