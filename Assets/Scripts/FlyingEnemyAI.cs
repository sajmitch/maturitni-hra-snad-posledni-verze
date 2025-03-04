using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlyingEnemyAI : MonoBehaviour
{
    private Transform player;
    public float speed = 3f;
    public float detectionRange = 6f;
    public LayerMask groundLayer;

    [Header("Enemy Health Settings")]
    public int maxHP = 1;
    private int currentHP;
    public Text textHP;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isFollowingPlayer = false;
    private bool isDead = false;
    private Vector2 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        moveDirection = Vector2.right;

        if (PlayerMovement.Instance != null)
        {
            player = PlayerMovement.Instance.transform;
        }

        if (textHP == null)
        {
            textHP = GetComponentInChildren<Text>();
        }

        currentHP = maxHP;
        UpdateHPText();
    }

    void Update()
    {
        if (isDead) return;

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
        if (isDead) return;

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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, 0.6f, groundLayer);
        if (hit.collider != null)
        {
            FlipDirection();
        }
    }

    void FollowPlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(directionToPlayer.x * speed, directionToPlayer.y * speed);
        spriteRenderer.flipX = directionToPlayer.x < 0;
    }

    void FlipDirection()
    {
        moveDirection.x *= -1;
        spriteRenderer.flipX = moveDirection.x < 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            FlipDirection();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        UpdateHPText();
        animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    void UpdateHPText()
    {
        if (textHP != null)
        {
            textHP.text = currentHP.ToString();
        }
    }
}