using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Slider healthSlider; // odkaz na UI Slider
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;

        if (healthSlider == null)
        {
            healthSlider = GameObject.Find("PlayerHealthBar")?.GetComponent<Slider>(); // automaticky hledá Slider
        }

        if (healthSlider == null)
        {
            return;
        }

        healthSlider.maxValue = gameManager.playerMaxHP;
        healthSlider.value = gameManager.playerMaxHP; // HP na max při startu
    }

    public void UpdateHealthUI(int currentHP)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHP;
        }
    }
}