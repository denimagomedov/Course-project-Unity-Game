using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class MatrixPuzzle : MonoBehaviour, IInteractable
{
    [SerializeField] private GameState gameState;
    [SerializeField] private FirstPersonController controller;
    [SerializeField] private Transform viewPoint;
    [SerializeField] private TMP_InputField[] fields;
    [SerializeField] private Button checkButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private TMP_Text feedback;
    [SerializeField] private CanvasGroup controls;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successClip;

    private readonly int[] expected = { 11, 5, 8, 7, 4, 1, 7, 1, 4 };

    public bool IsSolving { get; private set; }
    public string InteractionText => "Решить задачу";
    public bool CanInteract => isActiveAndEnabled && !IsSolving && !gameState.PuzzleSolved;

    private void Awake()
    {
        foreach (TMP_InputField field in fields)
            field.text = string.Empty;

        feedback.text = string.Empty;
        checkButton.onClick.AddListener(CheckAnswer);
        leaveButton.onClick.AddListener(Close);
        UpdateControls();
    }

    private void Update()
    {
        if (IsSolving && Application.isFocused && Keyboard.current?.escapeKey.wasPressedThisFrame == true)
            Close();
    }

    private void OnDisable()
    {
        Close();
    }

    public void Interact()
    {
        if (!CanInteract)
            return;

        IsSolving = true;
        controller.BeginPuzzleView(viewPoint.position, viewPoint.rotation);
        UpdateControls();
    }

    public void Close()
    {
        if (!IsSolving)
            return;

        IsSolving = false;
        foreach (TMP_InputField field in fields)
            field.DeactivateInputField();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        UpdateControls();
        controller.EndPuzzleView();
    }

    public void CheckAnswer()
    {
        if (!IsSolving || gameState.PuzzleSolved)
            return;

        for (int i = 0; i < expected.Length; i++)
        {
            if (!int.TryParse(fields[i].text, NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture, out int value) || value != expected[i])
            {
                feedback.text = "Решение неверно";
                feedback.color = new Color(1f, 0.65f, 0.5f);
                return;
            }
        }

        if (!gameState.TrySolvePuzzle())
            return;

        feedback.text = "Решение принято";
        feedback.color = new Color(0.55f, 1f, 0.65f);
        audioSource.PlayOneShot(successClip);
        EventSystem.current?.SetSelectedGameObject(null);
        UpdateControls();
    }

    private void UpdateControls()
    {
        controls.interactable = IsSolving;
        controls.blocksRaycasts = IsSolving;
        foreach (TMP_InputField field in fields)
            field.interactable = IsSolving && !gameState.PuzzleSolved;

        checkButton.interactable = IsSolving && !gameState.PuzzleSolved;
        leaveButton.interactable = IsSolving;
    }
}
