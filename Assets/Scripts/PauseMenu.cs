using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel; // panel s pauzovacím menu
    public TMP_Text pauseScoreText; // zobrazení skóre při pauze
    public Button resumeButton;
    public Button quitButton;

    private bool isPaused = false;

    [SerializeField] private AudioManager audioManager;

    void Start()
    {
        // skrytí panelu na začátku
        pausePanel.SetActive(false);

        // listenery na tlačítka
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);

       if (audioManager == null)
            {
                audioManager = FindObjectOfType<AudioManager>();
            }
        
    }

    void Update()
    {
        // pauznutí Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
            audioManager.PlaySFX(audioManager.buttonClickSound);
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // zastaví čas ve hře
        pausePanel.SetActive(true);

        // zobrazení aktuálního času
        if (GameManager.Instance != null)
        {
            float survivalTime = GameManager.Instance.GetSurvivalTime();
            pauseScoreText.text = $"Time: {survivalTime:F1} s";
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        // zvuk kliknutí
        audioManager.PlaySFX(audioManager.buttonClickSound);
    }

    public void QuitGame()
    {

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnButtonClick()
    {
        if (audioManager != null && audioManager.buttonClickSound != null)
        {
            audioManager.PlaySFX(audioManager.buttonClickSound);
        }
        else
        {
            Debug.LogError("AudioManager nebo buttonClickSound je null");
        }
    }
}