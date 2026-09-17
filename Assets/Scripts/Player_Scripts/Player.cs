using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed = 7f;
    public float jumpHeight = 8f;
    private float movement;
    private Rigidbody2D rb;
    private Animator animator;
    [HideInInspector] public bool isGround;

    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;

    public int jump_Count = 2;
    private int totall_Jumps;
    private bool facing_Right;
    void Start()
    {
        movement = 0f;
        isGround = true;
        facing_Right = true;
        totall_Jumps = jump_Count;
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        animator = this.gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        movement = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        Collider2D collInfo =  Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);

        if (collInfo == true)
        {
            isGround = true;
        }
        else {
            isGround = false;
        }
        Flip();

        if (Mathf.Abs(movement) > 0.1f)
        {
            animator.SetFloat("Run", 1f);
        }
        else if (movement < 0.1f)
        {
            animator.SetFloat("Run", 0f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlayAttackAnimations();
        }
        
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * speed;
    }

    void Flip()
    {
        if(movement < 0f && facing_Right == true)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            facing_Right = false;
        }
        else if (movement > 0f && facing_Right == false )
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            facing_Right = true;
        }
    }

    void PlayAttackAnimations()
    {
        int attack_Index = Random.Range(0, 3);
        if (attack_Index == 0)
        {
            animator.SetTrigger("Attack_1");
        }
        else if (attack_Index == 1)
        {
            animator.SetTrigger("Attack_2");
        }
        else
        {
            animator.SetTrigger("Attack_3");
        }
    }

    void Jump()
    {

        if (totall_Jumps > 0)
        {

            animator.SetBool("Jump", true);
    
        rb.linearVelocityY = jumpHeight;
            if (isGround == true)
            {
                isGround = false;
            }
            totall_Jumps -= 1;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Ground")
        {
            totall_Jumps = jump_Count;
            animator.SetBool("Jump", false);
        }

    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
        
    }
}