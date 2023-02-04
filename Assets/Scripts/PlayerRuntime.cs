using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRuntime : MonoBehaviour
{
    public float moveSpeed = 5;
    public Rigidbody2D rb;
    public Animator animator;
    public bool isAttacking;
    
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Attack");
            isAttacking = true;
        }
    }

    public void Attack()
    {
        
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
