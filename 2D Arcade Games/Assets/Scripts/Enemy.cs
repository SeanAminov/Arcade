using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField] public float chaseSpeed = 3f;

    [Header("References")]
    [SerializeField] private Player player;

    private void Awake()
    {
        // Auto-find player if not assigned
        if (player == null)
            player = FindFirstObjectByType<Player>();
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
            transform.position += (Vector3)(dir * chaseSpeed * Time.deltaTime);
        }
    }
}
