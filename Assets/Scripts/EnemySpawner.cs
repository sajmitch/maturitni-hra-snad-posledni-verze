using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab nepřítele
    public Transform[] spawnPoints; // Spawnovací body (Spawner1 a Spawner2)
    public float spawnInterval = 3f; // Interval mezi spawnováním

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Náhodně vyber spawnovací bod
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Vytvoř nepřítele na daném bodě
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}