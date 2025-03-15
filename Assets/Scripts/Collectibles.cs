using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType { HP, MaxHP, KillAll }
    public CollectibleType type; // Typ collectable
    private AudioManager audioManager; // ✅ Přidána reference na AudioManager

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>(); // ✅ Najde AudioManager v aktivní scéně
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Pokud hráč sebere item
        {
            if (audioManager != null)
            {
                audioManager.PlaySFX(audioManager.collectSound); // 🔊 Přehrání zvuku ještě před zničením
            }
            else
            {
                Debug.LogWarning("⚠️ AudioManager nebyl nalezen! Zvuk se nespustí.");
            }

            switch (type)
            {
                case CollectibleType.HP:
                    GameManager.Instance.Heal(1); // Přidá 1 HP hráči
                    Debug.Log("💚 Hráč získal +1 HP");
                    CollectibleSpawner.Instance.RemoveHPItem(gameObject); // ✅ Opravené volání funkce
                    break;

                case CollectibleType.MaxHP:
                    GameManager.Instance.Heal(GameManager.Instance.playerMaxHP); // Nastaví HP na maximum
                    Debug.Log("💖 Hráč získal Max HP Potion!");
                    CollectibleSpawner.Instance.PotionCollected("HealingPotion"); // ✅ Opravené volání funkce
                    Destroy(gameObject);
                    break;

                case CollectibleType.KillAll:
                    GameManager.Instance.EnableKillAllButton(); // Aktivuje tlačítko pro zabití nepřátel                   
                    Debug.Log("💀 Kill-All Potion sebran! Tlačítko aktivováno.");
                    CollectibleSpawner.Instance.PotionCollected("KillAllPotion"); // ✅ Opravené volání funkce
                    Destroy(gameObject);
                    break;
            }
        }
    }
}