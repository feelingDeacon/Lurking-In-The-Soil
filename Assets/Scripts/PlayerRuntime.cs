using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRuntime : MonoBehaviour
{
    public float moveSpeed = 5;
    public float attackRadius = 2.5f;
    public int attackDamage = 1;
    public Rigidbody2D rb;
    public Animator animator;
    public bool isAttacking;
    public bool _attacked;
    public Vector3 playerAttackDir;
    
    private static PlayerRuntime _instance;

    public static PlayerRuntime Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<PlayerRuntime>();
            }

            return _instance;
        }
    }

    public void InitPlayer()
    {
        isAttacking = false;
        _attacked = false;
    }
    
    public void UpdatePlayer()
    {
        if (!isAttacking)
        {
            UpdateMovementControl();
            UpdateAttackControl();
        }
    }
    
    public void FixedUpdatePlayer()
    {
        if (!isAttacking)
        {
            UpdateMovement();
        }
    }

    private Vector2 movement;
    
    public void UpdateMovementControl()
    {
        float horiz = Input.GetAxisRaw("Horizontal");
        float verti = Input.GetAxisRaw("Vertical");
        movement.x = horiz;
        movement.y = verti;
        if (horiz != 0 || verti != 0)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }
        animator.SetFloat("Speed", movement.magnitude);
    }

    public void UpdateAttackControl()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            playerAttackDir = (mouseWorldPos - transform.position).RemoveZ().normalized;
            animator.SetFloat("AttackHorizontal", playerAttackDir.x);
            animator.SetFloat("AttackVertical", playerAttackDir.y);
            animator.SetTrigger("Attack");
            isAttacking = true;
            _attacked = false;
        }
    }

    public void Attack()
    {
        if (_attacked) return;

        _attacked = true;
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position + playerAttackDir * 3,
            attackRadius, 1 << 13);
        foreach (var hitTarget in hitTargets)
        {
            RootBlock root = hitTarget.GetComponent<RootBlock>();
            root.GetHurt(attackDamage);
        }
    }
    
    public void AttackEnd()
    {
        isAttacking = false;
    }

    public void UpdateMovement()
    {
        if (movement.magnitude != 0)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
