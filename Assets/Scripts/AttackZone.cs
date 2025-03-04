using UnityEngine;

public class AttackZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            FlyingEnemyAI enemy = collision.GetComponent<FlyingEnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
        }
    }
}