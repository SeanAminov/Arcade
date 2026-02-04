using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField] public float chaseSpeed = 3f;

    private Player player;
    private Rigidbody2D rb;

    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (player == null) return;

        // Check if player's flashlight is on us
        bool isLit = player.IsInFlashlight(transform.position);

        // Only move if NOT lit
        if (!isLit)
        {
            Vector2 dir = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * chaseSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
