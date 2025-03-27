using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 5;
    public float attackCooldown = 2f;
    private bool canAttack = true;
    private bool playerInRange = false;
    private Transform playerTransform;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerInRange && canAttack && !GameManager.Instance.isPlayerAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            playerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private IEnumerator Attack()
    {
        canAttack = false;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f);

        if (playerTransform != null && !GameManager.Instance.isPlayerAttacking)
        {
            GameManager.Instance.TakeDamage(damage);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}