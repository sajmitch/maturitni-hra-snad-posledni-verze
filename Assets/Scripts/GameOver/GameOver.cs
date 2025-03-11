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
    public RectTransform backgroundPanel; // Odkaz na Panel s pozadím
    public float parallaxStrength = 15f; // Síla efektu
    private Vector2 startPos;
    private Vector2 canvasSize;

    void Start()
    {
        float lastScore = PlayerPrefs.GetFloat("LastScore", 0);
        string playerNickname = PlayerPrefs.GetString("PlayerNickname", "Unknown");

        scoreText.text = $"{playerNickname} - {lastScore:F1} s"; // Přidána mezera před "s"

        UpdateLeaderboard(playerNickname, lastScore);

        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(BackToMainMenu);

        // Inicializace parallaxu
        if (backgroundPanel != null)
        {
            startPos = backgroundPanel.anchoredPosition;
            canvasSize = new Vector2(Screen.width, Screen.height);
        }
    }

    void Update()
    {
        // 🎥 Parallax efekt podle myši
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

        // ✅ Načtení současného leaderboardu
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

        // ✅ Zjistíme, zda hráč už má lepší skóre
        var existingEntry = scores.FirstOrDefault(e => e.nickname == nickname);
        if (existingEntry != null && existingEntry.score >= newScore)
        {
            Debug.Log($"⏳ Player {nickname} already has a better score: {existingEntry.score:F1} s");
            positionText.text = $"Placement: - / {scores.Count}";
            return;
        }

        // ✅ Přidání hráče a seřazení leaderboardu
        scores.Add(new LeaderboardEntry(nickname, newScore));
        scores = scores.OrderByDescending(e => e.score).Take(10).ToList();

        // ✅ Zjistíme umístění hráče
        int position = scores.FindIndex(e => e.nickname == nickname && e.score == newScore) + 1;

        // ✅ Pokud hráč není v top 10, zobrazí `-/10`
        if (position == 0)
        {
            positionText.text = $"Placement: - / {scores.Count}";
        }
        else
        {
            positionText.text = $"Placement: {position} / {scores.Count}";
        }

        // ✅ Uložíme leaderboard
        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetString("LeaderboardName_" + i, scores[i].nickname);
            PlayerPrefs.SetFloat("LeaderboardScore_" + i, scores[i].score);
        }

        PlayerPrefs.Save();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void BackToMainMenu()
    {
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