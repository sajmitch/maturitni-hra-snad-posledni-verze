using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float initialSpawnInterval = 5f;
    public float minSpawnInterval = 2f;
    public float spawnReductionRate = 0.95f; // Každý spawn se zkrátí o 5 %

    public int enemyBaseHP = 1;
    public float hpIncreaseInterval = 10f; // Každých 10 sekund přidáme HP

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

            Transform spawnPoint = GetValidSpawnPoint();
            if (spawnPoint != null)
            {
                GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                newEnemy.SetActive(true);

                EnemyHealth enemyHealth = newEnemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.SetMaxHP(currentEnemyHP); // ✅ Nastavení HP pro nového netopýra
                }

                currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval * spawnReductionRate);
            }
            else
            {
                Debug.LogWarning("⚠️ Žádné volné místo pro spawn nepřítele!");
                yield return new WaitForSeconds(1f);
            }
        }
    }

    IEnumerator IncreaseEnemyHPOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(hpIncreaseInterval);
            currentEnemyHP++;
            Debug.Log("⬆️ Zvýšené HP nepřátel na: " + currentEnemyHP);
        }
    }

    Transform GetValidSpawnPoint()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            Collider2D overlap = Physics2D.OverlapCircle(spawnPoint.position, 1f, LayerMask.GetMask("Enemy"));
            if (overlap == null)
            {
                return spawnPoint;
            }
        }
        return null;
    }
}