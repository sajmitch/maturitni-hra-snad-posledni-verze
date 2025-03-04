using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public Transform attackZone; // Reference na AttackZone
    public float attackDuration = 0.2f; // Jak dlouho bude hitbox aktivní

    private bool isAttacking = false;
    private bool isGrounded = true;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Deaktivujeme AttackZone na startu
        attackZone.gameObject.SetActive(false);
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Pohyb hráče (nepohybuje se během útoku)
        if (!isAttacking)
        {
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }

        // Otočení hráče a přesunutí hitboxu útoku
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
            attackZone.localPosition = new Vector2(0.5f, 0);
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
            attackZone.localPosition = new Vector2(-0.5f, 0);
        }

        // Animace běhu
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // Skok
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("JumpTrigger");
            isGrounded = false;
        }

        // Útok (kliknutí myši)
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("AttackTrigger");

        // Zastaví pohyb hráče během útoku
        rb.velocity = Vector2.zero;

        attackZone.gameObject.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        attackZone.gameObject.SetActive(false);

        isAttacking = false;
    }

    // Zjištění, jestli hráč stojí na zemi
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}