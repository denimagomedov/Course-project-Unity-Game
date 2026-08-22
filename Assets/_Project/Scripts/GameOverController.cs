using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class GameOverController : MonoBehaviour
{
    [SerializeField] private FirstChaseTrigger firstChase;
    [SerializeField] private SecondChaseTrigger secondChase;
    [SerializeField] private FirstPersonController player;
    [SerializeField] private PlayerInteraction interaction;
    [SerializeField] private MatrixPuzzle puzzle;
    [SerializeField] private GameObject[] gameplayUI;
    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private GameObject menuContent;
    [SerializeField, Min(0f)] private float fadeDuration = 0.4f;

    public bool IsDefeated { get; private set; }

    private void Awake()
    {
        overlay.alpha = 0f;
        overlay.interactable = false;
        overlay.blocksRaycasts = true;
        menuContent.SetActive(false);
        overlay.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        firstChase.PlayerCaught += Defeat;
        if (secondChase != null)
            secondChase.PlayerCaught += Defeat;
    }

    private void OnDisable()
    {
        firstChase.PlayerCaught -= Defeat;
        if (secondChase != null)
            secondChase.PlayerCaught -= Defeat;
    }

    public void Defeat()
    {
        if (IsDefeated)
            return;

        IsDefeated = true;
        player.LockForDefeat();
        interaction.enabled = false;
        puzzle.Close();
        puzzle.enabled = false;
        foreach (GameObject element in gameplayUI)
            element.SetActive(false);

        EventSystem.current?.SetSelectedGameObject(null);
        overlay.gameObject.SetActive(true);
        StartCoroutine(FadeToMenu());
    }

    private IEnumerator FadeToMenu()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        overlay.alpha = 1f;
        menuContent.SetActive(true);
        overlay.interactable = true;
    }
}
