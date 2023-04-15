using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class DinosaurManager : MonoBehaviour
{
    [SerializeField] private float jumpForce;

    private bool canJump;
    private Rigidbody2D rigidbody;

    private void Awake()
    {
        canJump = true;

        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool receivedInput = Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0;

        if (receivedInput && canJump)
        {
            canJump = false;

            rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground")) 
        {
            canJump = true;
        }
    }
}
