using UnityEngine;

[DefaultExecutionOrder(100)]
[DisallowMultipleComponent]
[RequireComponent(typeof(ChaseEnemy))]
public sealed class EnemyFootsteps : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;
    [SerializeField, Min(0.1f)] private float stepDistance = 1.5f;

    private ChaseEnemy enemy;
    private Vector3 previousPosition;
    private float travelled;
    private int previousClip = -1;

    private void Awake()
    {
        enemy = GetComponent<ChaseEnemy>();
    }

    private void OnEnable()
    {
        previousPosition = transform.position;
        travelled = 0f;
    }

    private void LateUpdate()
    {
        Vector3 displacement = transform.position - previousPosition;
        previousPosition = transform.position;
        displacement.y = 0f;
        if (enemy.State != ChaseEnemy.ChaseState.Chase)
        {
            travelled = 0f;
            audioSource.Stop();
            return;
        }
        if (AudioListener.pause || Time.deltaTime <= 0f)
            return;
        float distance = displacement.magnitude;
        if (distance < 0.2f * Time.deltaTime || distance > 8f * Time.deltaTime)
        {
            travelled = 0f;
            return;
        }
        travelled += distance;
        if (travelled < stepDistance || clips.Length == 0)
            return;
        travelled %= stepDistance;
        int index = Random.Range(0, clips.Length);
        if (clips.Length > 1 && index == previousClip)
            index = (index + 1) % clips.Length;
        previousClip = index;
        audioSource.pitch = Random.Range(0.86f, 0.94f);
        audioSource.clip = clips[index];
        audioSource.Play();
    }

    private void OnDisable()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}
