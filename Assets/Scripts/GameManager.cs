using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    public int playerMaxHP = 5;
    private int playerCurrentHP;
    public bool isPlayerAttacking = false; // Nová proměnná pro ochranu před útoky

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
        playerCurrentHP = playerMaxHP;
    }

    public void TakeDamage(int damage)
    {
        if (isPlayerAttacking) return; // Blokuje damage během útoku hráče

        playerCurrentHP -= damage;
        Debug.Log("Hráč dostal poškození: " + damage + " | Zbývá HP: " + playerCurrentHP);

        if (playerCurrentHP <= 0)
        {
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        Debug.Log("Hráč zemřel!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart scény
    }

    public void Heal(int amount)
    {
        playerCurrentHP += amount;
        if (playerCurrentHP > playerMaxHP) playerCurrentHP = playerMaxHP;
        Debug.Log("Hráč se uzdravil o " + amount + " | HP: " + playerCurrentHP);
    }
}