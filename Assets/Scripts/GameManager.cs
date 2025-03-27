using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    public int playerMaxHP = 5;
    private int playerCurrentHP;
    public bool isPlayerAttacking = false;

    [Header("Score System")]
    private float survivalTime = 0f;
    private bool isPlayerAlive = true;
    public TMP_Text scoreText;

    [Header("UI Elements")]
    public Slider playerHealthBar;
    public Button killAllButton;

    private AudioManager audioManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerCurrentHP = playerMaxHP;

        audioManager = FindObjectOfType<AudioManager>();

        // nastavení HP baru
        if (playerHealthBar != null)
        {
            playerHealthBar.minValue = 0;
            playerHealthBar.maxValue = playerMaxHP;
            playerHealthBar.value = playerCurrentHP;
        }
        else
        {
            Debug.LogWarning("PlayerHealthBar nefunuguje");
        }

        // tlačítko Kill All
        if (killAllButton != null)
        {
            killAllButton.gameObject.SetActive(false);
            killAllButton.onClick.RemoveAllListeners();
            killAllButton.onClick.AddListener(KillAllEnemies);
        }
        else
        {
            Debug.LogWarning("KillAllButton nefunguje");
        }
    }

    void Update()
    {
        if (isPlayerAlive)
        {
            survivalTime += Time.deltaTime;
            if (scoreText != null)
            {
                scoreText.text = survivalTime.ToString("F1") + " s"; // zobrazení skóre ve formátu "xx.y s"
            }
        }
    }

    public float GetSurvivalTime()
    {
        return survivalTime;
    }

    public void UpdateHealthUI()
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.value = playerCurrentHP;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isPlayerAttacking) return;

        playerCurrentHP -= damage;
        playerCurrentHP = Mathf.Max(playerCurrentHP, 0);

        UpdateHealthUI();
        PlayerMovement.Instance?.FlashRed();

        // zvuk zásahu hráče
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.hitSound);
        }
        else
        {
            Debug.LogWarning("AudioManager není");
        }

        if (playerCurrentHP == 0)
        {
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        isPlayerAlive = false;
        PlayerMovement.Instance?.TriggerDeathAnimation();

        // zvuk smrti hráče
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.deathSound);
        }

        float deathAnimationLength = PlayerMovement.Instance?.GetDeathAnimationLength() ?? 1.5f;
        StartCoroutine(DelayedGameOver(deathAnimationLength));
    }

    private IEnumerator DelayedGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayerPrefs.SetFloat("LastScore", survivalTime);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameOverScene");
    }

    public void Heal(int amount)
    {
        playerCurrentHP = Mathf.Min(playerCurrentHP + amount, playerMaxHP);
        UpdateHealthUI();
    }

    public void EnableKillAllButton()
    {
        if (killAllButton != null)
        {
            killAllButton.gameObject.SetActive(true);
            killAllButton.onClick.RemoveAllListeners();
            killAllButton.onClick.AddListener(KillAllEnemies);

            // zvuk sebrání collectable
            if (audioManager != null)
            {
                audioManager.PlaySFX(audioManager.collectSound);
            }
            else
            {
                Debug.LogWarning("AudioManager není");
            }
        }
    }

    public void KillAllEnemies()
    {
        // zvuk click tlačítka
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.collectSound);
        }
        else
        {
            Debug.LogWarning("AudioManager není");
        }

        // zničí všechny nepřátele ve scéně
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }

        if (killAllButton != null)
        {
            killAllButton.gameObject.SetActive(false);
        }
    }
}