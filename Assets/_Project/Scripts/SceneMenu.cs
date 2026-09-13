using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class SceneMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup controls;
    [SerializeField] private Button playButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    public bool IsLoading { get; private set; }

    private void Awake()
    {
        if (playButton != null)
            playButton.onClick.AddListener(Play);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(MainMenu);
        if (quitButton != null)
            quitButton.onClick.AddListener(Quit);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnApplicationFocus(bool focused)
    {
        if (focused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Play()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
            FindAnyObjectByType<GameScreenController>().ShowIntro();
        else
            LoadScene("Main");
    }

    public void MainMenu()
    {
        LoadScene("MainMenu");
    }

    private void LoadScene(string scenePath)
    {
        if (IsLoading || !isActiveAndEnabled || !controls.interactable)
            return;

        IsLoading = true;
        controls.interactable = false;
        var screens = FindAnyObjectByType<GameScreenController>();
        if (scenePath == "Main") screens.LoadGame();
        else screens.MainMenu();
    }

    public void Quit()
    {
        if (IsLoading || !controls.interactable)
            return;

        FindAnyObjectByType<GameScreenController>().Quit();
    }
}
