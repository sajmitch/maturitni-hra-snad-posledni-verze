using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType { HP, MaxHP, KillAll }
    public CollectibleType type; // Typ collectable

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Pokud hráč sebere item
        {
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
                    Destroy(gameObject);
                    CollectibleSpawner.Instance.PotionCollected("HealingPotion"); // ✅ Opravené volání funkce
                    break;

                case CollectibleType.KillAll:
                    GameManager.Instance.EnableKillAllButton(); // Aktivuje tlačítko pro zabití nepřátel
                    Debug.Log("💀 Kill-All Potion sebran! Tlačítko aktivováno.");
                    Destroy(gameObject);
                    CollectibleSpawner.Instance.PotionCollected("KillAllPotion"); // ✅ Opravené volání funkce
                    break;
            }
        }
    }
}