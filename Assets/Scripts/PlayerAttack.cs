using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackZone;
    public float attackDuration = 0.3f;
    public float attackImmunityTime = 0.5f; // ochrana před útoky po úderu
    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        attackZone.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        GameManager.Instance.isPlayerAttacking = true;
        animator.SetTrigger("AttackTrigger");

        attackZone.gameObject.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        attackZone.gameObject.SetActive(false);
        isAttacking = false;
        yield return new WaitForSeconds(attackImmunityTime);

        GameManager.Instance.isPlayerAttacking = false;
    }
}