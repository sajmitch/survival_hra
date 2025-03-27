using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;


public class MainMenu : MonoBehaviour
{
    public TMP_InputField nicknameField;
    public TMP_Text leaderboardText;
    public TMP_Text nicknameWarningText; // skrytý text pro varování
    public Button startGameButton;
    public Button quitButton;

    [Header("Parallax Settings")]
    public RectTransform backgroundPanel; // odkaz na Panel s pozadím
    public float parallaxStrength = 15f; // síla efektu
    private Vector2 startPos;
    private Vector2 canvasSize;

    [SerializeField] private MusicManager musicManager;

    void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
        LoadLeaderboard();

        // parallax efekt
        if (backgroundPanel != null)
        {
            startPos = backgroundPanel.anchoredPosition;
            canvasSize = new Vector2(Screen.width, Screen.height);
        }

        // skrytí varování na začátku
        if (nicknameWarningText != null)
        {
            nicknameWarningText.gameObject.SetActive(false);
        }

        // listener pro změnu textu v InputField
        nicknameField.onValueChanged.AddListener(delegate { HideWarning(); });
    }

    void Update()
    {
        // parallax efekt podle myši
        if (backgroundPanel != null)
        {
            Vector2 mousePos = new Vector2(Input.mousePosition.x / canvasSize.x * 2 - 1, Input.mousePosition.y / canvasSize.y * 2 - 1);
            Vector2 targetPos = startPos + mousePos * parallaxStrength;
            backgroundPanel.anchoredPosition = Vector2.Lerp(backgroundPanel.anchoredPosition, targetPos, Time.deltaTime * 5);
        }
    }

    public void StartGame()

    {
        string nickname = nicknameField.text.Trim();

        if (string.IsNullOrEmpty(nickname))
        {
            Debug.LogWarning("Player nemá nick");
            musicManager.PlaySFX(musicManager.buttonClickSound);

            // zobrazí varování v UI
            if (nicknameWarningText != null)
            {
                nicknameWarningText.text = "Please enter a nickname!";
                nicknameWarningText.gameObject.SetActive(true);
            }
            return;
        }
        musicManager.PlaySFX(musicManager.buttonClickSound);
        PlayerPrefs.SetString("PlayerNickname", nickname);
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        musicManager.PlaySFX(musicManager.buttonClickSound);
    }

    void LoadLeaderboard()
    {
        leaderboardText.text = "** LEADERBOARD TOP 10 **\n";
        leaderboardText.text += "--------------------------------\n";

        int maxEntries = 10; // maximální počet zobrazených záznamů
        List<string> leaderboardEntries = new List<string>();

        for (int i = 0; i < maxEntries; i++)
        {
            string nameKey = "LeaderboardName_" + i;
            string scoreKey = "LeaderboardScore_" + i;

            if (PlayerPrefs.HasKey(scoreKey))
            {
                string playerName = PlayerPrefs.GetString(nameKey, "Unknown");
                float score = PlayerPrefs.GetFloat(scoreKey);

                // zkrácení jména na max 10 znaků
                if (playerName.Length > 10)
                    playerName = playerName.Substring(0, 10) + ".";

                leaderboardEntries.Add($"{(i + 1),-3} {playerName,-10} {score:F1} s");
            }
        }

        leaderboardText.text += string.Join("\n", leaderboardEntries);
    }

    void HideWarning()
    {
        // jakmile hráč začne psát, varování se skryje
        if (nicknameWarningText != null)
        {
            nicknameWarningText.gameObject.SetActive(false);
        }
    }
}