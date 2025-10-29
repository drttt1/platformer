using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int lives = 5;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer = 1;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private int totalCoinsInLevel = 5;
    private int collectedCoins = 0;

    public int CollectedCoins { get; private set; }
    public int TotalCoinsInLevel { get; private set; }

    private bool isGrounded = false;
    private float moveInput;
    public int CurrentLives { get; private set; }

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        CurrentLives = lives;
        TotalCoinsInLevel = totalCoinsInLevel;
        CollectedCoins = 0;
    }

    private void FixedUpdate()
    {
        CheckGround();
        Move();
    }

    private void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        if (isGrounded)
        {
            if (Mathf.Abs(moveInput) > 0.1f)
                State = States.run;
            else
                State = States.idle;

            if (Input.GetButtonDown("Jump"))
                Jump();
        }
        else
        {
            State = States.jump;
        }

        if (moveInput > 0.1f)
            sprite.flipX = false;
        else if (moveInput < -0.1f)
            sprite.flipX = true;
    }

    private void Move()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = moveInput * speed;
        rb.velocity = velocity;
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        State = States.jump;
    }

    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = false;

        foreach (Collider2D col in collider)
        {
            if (col.gameObject != gameObject && !col.isTrigger)
            {
                isGrounded = true;
                break;
            }
        }
    }
    public void TakeDamage(float damage)
    {
        CurrentLives -= (int)damage;
        Debug.Log($"Player took damage! Lives left: {CurrentLives}");

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHearts(CurrentLives);

        if (CurrentLives <= 0)
            Die();
        
    }
    public void CollectCoin(int value = 1)
    {
        CollectedCoins += value;
        Debug.Log($"Coin collected! Total: {CollectedCoins}/{TotalCoinsInLevel}");

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateCoinText(CollectedCoins, TotalCoinsInLevel);

        if (CollectedCoins >= TotalCoinsInLevel)
        {
            WinGame();
        }
    }
    private void WinGame()
    {
        Debug.Log("All coins collected! You win!");
        SceneManager.LoadScene("WinScene");
    }

    public void Die()
    {
        SceneManager.LoadScene("LoseScene");
    }
}


public enum States
{
    idle,
    run,
    jump
}
