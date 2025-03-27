using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType { HP, MaxHP, KillAll }
    public CollectibleType type; // typ collectable
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) //pokud hráč sebere item
        {
            if (audioManager != null)
            {
                audioManager.PlaySFX(audioManager.collectSound); // přehrání zvuku ještě před zničením
            }
            else
            {
                Debug.LogWarning("AudioManager není");
            }

            switch (type)
            {
                case CollectibleType.HP:
                    GameManager.Instance.Heal(1); // přidá 1 HP hráči
                    Debug.Log("Hráč získal +1 HP");
                    CollectibleSpawner.Instance.RemoveHPItem(gameObject);
                    break;

                case CollectibleType.MaxHP:
                    GameManager.Instance.Heal(GameManager.Instance.playerMaxHP); // nastaví HP na maximum
                    Debug.Log("Hráč získal Max HP Potion!");
                    CollectibleSpawner.Instance.PotionCollected("HealingPotion");
                    Destroy(gameObject);
                    break;

                case CollectibleType.KillAll:
                    GameManager.Instance.EnableKillAllButton(); // aktivuje tlačítko pro zabití nepřátel                   
                    Debug.Log("Kill-All Potion sebran! Tlačítko aktivováno.");
                    CollectibleSpawner.Instance.PotionCollected("KillAllPotion");
                    Destroy(gameObject);
                    break;
            }
        }
    }
}