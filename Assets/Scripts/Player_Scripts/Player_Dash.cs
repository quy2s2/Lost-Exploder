using UnityEngine;
using System.Collections;

public class Player_Dash : MonoBehaviour
{
    public float dash_Force = 10f;
    public float dash_Duration = .35f;

    private bool facingRight;
    public Rigidbody2D rb;
    private Animator animator;

    private bool isDashing;
    private bool hasDashed;
    private float direction;
    private Player player;

    void Start()
    {
        isDashing = false;
        hasDashed = false;
        facingRight = true;

        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        if (move > 0f)
        {
            facingRight = true;
        }
        else if (move < 0f)
        {
            facingRight = false;
        }

        // Chạm đất thì reset Dash
        if (player.isGround)
        {
            hasDashed = false;
            animator.SetBool("Dash", false);
            
        }

        // Chỉ Dash 1 lần khi đang trên không
        if (Input.GetKeyDown(KeyCode.LeftShift) &&
            isDashing == false &&
            hasDashed == false &&
            player.isGround == false)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        hasDashed = true;

        animator.SetBool("Dash", true);
        FindAnyObjectByType<SoundManager>().PlayDashSound();
        if (facingRight)
        {
            direction = 1f;
        }
        else
        {
            direction = -1f;
        }

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(dash_Force * direction, 0f);

        yield return new WaitForSeconds(dash_Duration);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 3f;
        isDashing = false;
    }
}