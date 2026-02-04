using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Player player;
    private Vector2 direction;
    private float speed;
    private bool hasHit = false;

    public void Init(Player owner, Vector2 dir, float spd)
    {
        player = owner;
        direction = dir.normalized;
        speed = spd;
    }

    private void Update()
    {
        if (hasHit) return;

        // Move bullet
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
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
}
