using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    public int playerMaxHP = 5;
    private int playerCurrentHP;
    public bool isPlayerAttacking = false;

    [Header("Score System")]
    private float survivalTime = 0f;
    private bool isPlayerAlive = true;
    public TMP_Text scoreText; // Používá TMP_Text místo Text

    [Header("UI Elements")]
    public Slider playerHealthBar;
    public Button killAllButton;

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

    void Update()
    {
        if (isPlayerAlive)
        {
            survivalTime += Time.deltaTime;

            if (scoreText != null)
            {
                scoreText.text = survivalTime.ToString("F1") + " s"; // Zobrazení jako "12.5 s"
            }
        }
    }

    public float GetSurvivalTime()
    {
        return survivalTime;
    }

    public void TakeDamage(int damage)
    {
        if (isPlayerAttacking) return;

        playerCurrentHP -= damage;
        if (playerCurrentHP < 0) playerCurrentHP = 0;

        if (playerHealthBar != null)
        {
            playerHealthBar.value = playerCurrentHP;
        }

        PlayerMovement.Instance?.FlashRed();

        if (playerCurrentHP == 0)
        {
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        isPlayerAlive = false;
        PlayerMovement.Instance?.TriggerDeathAnimation(); // Spustí animaci smrti

        float deathAnimationLength = PlayerMovement.Instance?.GetDeathAnimationLength() ?? 1.5f; // Získáme délku animace

        StartCoroutine(DelayedGameOver(deathAnimationLength)); // Počká na konec animace a přepne scénu
    }

    private IEnumerator DelayedGameOver(float delay)
    {
        yield return new WaitForSeconds(delay); // Počkej na dokončení animace
        PlayerPrefs.SetFloat("LastScore", survivalTime);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameOverScene");
    }

    public void Heal(int amount)
    {
        playerCurrentHP += amount;
        if (playerCurrentHP > playerMaxHP) playerCurrentHP = playerMaxHP;

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