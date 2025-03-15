using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameOver : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text positionText;
    public Button restartButton;
    public Button mainMenuButton;

    [Header("Parallax Settings")]
    public RectTransform backgroundPanel;
    public float parallaxStrength = 15f;
    private Vector2 startPos;
    private Vector2 canvasSize;

    [SerializeField] private MusicManager musicManager; // 🔊 Přidána reference na MusicManager

    void Start()
    {
        float lastScore = PlayerPrefs.GetFloat("LastScore", 0);
        string playerNickname = PlayerPrefs.GetString("PlayerNickname", "Unknown");

        scoreText.text = $"{playerNickname} - {lastScore:F1} s";

        UpdateLeaderboard(playerNickname, lastScore);

        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(BackToMainMenu);

        if (backgroundPanel != null)
        {
            startPos = backgroundPanel.anchoredPosition;
            canvasSize = new Vector2(Screen.width, Screen.height);
        }
    }

    void Update()
    {
        if (backgroundPanel != null)
        {
            Vector2 mousePos = new Vector2(Input.mousePosition.x / canvasSize.x * 2 - 1, Input.mousePosition.y / canvasSize.y * 2 - 1);
            Vector2 targetPos = startPos + mousePos * parallaxStrength;
            backgroundPanel.anchoredPosition = Vector2.Lerp(backgroundPanel.anchoredPosition, targetPos, Time.deltaTime * 5);
        }
    }

    void UpdateLeaderboard(string nickname, float newScore)
    {
        List<LeaderboardEntry> scores = new List<LeaderboardEntry>();

        // ✅ Načítání existujících záznamů
        for (int i = 0; i < 10; i++)
        {
            string nameKey = "LeaderboardName_" + i;
            string scoreKey = "LeaderboardScore_" + i;

            if (PlayerPrefs.HasKey(scoreKey))
            {
                scores.Add(new LeaderboardEntry(
                    PlayerPrefs.GetString(nameKey),
                    PlayerPrefs.GetFloat(scoreKey)
                ));
            }
        }

        // ✅ **Kontrola, zda už hráč v tabulce existuje**
        var existingEntry = scores.FirstOrDefault(e => e.nickname == nickname);

        if (existingEntry != null)
        {
            if (existingEntry.score >= newScore)
            {
                Debug.Log($"⏳ {nickname} už má lepší skóre: {existingEntry.score:F1} s");
                return; // Hráč už má lepší skóre, neukládáme nové
            }
            else
            {
                scores.Remove(existingEntry); // ❌ Smazání starého výsledku
            }
        }

        // ✅ **Přidání nového nejlepšího skóre hráče**
        scores.Add(new LeaderboardEntry(nickname, newScore));
        scores = scores.OrderByDescending(e => e.score).Take(10).ToList();

        // ✅ **Najít nové umístění hráče**
        int position = scores.FindIndex(e => e.nickname == nickname) + 1;
        positionText.text = position > 0 ? $"Placement: {position} / {scores.Count}" : "Placement: - / 10";

        // ✅ **Uložení leaderboardu**
        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetString("LeaderboardName_" + i, scores[i].nickname);
            PlayerPrefs.SetFloat("LeaderboardScore_" + i, scores[i].score);
        }

        PlayerPrefs.Save();
    }

    public void RestartGame()
    {
        // 🔊 Přehrát zvuk tlačítka
        if (musicManager != null)
        {
            musicManager.PlaySFX(musicManager.buttonClickSound);
        }
        else
        {
            Debug.LogWarning("⚠️ MusicManager nebyl nalezen! Zvuk tlačítka se nespustí.");
        }

        SceneManager.LoadScene("GameScene");
    }

    public void BackToMainMenu()
    {
        // 🔊 Přehrát zvuk tlačítka
        if (musicManager != null)
        {
            musicManager.PlaySFX(musicManager.buttonClickSound);
        }
        else
        {
            Debug.LogWarning("⚠️ MusicManager nebyl nalezen! Zvuk tlačítka se nespustí.");
        }

        SceneManager.LoadScene("MainMenuScene");
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string nickname;
    public float score;

    public LeaderboardEntry(string nickname, float score)
    {
        this.nickname = nickname;
        this.score = score;
    }
}