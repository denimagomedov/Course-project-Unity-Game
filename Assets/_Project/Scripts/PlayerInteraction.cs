using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FirstPersonController))]
[DisallowMultipleComponent]
public sealed class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField, Min(0.1f)] private float interactionDistance = 2f;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private TMP_Text prompt;

    private FirstPersonController controller;
    private InputAction interactAction;

    private void Awake()
    {
        controller = GetComponent<FirstPersonController>();

        if (playerCamera == null || inputActions == null || crosshair == null || prompt == null)
        {
            Debug.LogError("PlayerInteraction requires a camera, input actions, crosshair and prompt.", this);
            enabled = false;
            return;
        }

        interactAction = inputActions.FindAction("Player/Interact", true).Clone();
        interactAction.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
        HideUI();
    }

    private void OnEnable()
    {
        interactAction?.Enable();
    }

    private void OnDisable()
    {
        interactAction?.Disable();
        HideUI();
    }

    private void OnDestroy()
    {
        interactAction?.Dispose();
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused)
            HideUI();
    }

    private void LateUpdate()
    {
        if (!controller.AcceptsGameplayInput || playerCamera == null || !playerCamera.isActiveAndEnabled)
        {
            HideUI();
            return;
        }

        crosshair.SetActive(true);
        IInteractable target = FindTarget();

        if (interactAction.WasPressedThisFrame() && IsAvailable(target))
        {
            target.Interact();

            if (!isActiveAndEnabled || !controller.AcceptsGameplayInput)
            {
                HideUI();
                return;
            }

            target = FindTarget();
        }

        bool available = IsAvailable(target);
        prompt.text = available ? "[E] " + target.InteractionText : string.Empty;
        prompt.gameObject.SetActive(available);
    }

    private IInteractable FindTarget()
    {
        Ray ray = new Ray(playerCamera.transform.position,
            playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f)).direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, interactionDistance, ~0, QueryTriggerInteraction.Ignore);
        Collider nearest = null;
        float nearestDistance = float.PositiveInfinity;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform) || hit.distance >= nearestDistance)
                continue;

            nearest = hit.collider;
            nearestDistance = hit.distance;
        }

        return nearest != null ? nearest.GetComponentInParent<IInteractable>() : null;
    }

    private static bool IsAvailable(IInteractable target)
    {
        return target is MonoBehaviour behaviour && behaviour != null &&
            behaviour.isActiveAndEnabled && target.CanInteract;
    }

    private void HideUI()
    {
        if (crosshair != null)
            crosshair.SetActive(false);

        if (prompt != null)
        {
            prompt.text = string.Empty;
            prompt.gameObject.SetActive(false);
        }
    }
}
