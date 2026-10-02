using UnityEngine;

public class DeadZoneWater : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water") && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // 1. Dừng ngay lập tức chuyển động và lực hấp dẫn của nhân vật
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Dừng vận tốc rơi (Unity cũ dùng rb.velocity = Vector2.zero)
            rb.bodyType = RigidbodyType2D.Kinematic; // Khóa không cho lực hấp dẫn kéo rơi tiếp
        }

        // 2. Kích hoạt hiệu ứng Game Over
        if (GameManager.instance != null)
        {
            GameManager.instance.TriggerGameOverBackground();
        }

        // Tùy chọn 1: Nếu muốn nhân vật biến mất hoàn toàn ngay lập tức
         gameObject.SetActive(false); 

        // Tùy chọn 2: Vô hiệu hóa script điều khiển để người chơi không thể di chuyển tiếp
         this.enabled = false;
    }
}