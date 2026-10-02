using UnityEngine;

public class Trap3 : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 8f; // Điều chỉnh lực hất văng ở đây

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;

                // Đảm bảo hướng văng luôn có lực hất xéo lên trên một chút cho đẹp
                knockbackDir.y = Mathf.Abs(knockbackDir.y) + 0.5f;

                player.TakeDamage(damage, knockbackDir.normalized * knockbackForce);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                knockbackDir.y = Mathf.Abs(knockbackDir.y) + 0.5f;

                player.TakeDamage(damage, knockbackDir.normalized * knockbackForce);
            }
        }
    }
}