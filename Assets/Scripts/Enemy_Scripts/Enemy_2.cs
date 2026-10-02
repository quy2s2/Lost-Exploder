using UnityEngine;

public class Enemy_2 : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Attack")]
    [SerializeField] private Transform attackPonint;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private LayerMask whatIsPlayer;

    [Header("Check Player")]
    [SerializeField] private Transform checkPoint;
    [SerializeField] private float size_X = 5f;
    [SerializeField] private float size_Y = 2.5f;

    [SerializeField] private float retrieveDistance = 2.5f;
    [SerializeField] private float chaseSpeed = 3.5f;

    [Header("Patrol")]
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float patrolSpeed = 2f;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;

    private bool isEnemyDied = false;
    private bool facingLeft = true;

    private float patrolStartX;
    private bool patrolRight = true;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("KHÔNG TÌM THẤY PLAYER! Hãy kiểm tra Tag = Player.");
        }

        patrolStartX = transform.position.x;

        facingLeft = true;
        isEnemyDied = false;
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (isEnemyDied)
        {
            return;
        }

        if (player == null)
        {
            animator.SetBool("Chase", false);
            animator.SetBool("Attack", false);
            return;
        }

        CheckPlayer();
    }


    // =========================================================
    // CHECK PLAYER
    // =========================================================

    private void CheckPlayer()
    {
        if (checkPoint == null)
        {
            Debug.LogError("ENEMY_2 CHƯA GÁN CHECK POINT!");
            return;
        }

        Collider2D collInfo = Physics2D.OverlapBox(
            checkPoint.position,
            new Vector2(size_X, size_Y),
            0f,
            whatIsPlayer
        );

        // =====================================================
        // ĐÃ PHÁT HIỆN PLAYER
        // =====================================================

        if (collInfo != null)
        {
            Vector2 targetPos = new Vector2(
                player.position.x,
                transform.position.y
            );

            // -----------------------------
            // QUAY MẶT VỀ PHÍA PLAYER
            // -----------------------------

            if (transform.position.x < player.position.x)
            {
                // Player ở bên phải
                if (facingLeft)
                {
                    transform.eulerAngles = new Vector3(0f, -180f, 0f);
                    facingLeft = false;
                }
            }
            else if (transform.position.x > player.position.x)
            {
                // Player ở bên trái
                if (!facingLeft)
                {
                    transform.eulerAngles = new Vector3(0f, 0f, 0f);
                    facingLeft = true;
                }
            }

            // -----------------------------
            // ĐUỔI PLAYER
            // -----------------------------

            float distance = Vector2.Distance(
                transform.position,
                player.position
            );

            if (distance > retrieveDistance)
            {
                animator.SetBool("Chase", true);
                animator.SetBool("Attack", false);

                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPos,
                    chaseSpeed * Time.deltaTime
                );
            }

            // -----------------------------
            // ĐỦ GẦN -> ATTACK
            // -----------------------------

            else
            {
                animator.SetBool("Chase", false);
                animator.SetBool("Attack", true);
            }
        }

        // =====================================================
        // KHÔNG THẤY PLAYER -> PATROL
        // =====================================================

        else
        {
            animator.SetBool("Chase", false);
            animator.SetBool("Attack", false);

            Patrol();
        }
    }


    // =========================================================
    // PATROL
    // =========================================================

    private void Patrol()
    {
        if (patrolDistance <= 0f || patrolSpeed <= 0f)
        {
            animator.SetBool("Chase", false);
            return;
        }

        animator.SetBool("Chase", true);

        float leftPoint = patrolStartX - patrolDistance;
        float rightPoint = patrolStartX + patrolDistance;


        // =====================================================
        // ĐI SANG PHẢI
        // =====================================================

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


        // =====================================================
        // ĐI SANG TRÁI
        // =====================================================

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


    // =========================================================
    // ENEMY ATTACK PLAYER
    // =========================================================

    public void Attack()
    {
        // Enemy đã chết thì không được đánh
        if (isEnemyDied)
        {
            return;
        }

        // Kiểm tra Attack Point
        if (attackPonint == null)
        {
            Debug.LogError(
                gameObject.name + " CHƯA GÁN ATTACK POINT!"
            );

            return;
        }

        // Tìm tất cả Collider của Player trong vùng đánh
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPonint.position,
            attackRadius,
            whatIsPlayer
        );

        // Không tìm thấy Player
        if (hits.Length == 0)
        {
            Debug.Log("ENEMY ATTACK -> KHÔNG TRÚNG PLAYER");
            return;
        }

        // Kiểm tra từng Collider
        foreach (Collider2D hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            // Tìm Player ở object hiện tại hoặc object cha
            Player playerScript = hit.GetComponent<Player>();

            if (playerScript == null)
            {
                playerScript = hit.GetComponentInParent<Player>();
            }

            // Nếu tìm thấy Player
            if (playerScript != null)
            {
                playerScript.TakeDamage(1);

                Debug.Log(
                    gameObject.name +
                    " ĐÁNH PLAYER -1 HP"
                );

                return;
            }
        }

        Debug.Log(
            "ENEMY ATTACK -> CÓ COLLIDER NHƯNG KHÔNG TÌM THẤY PLAYER"
        );
    }


    // =========================================================
    // ENEMY TAKE DAMAGE
    // =========================================================

    public void TakeDamage(int damage)
    {
        if (isEnemyDied)
        {
            return;
        }

        if (damage <= 0)
        {
            return;
        }

        maxHealth -= damage;

        Debug.Log(
            gameObject.name +
            " HP: " +
            maxHealth
        );

        // Enemy chết
        if (maxHealth <= 0)
        {
            Die();
            return;
        }

        // Enemy bị thương
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
    }


    // =========================================================
    // DIE
    // =========================================================

    private void Die()
    {
        if (isEnemyDied)
        {
            return;
        }

        isEnemyDied = true;

        Debug.Log(
            gameObject.name +
            " Died."
        );

        // Tắt AI
        if (animator != null)
        {
            animator.SetBool("Chase", false);
            animator.SetBool("Attack", false);

            animator.SetTrigger("Death");
        }

        // Tắt vật lý
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }

        // Tắt Collider
        if (boxCollider2D != null)
        {
            boxCollider2D.enabled = false;
        }

        // Xóa Enemy sau khi animation Death chạy
        Destroy(gameObject, 0.901f);
    }


    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmos()
    {
        // Vùng phát hiện Player
        if (checkPoint != null)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireCube(
                checkPoint.position,
                new Vector2(size_X, size_Y)
            );
        }

        // Vùng Attack
        if (attackPonint != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                attackPonint.position,
                attackRadius
            );
        }
    }
}