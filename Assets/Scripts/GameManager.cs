using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    public int playerMaxHP = 5;
    private int playerCurrentHP;
    public bool isPlayerAttacking = false; // Blokuje damage během útoku hráče

    [Header("UI Elements")]
    public Slider playerHealthBar; // HP bar hráče
    public Button killAllButton; // Tlačítko pro zabití všech nepřátel

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

        // 🔍 Automaticky najde Slider, pokud není přiřazen ručně
        if (playerHealthBar == null)
        {
            playerHealthBar = GameObject.Find("PlayerHealthBar")?.GetComponent<Slider>();
        }

        if (playerHealthBar != null)
        {
            playerHealthBar.minValue = 0;
            playerHealthBar.maxValue = playerMaxHP;
            playerHealthBar.value = playerCurrentHP;
        }

        // 🔍 Najdeme tlačítko Kill-All (musí být deaktivované na startu)
        if (killAllButton == null)
        {
            killAllButton = GameObject.Find("KillAllButton")?.GetComponent<Button>();
        }

        if (killAllButton != null)
        {
            killAllButton.gameObject.SetActive(false);
            killAllButton.onClick.AddListener(KillAllEnemies);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isPlayerAttacking) return; // Blokuje damage během útoku hráče

        playerCurrentHP -= damage;
        if (playerCurrentHP < 0) playerCurrentHP = 0; // HP nesmí jít do mínusu

        Debug.Log("🔥 Hráč dostal hit! HP: " + playerCurrentHP);

        if (playerHealthBar != null)
        {
            playerHealthBar.value = playerCurrentHP;
        }

        PlayerMovement.Instance?.FlashRed();
  

        if (playerCurrentHP == 0)
        {
            StartCoroutine(PlayerDeath());
        }
    }

    private IEnumerator PlayerDeath()
    {
        Debug.Log("💀 Hráč zemřel!");

        PlayerMovement.Instance?.TriggerDeathAnimation();
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Heal(int amount)
    {
        playerCurrentHP += amount;
        if (playerCurrentHP > playerMaxHP) playerCurrentHP = playerMaxHP;

        Debug.Log("💚 Hráč se uzdravil! HP: " + playerCurrentHP);

        if (playerHealthBar != null)
        {
            playerHealthBar.value = playerCurrentHP;
        }
    }

    public void EnableKillAllButton()
    {
        if (killAllButton != null)
        {
            killAllButton.gameObject.SetActive(true);
        }
    }

    public void KillAllEnemies()
    {
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }

        if (killAllButton != null)
        {
            killAllButton.gameObject.SetActive(false);
        }
    }
}