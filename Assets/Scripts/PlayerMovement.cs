using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public float attackDuration = 1.1f; // Délka útoku

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded = true;
    private bool isAttacking = false;
    private bool attackEndedOnGround = false; // Kontroluje, zda hráč dopadl během útoku

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
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        // **Pohyb hráče (blokován jen na zemi během útoku)**
        if (!isAttacking || !isGrounded)
        {
            rb.velocity = new Vector2(move * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // **Otočení hráče**
        if (move > 0) spriteRenderer.flipX = false;
        if (move < 0) spriteRenderer.flipX = true;

        // **Animace běhu (nepokračuje, pokud je útok)**
        if (isGrounded && !isAttacking)
        {
            animator.SetFloat("Speed", Mathf.Abs(move));
        }

        // **Skákání (blokováno pouze pokud je hráč už v útoku)**
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("JumpTrigger");
            isGrounded = false;
            animator.SetBool("isGrounded", false);
        }

        // **Útok (možný i ve vzduchu)**
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    // ✅ **Správná Coroutine pro útok**
    private IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("AttackTrigger");

        // **Při útoku na zemi hráč stojí**
        if (isGrounded)
        {
            rb.velocity = Vector2.zero;
        }

        yield return new WaitForSeconds(attackDuration); // ⏳ Počkáme na konec animace

        // **Pokud hráč dopadl během útoku, zajistí že stojí**
        if (isGrounded)
        {
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.2f); // ✋ Krátké zpoždění pro plynulost
        }

        isAttacking = false;
    }

    // 🛬 **Detekce kontaktu se zemí**
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isGrounded", true);

            // **Pokud hráč dopadl během útoku, zajistí že stojí**
            if (isAttacking)
            {
                rb.velocity = Vector2.zero;
                attackEndedOnGround = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("isGrounded", false);
        }
    }
}