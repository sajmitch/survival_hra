using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public float attackDuration = 1.1f;

    [Header("Attack Settings")]
    public Transform attackZone;

    [Header("Collision Settings")]
    public LayerMask groundLayer;
    public LayerMask wallLayer; // vrstva pro zdi
    public float wallCheckDistance = 0.2f; // jak daleko bude Raycast kontrolovat stěnu

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded = true;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isTouchingWall = false; // kontrola, jestli hráč narazil do zdi

    AudioManager audioManager;

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

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (attackZone == null)
        {
            Debug.LogError("AttackZone není připojena k hráči!");
        }
    }

    void Update()
    {
        if (isDead) return;

        float move = Input.GetAxisRaw("Horizontal");

        // detekce zdi pomocí toho Raycastu
        isTouchingWall = CheckWallCollision(move);

        // hráč se nehýbe, takže vypnout animaci běhu
        if (move == 0 || isTouchingWall)
        {
            animator.SetFloat("Speed", 0);
        }
        else if (isGrounded && !isAttacking)
        {
            animator.SetFloat("Speed", Mathf.Abs(move));
        }

        // pokud je hráč u zdi
        if (isTouchingWall)
        {
            move = 0; // zabrání posunu proti zdi
        }

        // pohyb hráče
        if (isAttacking && isGrounded)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (!isAttacking)
            {
                rb.velocity = new Vector2(move * speed, rb.velocity.y);
            }
        }

        // aby měl správnou stranu na směr attackZone
        if (move > 0)
        {
            spriteRenderer.flipX = false;
            if (attackZone != null) attackZone.localPosition = new Vector2(0.5f, 0);
        }
        else if (move < 0)
        {
            spriteRenderer.flipX = true;
            if (attackZone != null) attackZone.localPosition = new Vector2(-0.5f, 0);
        }

        // skákání (funguje s Space i W, ale jen když je hráč na zemi logicky)
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isGrounded && !isAttacking)
        {
            Jump();
        }

        
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animator.SetTrigger("JumpTrigger");
        isGrounded = false;
        animator.SetBool("isGrounded", false);

        // zvuk skoku
        audioManager.PlaySFX(audioManager.jumpSound);

    }

    // detekce země a kolize se stěnou
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isGrounded", true);
            animator.SetFloat("Speed", 0);
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

    // raycast detekce stěny
    private bool CheckWallCollision(float moveDirection)
    {
        if (moveDirection == 0) return false;

        Vector2 direction = moveDirection > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, wallCheckDistance, wallLayer);

        return hit.collider != null; // pokud něco trefí, vrátí true (hráč je u zdi)
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        GameManager.Instance.isPlayerAttacking = true;

        animator.SetTrigger("AttackTrigger");
        audioManager.PlaySFX(audioManager.attackSound);

        if (isGrounded)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
        GameManager.Instance.isPlayerAttacking = false;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (isGrounded)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    // cervenej hit - GPT
    public void FlashRed()
    {
        StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    public void TriggerDeathAnimation()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Spuštěna animace smrti hráče");

        animator.SetTrigger("Death");
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        audioManager.PlaySFX(audioManager.deathSound);

    }

    public float GetDeathAnimationLength()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }
}