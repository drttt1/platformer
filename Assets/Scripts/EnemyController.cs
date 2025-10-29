using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private bool isAlive = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isAlive) return;

        Move();
        CheckForWall();
    }

    private void Move()
    {
        float direction = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(direction * speed, rb.velocity.y);
    }

    private void CheckForWall()
    {
        bool isGroundedAhead = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (!isGroundedAhead)
        {
            movingRight = !movingRight;
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (IsPlayerJumpingOnHead(collision))
            {
                Die();
            }
            else
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
            }
        }
    }

    private bool IsPlayerJumpingOnHead(Collision2D collision)
    {
        float playerBottom = collision.gameObject.GetComponent<Collider2D>().bounds.min.y;
        float enemyTop = GetComponent<Collider2D>().bounds.max.y;

        return playerBottom >= enemyTop - 0.1f && collision.rigidbody.velocity.y < 0;
    }

    private void Die()
    {
        isAlive = false;
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.1f);
    }
}
