using UnityEngine;

public class AttackZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                Debug.Log("🔥 Nepřítel zasažen!"); // Debug zpráva do konzole
                enemyHealth.TakeDamage(1);
            }
            else
            {
                Debug.LogWarning("⚠ Nepřítel nemá EnemyHealth script!");
            }
        }
    }
}