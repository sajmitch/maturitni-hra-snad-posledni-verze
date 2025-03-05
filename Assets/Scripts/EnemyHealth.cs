using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 2;
    private int currentHP;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Přidáno
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true); // Nastavení booleanu v animatoru
        rb.velocity = Vector2.zero; // Zastavení pohybu nepřítele
        rb.isKinematic = true; // Vypnutí fyziky
        GetComponent<Collider2D>().enabled = false; // Deaktivace kolize, pokud nechceš kolidovat

        // Počkáme na konec animace
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject);
    }
}