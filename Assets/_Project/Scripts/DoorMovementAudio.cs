using UnityEngine;

[DisallowMultipleComponent]
public sealed class DoorMovementAudio : MonoBehaviour
{
    [SerializeField] private Transform hinge;
    [SerializeField] private AudioSource movementSource;
    [SerializeField] private AudioSource latchSource;
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;
    [SerializeField] private bool playEndLatch;

    private Quaternion previousRotation;
    private bool travelling;
    private bool paused;
    private bool finishing;
    private float level;

    private void Start()
    {
        previousRotation = hinge.localRotation;
        level = movementSource.volume;
    }

    private void LateUpdate()
    {
        float previousAngle = Mathf.Abs(Mathf.DeltaAngle(0f, previousRotation.eulerAngles.y));
        float change = Quaternion.Angle(previousRotation, hinge.localRotation);
        previousRotation = hinge.localRotation;
        if (Time.deltaTime <= 0f || AudioListener.pause)
            return;
        float angle = Mathf.Abs(Mathf.DeltaAngle(0f, hinge.localEulerAngles.y));
        if (change > 0.01f)
        {
            if (!travelling)
            {
                movementSource.volume = level;
                movementSource.clip = angle > previousAngle ? openClip : closeClip;
                movementSource.Play();
                travelling = true;
                finishing = false;
            }
            else if (paused)
                movementSource.UnPause();
            paused = false;
        }
        else if (travelling && !paused)
        {
            movementSource.Pause();
            paused = true;
        }
        if (travelling && (angle < 0.01f || Mathf.Abs(angle - 90f) < 0.01f))
        {
            if (playEndLatch)
                latchSource.Play();
            finishing = true;
            travelling = false;
            paused = false;
        }
        if (finishing)
        {
            movementSource.volume = Mathf.MoveTowards(movementSource.volume, 0f, level * Time.deltaTime / 0.05f);
            if (movementSource.volume == 0f)
            {
                movementSource.Stop();
                finishing = false;
            }
        }
    }

    private void OnDisable()
    {
        if (movementSource != null)
            movementSource.Stop();
        if (latchSource != null)
            latchSource.Stop();
        travelling = false;
        paused = false;
        finishing = false;
    }
}
