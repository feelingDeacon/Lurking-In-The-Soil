using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRuntime : MonoBehaviour
{
    public float baseMoveSpeed = 25;
    public float baseAttackRadius = 2.5f;
    public int baseAttackDamage = 1;
    public Rigidbody2D rb;
    public Animator animator;
    public bool isAttacking;
    public bool _attacked;
    public Vector3 playerAttackDir;

    public float CurrMoveSpeed => baseMoveSpeed + UpgradeManager.Instance.extraWalkSpeed;
    public float CurrAttackRadius => baseAttackRadius + UpgradeManager.Instance.extraAttackRange;
    public int CurrAttackDamage => baseAttackDamage + UpgradeManager.Instance.extraAttackDamage;
    
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

    public ParticleSystem attackPS;
    
    public void Attack()
    {
        if (_attacked) return;

        _attacked = true;
        Vector2 attackCenter = transform.position + playerAttackDir * 3;
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackCenter,
            CurrAttackRadius, 1 << 13);
        foreach (var hitTarget in hitTargets)
        {
            RootBlock root = hitTarget.GetComponent<RootBlock>();
            root.GetHurt(CurrAttackDamage);
        }

        attackPS.transform.position = attackCenter;
        attackPS.transform.localScale = new Vector3(CurrAttackRadius, CurrAttackRadius, CurrAttackRadius);
        attackPS.transform.right = (playerAttackDir).normalized;
        attackPS.Play();
        // Debug.Log("SSSS " + attackCenter + ", " + CurrAttackRadius + ", " + playerAttackDir);
    }
    
    public void AttackEnd()
    {
        isAttacking = false;
    }

    public void UpdateMovement()
    {
        if (movement.magnitude != 0)
        {
            rb.MovePosition(rb.position + movement.normalized * CurrMoveSpeed * Time.fixedDeltaTime);
        }
    }
}
