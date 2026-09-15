using UnityEngine;

[DisallowMultipleComponent]
public sealed class RouteAmbience : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameOverController gameOver;
    [SerializeField] private ChaseEnemy firstEnemy;
    [SerializeField] private ChaseEnemy secondEnemy;
    [SerializeField, Range(0f, 1f)] private float baseLevel = 0.14f;
    [SerializeField, Range(0f, 1f)] private float chaseLevel = 0.24f;
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.8f;

    private void OnEnable()
    {
        audioSource.volume = 0f;
        audioSource.Play();
    }

    private void Update()
    {
        if (AudioListener.pause || Time.deltaTime <= 0f)
            return;

        bool defeated = gameOver.IsDefeated;
        bool chasing = firstEnemy.State == ChaseEnemy.ChaseState.Chase ||
            secondEnemy.State == ChaseEnemy.ChaseState.Chase;
        float target = defeated ? 0f : chasing ? chaseLevel : baseLevel;
        if (target > 0f && !audioSource.isPlaying)
            audioSource.Play();
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, target,
            Mathf.Max(baseLevel, chaseLevel) * Time.deltaTime / (defeated ? 0.2f : fadeDuration));
        if (target == 0f && audioSource.volume == 0f && audioSource.isPlaying)
            audioSource.Stop();
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }
}
