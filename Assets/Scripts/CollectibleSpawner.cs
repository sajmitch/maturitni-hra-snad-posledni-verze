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
    public float minX, maxX, minY, maxY; // Oblast pro náhodný spawn HP itemů
    public int maxHPItems = 5; // Maximální počet HP listů na mapě
    private List<GameObject> spawnedHPItems = new List<GameObject>(); // Sledování aktivních HP itemů

    [Header("Potion Spawn Settings")]
    public Transform fullHpSpawner;
    public Transform killAllSpawner;

    private GameObject activeFullHpPotion = null;
    private GameObject activeKillAllPotion = null;

    [Header("Spawn Timers")]
    public float hpItemSpawnDelay = 10f; // Počáteční zpoždění pro HP listy
    public float hpItemSpawnInterval = 12f; // Interval pro spawn HP listů

    public float initialPotionSpawnDelay = 20f; // ⏳ Počáteční zpoždění pro spawn potionů
    public Vector2 potionRespawnDelayRange = new Vector2(5f, 10f); // Random interval pro respawn potionů

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

    /// <summary>
    /// Pravidelně spawnuje HP itemy do maximálního limitu
    /// </summary>
    private IEnumerator SpawnHPItemsOverTime()
    {
        yield return new WaitForSeconds(hpItemSpawnDelay); // Počáteční zpoždění

        while (true)
        {
            if (spawnedHPItems.Count < maxHPItems) // Kontrola maximálního limitu
            {
                SpawnRandomHPItem();
            }
            yield return new WaitForSeconds(hpItemSpawnInterval);
        }
    }

    /// <summary>
    /// Vytvoří HP item na náhodné pozici v určené oblasti
    /// </summary>
    private void SpawnRandomHPItem()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0);

        GameObject hpItem = Instantiate(hpItemPrefab, randomPosition, Quaternion.identity);
        spawnedHPItems.Add(hpItem);
    }

    /// <summary>
    /// Odstraní sebraný HP item ze seznamu
    /// </summary>
    public void RemoveHPItem(GameObject hpItem)
    {
        if (spawnedHPItems.Contains(hpItem))
        {
            spawnedHPItems.Remove(hpItem);
            Destroy(hpItem);
        }
    }

    /// <summary>
    /// Počká na začáteční dobu a pak spawne potiony
    /// </summary>
    private IEnumerator DelayedPotionSpawn()
    {
        yield return new WaitForSeconds(initialPotionSpawnDelay); // 🕒 První spawn po startu hry

        if (fullHpSpawner != null)
        {
            activeFullHpPotion = Instantiate(maxHpPotionPrefab, fullHpSpawner.position, Quaternion.identity);
        }

        if (killAllSpawner != null)
        {
            activeKillAllPotion = Instantiate(killAllPotionPrefab, killAllSpawner.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Respawnuje potion po určité době
    /// </summary>
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