using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackZone; // Reference na AttackZone
    public float attackDuration = 0.3f; // Jak dlouho bude hitbox aktivní
    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        attackZone.gameObject.SetActive(false); // Na začátku je vypnutý
    }

    void Update()
    {
        // Útok na levé tlačítko myši
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("AttackTrigger");

        // Aktivace hitboxu
        attackZone.gameObject.SetActive(true);

        yield return new WaitForSeconds(attackDuration); // Počkáme na konec útoku

        attackZone.gameObject.SetActive(false); // Deaktivace hitboxu
        isAttacking = false;
    }
}