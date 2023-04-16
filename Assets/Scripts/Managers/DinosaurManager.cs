using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class DinosaurManager : MonoBehaviour
{
    [SerializeField] private float jumpForce;

    private bool canJump;
    private Animator animator;
    private Rigidbody2D rigidbody;

    public event Action OnDead;

    private void Awake()
    {
        canJump = true;

        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();

        animator.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground")) 
        {
            canJump = true;
        }

        if (collision.collider.CompareTag("Respawn")) 
        {
            Die();
        }
    }

    public void Jump()
    {
        if (canJump == false || 
            (GameManager.Instance.State == GameState.Home || GameManager.Instance.State == GameState.Restarting)) return;
        
        canJump = false;
        animator.enabled = true;

        rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Die() 
    {
        animator.SetTrigger("Dead");

        OnDead?.Invoke();
    }
}
