using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(FirstPersonController), typeof(CharacterController))]
public sealed class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;
    [SerializeField, Min(0.1f)] private float walkStepDistance = 1.45f;
    [SerializeField, Min(0.1f)] private float runStepDistance = 1.75f;

    private FirstPersonController controller;
    private CharacterController character;
    private Vector3 previousPosition;
    private float travelled;
    private int previousClip = -1;
    private float baseVolume;

    private void Awake()
    {
        controller = GetComponent<FirstPersonController>();
        character = GetComponent<CharacterController>();
        baseVolume = audioSource.volume;
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
        float distance = displacement.magnitude;
        if (!controller.AcceptsGameplayInput || controller.IsDefeated || !character.isGrounded ||
            Time.deltaTime <= 0f || AudioListener.pause || distance < 0.2f * Time.deltaTime ||
            distance > 8f * Time.deltaTime)
        {
            travelled = 0f;
            if (!controller.AcceptsGameplayInput || controller.IsDefeated)
                audioSource.Stop();
            return;
        }

        bool running = distance / Time.deltaTime > 4f;
        travelled += distance;
        float stride = running ? runStepDistance : walkStepDistance;
        if (travelled < stride || clips.Length == 0)
            return;

        travelled %= stride;
        int index = Random.Range(0, clips.Length);
        if (clips.Length > 1 && index == previousClip)
            index = (index + 1) % clips.Length;
        previousClip = index;
        audioSource.pitch = Random.Range(0.96f, 1.04f);
        audioSource.volume = baseVolume * (running ? 1f : 0.72f);
        audioSource.clip = clips[index];
        audioSource.Play();
    }

    private void OnDisable()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}
