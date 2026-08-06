using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameHUD : MonoBehaviour
{
    [SerializeField] private GameState gameState;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMP_Text messageText;

    private float messageExpiresAt;

    private void Awake()
    {
        ClearMessage();
    }

    private void Start()
    {
        objectiveText.text = gameState.CurrentObjective;
    }

    private void LateUpdate()
    {
        if (objectiveText.text != gameState.CurrentObjective)
            objectiveText.text = gameState.CurrentObjective;

        if (messagePanel.activeSelf && Time.time >= messageExpiresAt)
            ClearMessage();
    }

    public void ShowMessage(string text, Color color, float duration = 2f)
    {
        messageText.text = text;
        messageText.color = color;
        messageExpiresAt = Time.time + duration;
        messagePanel.SetActive(true);
    }

    private void ClearMessage()
    {
        messageText.text = string.Empty;
        messagePanel.SetActive(false);
    }
}
