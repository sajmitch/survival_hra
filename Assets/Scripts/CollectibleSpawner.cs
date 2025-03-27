using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    public static CollectibleSpawner Instance { get; private set; }

    [Header("Collectibles Prefabs")]
    public GameObject hpItemPrefab;
    public GameObject maxHpPotionPrefab;
    public GameObject killAllPotionPrefab;

    [Header("HP Item Spawn Settings")]
    public float minX, maxX, minY, maxY; // oblast pro náhodný spawn HP itemů
    public int maxHPItems = 5; // maximální počet HP itemů na mapě
    private List<GameObject> spawnedHPItems = new List<GameObject>(); // sledování aktivních HP itemů

    [Header("Potion Spawn Settings")]
    public Transform fullHpSpawner;
    public Transform killAllSpawner;

    private GameObject activeFullHpPotion = null;
    private GameObject activeKillAllPotion = null;

    [Header("Spawn Timers")]
    public float hpItemSpawnDelay = 10f; // počáteční zpoždění pro HP srdce
    public float hpItemSpawnInterval = 12f; // interval pro spawn HP srdce

    public float initialPotionSpawnDelay = 20f; // počáteční zpoždění pro spawn potionů
    public Vector2 potionRespawnDelayRange = new Vector2(5f, 10f); // random interval pro respawn potionů

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
        StartCoroutine(SpawnHPItemsOverTime());
        StartCoroutine(DelayedPotionSpawn());
    }

    private IEnumerator SpawnHPItemsOverTime()
    {
        yield return new WaitForSeconds(hpItemSpawnDelay); // počáteční zpoždění

        while (true)
        {
            if (spawnedHPItems.Count < maxHPItems) // kontrola maximálního limitu
            {
                SpawnRandomHPItem();
            }
            yield return new WaitForSeconds(hpItemSpawnInterval);
        }
    }

    private void SpawnRandomHPItem()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0);

        GameObject hpItem = Instantiate(hpItemPrefab, randomPosition, Quaternion.identity);
        spawnedHPItems.Add(hpItem);
    }

    public void RemoveHPItem(GameObject hpItem)
    {
        if (spawnedHPItems.Contains(hpItem))
        {
            spawnedHPItems.Remove(hpItem);
            Destroy(hpItem);
        }
    }

    private IEnumerator DelayedPotionSpawn()
    {
        yield return new WaitForSeconds(initialPotionSpawnDelay); // první spawn po startu hry

        if (fullHpSpawner != null)
        {
            activeFullHpPotion = Instantiate(maxHpPotionPrefab, fullHpSpawner.position, Quaternion.identity);
        }

        if (killAllSpawner != null)
        {
            activeKillAllPotion = Instantiate(killAllPotionPrefab, killAllSpawner.position, Quaternion.identity);
        }
    }

    // respawnuje potion po určité době

    public void PotionCollected(string potionType)
    {
        StartCoroutine(RespawnPotionWithDelay(potionType));
    }

    private IEnumerator RespawnPotionWithDelay(string potionType)
    {
        float delay = Random.Range(potionRespawnDelayRange.x, potionRespawnDelayRange.y);
        yield return new WaitForSeconds(delay);

        if (potionType == "HealingPotion" && fullHpSpawner != null && activeFullHpPotion == null)
        {
            activeFullHpPotion = Instantiate(maxHpPotionPrefab, fullHpSpawner.position, Quaternion.identity);
        }
        else if (potionType == "KillAllPotion" && killAllSpawner != null && activeKillAllPotion == null)
        {
            activeKillAllPotion = Instantiate(killAllPotionPrefab, killAllSpawner.position, Quaternion.identity);
        }
    }
}