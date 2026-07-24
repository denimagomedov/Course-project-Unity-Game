using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[DisallowMultipleComponent]
public sealed class FirstPersonController : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField, Min(0.1f)] private float walkSpeed = 3f;
    [SerializeField, Min(0.1f)] private float sprintSpeed = 5.5f;
    [SerializeField, Min(0.001f)] private float mouseSensitivity = 0.15f;
    [SerializeField, Range(1f, 89f)] private float pitchLimit = 85f;
    [SerializeField, Min(0.1f)] private float gravity = 20f;

    private CharacterController characterController;
    private InputActionMap playerActions;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;
    private float pitch;
    private float verticalVelocity;
    private bool cursorCaptured;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (playerCamera == null || inputActions == null)
        {
            Debug.LogError("FirstPersonController requires a camera and an input actions asset.", this);
            enabled = false;
            return;
        }

        playerActions = inputActions.FindActionMap("Player", true).Clone();
        playerActions.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
        moveAction = playerActions.FindAction("Move", true);
        lookAction = playerActions.FindAction("Look", true);
        sprintAction = playerActions.FindAction("Sprint", true);
        pitch = Mathf.DeltaAngle(0f, playerCamera.localEulerAngles.x);
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        sprintAction.Enable();
        SetCursorCaptured(true);
    }

    private void OnDisable()
    {
        playerActions?.Disable();
        verticalVelocity = 0f;
        SetCursorCaptured(false);
    }

    private void OnDestroy()
    {
        playerActions?.Dispose();
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused)
            SetCursorCaptured(false);
    }

    private void Update()
    {
        bool acceptsInput = UpdateCursorCapture();
        Vector3 horizontalVelocity = Vector3.zero;

        if (acceptsInput)
        {
            Vector2 look = lookAction.ReadValue<Vector2>() * mouseSensitivity;
            transform.Rotate(0f, look.x, 0f, Space.World);
            pitch = Mathf.Clamp(pitch - look.y, -pitchLimit, pitchLimit);
            playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);

            Vector2 movement = Vector2.ClampMagnitude(moveAction.ReadValue<Vector2>(), 1f);
            float speed = sprintAction.IsPressed() ? sprintSpeed : walkSpeed;
            horizontalVelocity = (transform.right * movement.x + transform.forward * movement.y) * speed;
        }

        if (characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity = Mathf.Max(verticalVelocity - gravity * Time.deltaTime, -50f);
        CollisionFlags collisions = characterController.Move(
            (horizontalVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);

        if ((collisions & CollisionFlags.Below) != 0)
            verticalVelocity = -2f;
    }

    private bool UpdateCursorCapture()
    {
        if (!Application.isFocused || Keyboard.current?.escapeKey.wasPressedThisFrame == true)
        {
            SetCursorCaptured(false);
            return false;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
            cursorCaptured = false;

        if (!cursorCaptured)
        {
            if (Mouse.current?.leftButton.wasPressedThisFrame == true)
                SetCursorCaptured(true);

            return false;
        }

        return true;
    }

    private void SetCursorCaptured(bool captured)
    {
        cursorCaptured = captured;
        Cursor.lockState = captured ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !captured;
    }
}
