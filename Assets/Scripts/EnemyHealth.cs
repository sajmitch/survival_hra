using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    private int maxHP = 1;
    private int currentHP;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isDead = false;

    private AudioManager audioManager;

    [Header("UI Health Text")]
    public Text healthText;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioManager = FindObjectOfType<AudioManager>(); // najde AudioManager ve scéně

        if (maxHP < 1) maxHP = 1; // zajištění minimálního HP = 1
        currentHP = maxHP;
        UpdateHealthText();
    }

    public void SetMaxHP(int value)
    {
        maxHP = Mathf.Max(1, value);
        currentHP = maxHP;
        UpdateHealthText();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        animator.SetTrigger("Hit");
        UpdateHealthText();

        // zvuk zásahu nepřítele
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.enemyHitSound);
        }

        if (currentHP <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        if (isDead) yield break;

        isDead = true;
        animator.SetBool("IsDead", true);
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        if (healthText != null)
        {
            healthText.gameObject.SetActive(false);
        }

        // přehrání zvuku smrti ještě před čekáním
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.enemyDeathSound);
        }

        // počká na dokončení animace smrti a pak zničí objekt
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = currentHP.ToString();
            healthText.color = currentHP == 1 ? Color.red : Color.white;
            healthText.gameObject.SetActive(true);
        }
    }
}