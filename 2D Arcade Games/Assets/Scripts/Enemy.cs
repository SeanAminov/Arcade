using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField] public float chaseSpeed = 3f;

    private Player player;
    private Rigidbody2D rb;
    private float frozenTimer = 0f;

    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (player == null) return;

        // Check if player's flashlight is on us
        bool isLit = player.IsInFlashlight(transform.position);

        if (isLit)
        {
            frozenTimer = player.FlashlightFreezeTime;
        }

        if (frozenTimer > 0f)
        {
            frozenTimer -= Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Only move if NOT frozen
        Vector2 dir = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * (chaseSpeed * DifficultyRuntime.EnemySpeedMultiplier);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        Player p = collision.gameObject.GetComponent<Player>();
        if (p != null)
        {
            // Player touched enemy - death!
            if (GameManager.Instance != null)
                GameManager.Instance.PlayerDied();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        Player p = other.GetComponent<Player>();
        if (p != null)
        {
            // Player touched enemy - death!
            if (GameManager.Instance != null)
                GameManager.Instance.PlayerDied();
        }
    }

    public void Die()
    {
        // Add score before destroying
        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(1);

        Destroy(gameObject);
    }
}
