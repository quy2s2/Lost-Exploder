using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Attack")]
    [SerializeField] private Transform attackPonint;
    [SerializeField] private float attackRadius = 1f;

    [Header("Check Player")]
    [SerializeField] private Transform checkPoint;
    [SerializeField] private float size_X = 5f;
    [SerializeField] private float size_Y = 2.5f;
    [SerializeField] private LayerMask whatIsPlayer;

    [SerializeField] private float retrieveDistance = 2.5f;
    [SerializeField] private float chaseSpeed = 3.5f;

    [Header("Patrol")]
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float patrolSpeed = 2f;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;

    private bool isEnemyDied;
    private bool facingLeft;

    private float patrolStartX;
    private bool patrolRight = true;

    private void Start()
    {
        facingLeft = true;
        isEnemyDied = false;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        patrolStartX = transform.position.x;
    }

    void Update()
    {
        if (player == null)
        {
            animator.SetBool("Attack", false);
            return;
        }

        if (isEnemyDied)
        {
            return;
        }

        CheckPlayer();
    }

    void CheckPlayer()
    {
        Collider2D collInfo = Physics2D.OverlapBox(
            checkPoint.position,
            new Vector2(size_X, size_Y),
            0f,
            whatIsPlayer
        );

        if (collInfo)
        {
            Vector2 targetPos = new Vector2(
                player.position.x,
                transform.position.y
            );

            if (transform.position.x < player.position.x && facingLeft)
            {
                transform.eulerAngles = new Vector3(0f, -180f, 0f);
                facingLeft = false;
            }
            else if (transform.position.x > player.position.x && !facingLeft)
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                facingLeft = true;
            }

            if (Vector2.Distance(transform.position, player.position) > retrieveDistance)
            {
                animator.SetBool("Chase", true);
                animator.SetBool("Attack", false);

                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPos,
                    chaseSpeed * Time.deltaTime
                );
            }
            else
            {
                animator.SetBool("Chase", false);
                animator.SetBool("Attack", true);
            }
        }
        else
        {
            animator.SetBool("Chase", false);
            animator.SetBool("Attack", false);

            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolDistance <= 0f || patrolSpeed <= 0f)
        {
            animator.SetBool("Chase", false);
            return;
        }

        animator.SetBool("Chase", true);

        float leftPoint = patrolStartX - patrolDistance;
        float rightPoint = patrolStartX + patrolDistance;

        if (patrolRight)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(rightPoint, transform.position.y),
                patrolSpeed * Time.deltaTime
            );

            if (transform.position.x >= rightPoint - 0.05f)
            {
                patrolRight = false;
            }

            if (facingLeft)
            {
                transform.eulerAngles = new Vector3(0f, -180f, 0f);
                facingLeft = false;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(leftPoint, transform.position.y),
                patrolSpeed * Time.deltaTime
            );

            if (transform.position.x <= leftPoint + 0.05f)
            {
                patrolRight = true;
            }

            if (!facingLeft)
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                facingLeft = true;
            }
        }
    }

    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(
            attackPonint.position,
            attackRadius,
            whatIsPlayer
        );

        if (collInfo)
        {
            if (collInfo.gameObject.GetComponent<Player>() != null)
            {
                collInfo.gameObject.GetComponent<Player>().TakeDamage(1);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        maxHealth -= damage;

        if (maxHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    private void OnDrawGizmos()
    {
        if (checkPoint != null)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireCube(
                checkPoint.position,
                new Vector2(size_X, size_Y)
            );
        }

        if (attackPonint != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                attackPonint.position,
                attackRadius
            );
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " Died.");

        isEnemyDied = true;

        animator.SetTrigger("Death");

        rb.gravityScale = 0f;
        boxCollider2D.enabled = false;

        Destroy(gameObject, 3f);
    }
}