using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Miss conditions")]
    [Tooltip("If true, bullet counts as a miss when it leaves the camera view (recommended if you don't use arena walls).")]
    [SerializeField] private bool missWhenOffscreen = true;
    [Tooltip("How far outside the camera view the bullet can go before it counts as a miss (world units).")]
    [SerializeField] private float offscreenMargin = 1.0f;
    [Tooltip("Failsafe: bullet will count as a miss after this many seconds even if it never hits anything.")]
    [SerializeField] private float maxLifetimeSeconds = 4.0f;

    private Player player;
    private Vector2 direction;
    private float speed;
    private bool hasHit = false;
    private Camera cam;
    private float spawnTime;

    public void Init(Player owner, Vector2 dir, float spd)
    {
        player = owner;
        direction = dir.normalized;
        speed = spd;
    }

    private void Awake()
    {
        cam = Camera.main;
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (hasHit) return;

        // Move bullet
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Miss if it leaves camera view (useful when there are no wall colliders)
        if (missWhenOffscreen && IsOutsideCameraView((Vector2)transform.position, offscreenMargin))
        {
            Miss();
            return;
        }

        // Optional: miss if it leaves the arena bounds (if arena exists)
        if (Arena.Instance != null && !Arena.Instance.IsInsideArena(transform.position))
        {
            Miss();
            return;
        }

        // Failsafe lifetime
        if (maxLifetimeSeconds > 0f && Time.time - spawnTime >= maxLifetimeSeconds)
        {
            Miss();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // Check if hit enemy
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            Hit(enemy);
            return;
        }

        // Check if hit wall (by tag)
        if (other.CompareTag("Wall"))
        {
            Miss();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        // Check if hit enemy
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            Hit(enemy);
            return;
        }

        // Check if hit wall (by tag)
        if (collision.gameObject.CompareTag("Wall"))
        {
            Miss();
        }
    }

    private void Hit(Enemy enemy)
    {
        hasHit = true;
        enemy.Die();
        if (player != null) player.OnBulletHit();
        Destroy(gameObject);
    }

    private void Miss()
    {
        hasHit = true;
        if (player != null) player.OnBulletMiss();
        Destroy(gameObject);
    }

    private bool IsOutsideCameraView(Vector2 worldPos, float margin)
    {
        if (cam == null || !cam.orthographic) return false;

        Vector2 camPos = cam.transform.position;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float left = camPos.x - halfW - margin;
        float right = camPos.x + halfW + margin;
        float bottom = camPos.y - halfH - margin;
        float top = camPos.y + halfH + margin;

        return (worldPos.x < left || worldPos.x > right || worldPos.y < bottom || worldPos.y > top);
    }
}
