using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; // pole víc spawn pointů
    public float initialSpawnInterval = 5f;
    public float minSpawnInterval = 2f; 
    public float spawnReductionRate = 0.95f; // zkracování intervalu

    public int enemyBaseHP = 1;
    public float hpIncreaseInterval = 10f; // zvýšení HP každých X sekund

    private float currentSpawnInterval;
    private int currentEnemyHP;

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        currentEnemyHP = enemyBaseHP;

        StartCoroutine(SpawnEnemies());
        StartCoroutine(IncreaseEnemyHPOverTime());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnInterval);

            if (spawnPoints.Length > 0)
            {
                // vybere náhodně 1 spawn bod
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                SpawnEnemyAt(spawnPoint);
            }
            else
            {
                Debug.LogWarning("Nejsou spawnery hele");
            }

            // redukce intervalu aby to bylo těžší
            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval * spawnReductionRate);
        }
    }

    void SpawnEnemyAt(Transform spawnPoint)
    {
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        newEnemy.SetActive(true);

        // přiřadí nepříteli správné HP
        EnemyHealth enemyHealth = newEnemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.SetMaxHP(currentEnemyHP);
        }
    }

    IEnumerator IncreaseEnemyHPOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(hpIncreaseInterval);
            currentEnemyHP++;
            Debug.Log("Zvýšené HP nepřátel na: " + currentEnemyHP);
        }
    }
}