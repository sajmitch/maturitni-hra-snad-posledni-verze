using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel; // ⚪ Panel s pauzovacím menu
    public TMP_Text pauseScoreText; // 🕒 Zobrazení skóre při pauze
    public Button resumeButton;
    public Button quitButton;

    private bool isPaused = false;

    // AudioManager audioManager;
    [SerializeField] private AudioManager audioManager;

    void Start()
    {
        // 🎯 Skrytí panelu na začátku
        pausePanel.SetActive(false);

        // Přidání listenerů na tlačítka
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);

       if (audioManager == null)
            {
                audioManager = FindObjectOfType<AudioManager>();
            }
        
    }

    void Update()
    {
        // 🎮 Pauznutí pomocí Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
            audioManager.PlaySFX(audioManager.buttonClickSound);
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // ⏸ Zastaví čas ve hře
        pausePanel.SetActive(true);

        // 🕒 Zobrazení aktuálního skóre (čas přežití)
        if (GameManager.Instance != null)
        {
            float survivalTime = GameManager.Instance.GetSurvivalTime();
            pauseScoreText.text = $"Time: {survivalTime:F1} s";
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        // 🔊 Zvuk kliknutí
        audioManager.PlaySFX(audioManager.buttonClickSound);
    }

    public void QuitGame()
    {
        // 🔊 Zvuk kliknutí
        // AudioManager.Instance.PlaySFX(AudioManager.Instance.attackSound);


        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnButtonClick()
    {
        if (audioManager != null && audioManager.buttonClickSound != null)
        {
            audioManager.PlaySFX(audioManager.buttonClickSound);
        }
        else
        {
            Debug.LogError("❌ AudioManager nebo buttonClickSound je null!");
        }
    }
}