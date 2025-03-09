using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Slider healthSlider; // Odkaz na UI Slider
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;

        if (healthSlider == null)
        {
            healthSlider = GameObject.Find("PlayerHealthBar")?.GetComponent<Slider>(); // 🔍 Automaticky hledá Slider
        }

        if (healthSlider == null)
        {
            Debug.LogError("❌ Chybí reference na HealthBar! Připoj ho ke GameManageru.");
            return;
        }

        healthSlider.maxValue = gameManager.playerMaxHP;
        healthSlider.value = gameManager.playerMaxHP; // HP na max při startu
    }

    public void UpdateHealthUI(int currentHP)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHP;
        }
    }
}