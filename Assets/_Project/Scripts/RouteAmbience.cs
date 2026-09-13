using UnityEngine;

[DisallowMultipleComponent]
public sealed class RouteAmbience : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameState gameState;
    [SerializeField] private GameOverController gameOver;
    [SerializeField] private ChaseEnemy firstEnemy;
    [SerializeField] private ChaseEnemy secondEnemy;
    [SerializeField, Range(0f, 1f)] private float level = 0.14f;
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.8f;

    private int phase;

    private void OnEnable()
    {
        phase = 0;
        audioSource.volume = 0f;
        audioSource.Play();
    }

    private void Update()
    {
        if (AudioListener.pause || Time.deltaTime <= 0f)
            return;

        if (secondEnemy.HasAppeared)
            phase = 3;
        else if (firstEnemy.HasAppeared && phase < 2)
            phase = 2;
        else if (gameState.HasLeftLibrary && phase == 0)
            phase = 1;

        bool defeated = gameOver.IsDefeated;
        float target = !defeated && (phase == 0 || phase == 2) ? level : 0f;
        if (target > 0f && !audioSource.isPlaying)
            audioSource.Play();
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, target,
            level * Time.deltaTime / (defeated ? 0.2f : fadeDuration));
        if (target == 0f && audioSource.volume == 0f && audioSource.isPlaying)
            audioSource.Stop();
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }
}
