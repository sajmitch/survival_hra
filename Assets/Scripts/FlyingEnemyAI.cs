using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour
{
    private Transform player;
    public float speed = 3f;
    public float detectionRange = 6f;
    public float minDistanceFromPlayer = 2f;
    public float stopDistance = 1.5f;

    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isFollowingPlayer = false;
    private Vector2 moveDirection;
    private bool isAttacking = false;

    private float idleTime = 0f; // čas co se nehejbe
    public float maxIdleTime = 1.5f;
    public float velocityThreshold = 0.05f; // takovej předěl

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        moveDirection = Vector2.right;

        if (PlayerMovement.Instance != null)
        {
            player = PlayerMovement.Instance.transform;
        }
    }

    void Update()
    {
        if (isAttacking) return;

        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            isFollowingPlayer = true;
        }
        else
        {
            isFollowingPlayer = false;
        }

        // kontrola nehybnosti
        if (rb.velocity.magnitude < velocityThreshold)
        {
            idleTime += Time.deltaTime;
            if (idleTime >= maxIdleTime)
            {
                ChangeRandomDirection();
                idleTime = 0f;
            }
        }
        else
        {
            idleTime = 0f; // reset časovače, pokud se netopýr hejbe
        }
    }

    void FixedUpdate()
    {
        if (isAttacking) return;

        if (isFollowingPlayer)
        {
            MaintainDistance();
        }
        else
        {
            MoveHorizontally();
        }
    }

    void MaintainDistance()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > minDistanceFromPlayer)
        {
            FollowPlayer();
        }
        else if (distanceToPlayer < stopDistance)
        {
            rb.velocity = Vector2.zero;
        }
    }

    void FollowPlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(directionToPlayer.x * speed, directionToPlayer.y * speed);

        if (rb.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    void MoveHorizontally()
    {
        rb.velocity = new Vector2(moveDirection.x * speed, rb.velocity.y);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, 1.0f, groundLayer);
        if (hit.collider != null)
        {
            FlipDirection();
        }
    }

    void FlipDirection()
    {
        moveDirection.x *= -1;
        spriteRenderer.flipX = moveDirection.x < 0;
    }

    // když se netopýr sekne, náhodně změní směr - gpt
    void ChangeRandomDirection()
    {
        Debug.Log("Netopýr změnil směr, aby se uvolnil!");

        int randomChoice = Random.Range(0, 3);
        switch (randomChoice)
        {
            case 0:
                moveDirection = Vector2.right; 
                break;
            case 1:
                moveDirection = Vector2.left; 
                break;
            case 2:
                moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized; // náhodný směr
                break;
        }

        // aby se netopýr dostal pryč
        rb.velocity = moveDirection * speed * 1.2f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            FlipDirection();
        }
    }

    public void ResumeMovement()
    {
        isAttacking = false;
        isFollowingPlayer = true;
    }

    public void StopMovement()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
    }
}