using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public sealed class GameScreenController : MonoBehaviour
{
    [SerializeField] private FirstPersonController player;
    [SerializeField] private MatrixPuzzle puzzle;
    [SerializeField] private GameOverController gameOver;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject intro;
    [SerializeField] private GameObject introBody;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject ending;
    [SerializeField] private Button endingMenuButton;
    [SerializeField] private CanvasGroup fade;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseMenuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private AudioSource victoryAudio;

    public bool IsPaused { get; private set; }
    public bool IsEnding { get; private set; }
    public bool IsTransitioning { get; private set; }

    private void Awake()
    {
        RestoreGlobals();
        if (intro != null) intro.SetActive(false);
        if (pause != null) pause.SetActive(false);
        if (ending != null) ending.SetActive(false);
        continueButton?.onClick.AddListener(ContinueIntro);
        resumeButton?.onClick.AddListener(Resume);
        pauseMenuButton?.onClick.AddListener(MainMenu);
        quitButton?.onClick.AddListener(Quit);
        endingMenuButton?.onClick.AddListener(MainMenu);
        fade.alpha = player != null ? 1f : 0f;
        fade.blocksRaycasts = player != null;
        if (player != null)
        {
            IsTransitioning = true;
            player.SetScreenLocked(true);
        }
    }

    private IEnumerator Start()
    {
        if (player == null) yield break;
        yield return Fade(0f);
        IsTransitioning = false;
        player.SetScreenLocked(false);
    }

    private void Update()
    {
        if (player == null || IsTransitioning || IsEnding || gameOver.IsDefeated || !Application.isFocused)
            return;
        if (Keyboard.current?.escapeKey.wasPressedThisFrame != true) return;
        if (puzzle.IsSolving)
        {
            puzzle.Close();
            return;
        }
        if (IsPaused) Resume();
        else OpenPause();
    }

    public void ShowIntro()
    {
        if (IsTransitioning || intro.activeSelf) return;
        menu.SetActive(false);
        intro.SetActive(true);
        introBody.SetActive(false);
        EventSystem.current?.SetSelectedGameObject(null);
        StartCoroutine(RevealIntro());
    }

    private IEnumerator RevealIntro()
    {
        yield return new WaitForSecondsRealtime(0.65f);
        introBody.SetActive(true);
    }

    public void ContinueIntro()
    {
        if (intro.activeSelf && introBody.activeSelf) LoadGame();
    }

    public void LoadGame() => BeginLoad("Main");
    public void MainMenu() => BeginLoad("MainMenu");

    private void BeginLoad(string scene)
    {
        if (IsTransitioning) return;
        IsTransitioning = true;
        player?.SetScreenLocked(true);
        RestoreGlobals();
        StartCoroutine(Load(scene));
    }

    private IEnumerator Load(string scene)
    {
        yield return Fade(1f);
        yield return SceneManager.LoadSceneAsync(scene);
    }

    public void OpenPause()
    {
        if (IsPaused || IsTransitioning || IsEnding || gameOver.IsDefeated || puzzle.IsSolving || player.IsControlLocked) return;
        IsPaused = true;
        player.SetScreenLocked(true);
        gameplayUI.SetActive(false);
        pause.SetActive(true);
        EventSystem.current?.SetSelectedGameObject(null);
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void Resume()
    {
        if (!IsPaused || IsTransitioning) return;
        RestoreGlobals();
        IsPaused = false;
        pause.SetActive(false);
        gameplayUI.SetActive(true);
        EventSystem.current?.SetSelectedGameObject(null);
        player.SetScreenLocked(false);
    }

    public void ShowEnding()
    {
        if (IsEnding || IsTransitioning || IsPaused || gameOver.IsDefeated) return;
        IsEnding = true;
        victoryAudio?.Play();
        player.SetScreenLocked(true);
        puzzle.Close();
        gameplayUI.SetActive(false);
        EventSystem.current?.SetSelectedGameObject(null);
        StartCoroutine(End());
    }

    private IEnumerator End()
    {
        IsTransitioning = true;
        yield return Fade(1f);
        ending.SetActive(true);
        var ambience = FindAnyObjectByType<RouteAmbience>();
        if (ambience != null) ambience.enabled = false;
        endingMenuButton.gameObject.SetActive(false);
        fade.alpha = 0f;
        fade.blocksRaycasts = false;
        yield return new WaitForSecondsRealtime(0.8f);
        endingMenuButton.gameObject.SetActive(true);
        IsTransitioning = false;
    }

    private IEnumerator Fade(float target)
    {
        fade.blocksRaycasts = true;
        float start = fade.alpha;
        for (float elapsed = 0f; elapsed < 0.65f; elapsed += Time.unscaledDeltaTime)
        {
            fade.alpha = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, elapsed / 0.65f));
            yield return null;
        }
        fade.alpha = target;
        fade.blocksRaycasts = target > 0f;
    }

    public static void RestoreGlobals()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Quit()
    {
        if (IsTransitioning) return;
        RestoreGlobals();
        if (Application.isEditor)
        {
            if (IsPaused) Resume();
            return;
        }
        Application.Quit();
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}
