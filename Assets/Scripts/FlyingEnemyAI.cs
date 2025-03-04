using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour
{
    private Transform player; // Odkaz na hráče
    public float speed = 3f;
    public float detectionRange = 6f;
    public LayerMask groundLayer; // Určuje, co jsou překážky

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isFollowingPlayer = false;
    private Vector2 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        moveDirection = Vector2.right; // Netopýr začne letět doprava

        // Najdi hráče přes Singleton
        if (PlayerMovement.Instance != null)
        {
            player = PlayerMovement.Instance.transform;
        }
    }

    void Update()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            isFollowingPlayer = true;
        }
        else
        {
            isFollowingPlayer = false;
        }
    }

    void FixedUpdate()
    {
        if (isFollowingPlayer)
        {
            FollowPlayer();
        }
        else
        {
            MoveHorizontally();
        }
    }

    void MoveHorizontally()
    {
        rb.velocity = new Vector2(moveDirection.x * speed, rb.velocity.y);

        // Raycast detekující stěnu nebo platformu
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, 1.0f, groundLayer);
        if (hit.collider != null)
        {
            FlipDirection(); // Změna směru při nárazu
        }
    }

    void FollowPlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(directionToPlayer.x * speed, directionToPlayer.y * speed);

        // Otočení sprite podle směru pohybu
        if (rb.velocity.x > 0)
        {
            spriteRenderer.flipX = false; // Směr doprava
        }
        else if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = true; // Směr doleva
        }
    }

    void FlipDirection()
    {
        moveDirection.x *= -1; // Změna směru na opačný

        // Správné otočení netopýra
        spriteRenderer.flipX = moveDirection.x < 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Pokud narazí do překážky, změní směr
        {
            FlipDirection();
        }
    }
}