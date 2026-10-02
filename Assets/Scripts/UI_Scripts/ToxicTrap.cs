using UnityEngine;

public class Trap2 : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private int damage = 1;

    // Hàm này chạy khi Player chạm vào vùng Is Trigger của bẫy
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng chạm vào có tag là "Player" không
        if (collision.CompareTag("Player"))
        {
            // Lấy script Player và gọi hàm TakeDamage
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}