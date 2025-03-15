using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 🎮 **GameManager – Správa hry, skóre, hráčova HP, nepřátel a tlačítka Kill All**
/// </summary>
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
    public TMP_Text scoreText;

    [Header("UI Elements")]
    public Slider playerHealthBar;
    public Button killAllButton;

    private AudioManager audioManager;

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

        // 🔍 Najdi AudioManager ve scéně
        audioManager = FindObjectOfType<AudioManager>();

        // 🎚️ Nastavení HP baru
        if (playerHealthBar != null)
        {
            playerHealthBar.minValue = 0;
            playerHealthBar.maxValue = playerMaxHP;
            playerHealthBar.value = playerCurrentHP;
        }
        else
        {
            Debug.LogWarning("⚠️ PlayerHealthBar nebyl nalezen!");
        }

        // 🎮 Nastavení tlačítka Kill All
        if (killAllButton != null)
        {
            killAllButton.gameObject.SetActive(false);
            killAllButton.onClick.RemoveAllListeners();
            killAllButton.onClick.AddListener(KillAllEnemies);
        }
        else
        {
            Debug.LogWarning("⚠️ KillAllButton nebyl nalezen!");
        }
    }

    void Update()
    {
        if (isPlayerAlive)
        {
            survivalTime += Time.deltaTime;
            if (scoreText != null)
            {
                scoreText.text = survivalTime.ToString("F1") + " s"; // Zobrazení skóre ve formátu "12.5 s"
            }
        }
    }

    public float GetSurvivalTime()
    {
        return survivalTime;
    }

    /// <summary>
    /// ✅ **Aktualizuje UI HP baru**
    /// </summary>
    public void UpdateHealthUI()
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.value = playerCurrentHP;
        }
    }

    /// <summary>
    /// 🚨 **Hráč dostane poškození**
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isPlayerAttacking) return;

        playerCurrentHP -= damage;
        playerCurrentHP = Mathf.Max(playerCurrentHP, 0);

        UpdateHealthUI();
        PlayerMovement.Instance?.FlashRed();

        // 🎵 **Zvuk zásahu hráče**
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.hitSound);
        }
        else
        {
            Debug.LogWarning("⚠️ AudioManager nebyl nalezen! Zvuk zásahu se nespustí.");
        }

        if (playerCurrentHP == 0)
        {
            PlayerDeath();
        }
    }

    /// <summary>
    /// 💀 **Hráč umírá**
    /// </summary>
    private void PlayerDeath()
    {
        isPlayerAlive = false;
        PlayerMovement.Instance?.TriggerDeathAnimation();

        // 🎵 **Zvuk smrti hráče**
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.deathSound);
        }

        float deathAnimationLength = PlayerMovement.Instance?.GetDeathAnimationLength() ?? 1.5f;
        StartCoroutine(DelayedGameOver(deathAnimationLength));
    }

    private IEnumerator DelayedGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayerPrefs.SetFloat("LastScore", survivalTime);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameOverScene");
    }

    /// <summary>
    /// ❤️ **Léčí hráče**
    /// </summary>
    public void Heal(int amount)
    {
        playerCurrentHP = Mathf.Min(playerCurrentHP + amount, playerMaxHP);
        UpdateHealthUI();
    }

    /// <summary>
    /// ☠️ **Aktivuje tlačítko pro Kill-All**
    /// </summary>
    public void EnableKillAllButton()
    {
        if (killAllButton != null)
        {
            killAllButton.gameObject.SetActive(true);
            killAllButton.onClick.RemoveAllListeners();
            killAllButton.onClick.AddListener(KillAllEnemies);

            // 🎵 **Přehrání zvuku sebrání collectable**
            if (audioManager != null)
            {
                audioManager.PlaySFX(audioManager.collectSound);
            }
            else
            {
                Debug.LogWarning("⚠️ AudioManager není dostupný, zvuk se nespustí.");
            }
        }
    }

    /// <summary>
    /// 🔥 **Zabije všechny nepřátele a deaktivuje tlačítko**
    /// </summary>
    public void KillAllEnemies()
    {
        // 🎵 **Přehrání zvuku aktivace tlačítka**
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.collectSound);
        }
        else
        {
            Debug.LogWarning("⚠️ AudioManager není dostupný, zvuk tlačítka Kill All se nespustí.");
        }

        // 🏴‍☠️ Zničí všechny nepřátele ve scéně
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