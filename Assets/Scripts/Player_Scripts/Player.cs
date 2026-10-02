using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections; // Cần dùng cho Coroutine

public class Player : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Movement")]
    public float speed = 7f;
    public float runSpeed = 12f;

    public float jumpHeight = 8f;
    private float movement;

    public Slider healthSlider;
    private Rigidbody2D rb;
    private Animator animator;
    [HideInInspector] public bool isGround;

    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;

    public int jump_Count = 2;
    private int totall_Jumps;
    private bool facing_Right;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask whatIsEnemy;

    public GameObject explosionPrefab;
    public Transform explosionSpawPoint;

    private bool isDead = false;

    // --- BIẾN MỚI CHO HỆ THỐNG KNOCKBACK ---
    private bool isKnockback = false;
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration = 0.2f; // Thời gian bị khống chế khi bị hất văng

    void Start()
    {
        totall_Jumps = jump_Count;
        movement = 0f;
        isGround = true;
        facing_Right = true;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    void Update()
    {
        if (isDead)
            return;

        // Nếu đang bị hất văng thì bỏ qua các Input di chuyển/nhảy/đánh
        if (isKnockback)
            return;

        movement = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        Collider2D collInfo = Physics2D.OverlapCircle(
            groundCheckPoint.position,
            groundCheckRadius,
            whatIsGround
        );

        isGround = collInfo != null;

        Flip();

        if (Mathf.Abs(movement) > 0.1f)
        {
            animator.SetFloat("Run", 1f);
        }
        else
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
        if (isDead)
            return;

        // Bỏ qua di chuyển nếu đang trong trạng thái bị hất văng
        if (isKnockback)
            return;

        float currentSpeed = Input.GetKey(KeyCode.LeftAlt) ? runSpeed : speed;

        transform.position += new Vector3(
            movement,
            0f,
            0f
        ) * Time.fixedDeltaTime * currentSpeed;
    }

    void Flip()
    {
        if (movement < 0f && facing_Right)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facing_Right = false;
        }
        else if (movement > 0f && !facing_Right)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facing_Right = true;
        }
    }

    // =========================================================
    // PLAYER NHẬN DAMAGE & HẤT VĂNG
    // =========================================================

    // Hàm nhận damage hỗ trợ Lực Hất Văng
    public void TakeDamage(int damageAmount, Vector2 knockbackForce)
    {
        if (isDead)
            return;

        maxHealth -= damageAmount;

        Debug.Log("PLAYER HP: " + maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = maxHealth;
        }

        if (maxHealth <= 0)
        {
            Die();
            return;
        }

        animator.SetTrigger("Hurt");

        // Xử lý hất văng nếu có lực
        if (knockbackForce != Vector2.zero)
        {
            StartCoroutine(ApplyKnockback(knockbackForce));
        }
    }

    // Giữ nguyên hàm cũ để tương thích với các script khác chưa truyền lực hất
    public void TakeDamage(int damageAmount)
    {
        TakeDamage(damageAmount, Vector2.zero);
    }

    private IEnumerator ApplyKnockback(Vector2 knockbackForce)
    {
        isKnockback = true;

        // Áp dụng lực trực tiếp vào Rigidbody2D
        rb.linearVelocity = Vector2.zero; // Triệt tiêu vận tốc cũ
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);

        // Chờ thời gian Knockback kết thúc
        yield return new WaitForSeconds(knockbackDuration);

        isKnockback = false;
    }

    // =========================================================
    // PLAYER ATTACK ANIMATION
    // =========================================================

    void PlayAttackAnimations()
    {
        int attack_Index = Random.Range(0, 3);

        if (attack_Index == 0)
        {
            animator.SetTrigger("Attack_1");
            FindAnyObjectByType<SoundManager>()?.PlayAttackSound();
        }
        else if (attack_Index == 1)
        {
            animator.SetTrigger("Attack_2");
        }
        else
        {
            animator.SetTrigger("Attack_3");
            FindAnyObjectByType<SoundManager>()?.PlayAttackSound();
        }
    }

    // =========================================================
    // PLAYER GÂY DAMAGE ENEMY
    // =========================================================

    public void Attack()
    {
        if (isDead)
            return;

        if (attackPoint == null)
        {
            Debug.LogError("PLAYER CHƯA GÁN ATTACK POINT!");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            whatIsEnemy
        );

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(1);
                Debug.Log("PLAYER ĐÁNH ENEMY -1 HP");
                return;
            }
        }

        Debug.Log("PLAYER ATTACK - KHÔNG TRÚNG ENEMY");
    }

    // =========================================================
    // JUMP
    // =========================================================

    void Jump()
    {
        if (totall_Jumps > 0)
        {
            animator.SetBool("Jump", true);
            FindAnyObjectByType<SoundManager>()?.PlayJumpSound();

            rb.linearVelocityY = jumpHeight;

            if (isGround)
            {
                isGround = false;
            }

            totall_Jumps--;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            totall_Jumps = jump_Count;
            animator.SetBool("Jump", false);
        }

        
    }

    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(
                groundCheckPoint.position,
                groundCheckRadius
            );
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                attackPoint.position,
                attackRadius
            );
        }
    }

    // =========================================================
    // DIE
    // =========================================================

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(gameObject.name + " Died");

        GameManager.instance.TriggerGameOverBackground();

        if (explosionPrefab != null && explosionSpawPoint != null)
        {
            GameObject tempExplotion = Instantiate(
                explosionPrefab,
                explosionSpawPoint.position,
                Quaternion.identity
            );

            Destroy(tempExplotion, 0.901f);
        }

        Destroy(gameObject);
    }
}