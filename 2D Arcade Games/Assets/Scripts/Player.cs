using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float moveSpeed = 5f;
    [Tooltip("If true, player is clamped to the camera view (screen edges). If false, clamps to the Arena bounds.")]
    [SerializeField] public bool clampToCameraView = true;
    [Tooltip("How far inside the screen edge the player is allowed to move (world units). Increase if you don't want to touch the exact edge.")]
    [SerializeField] public float cameraEdgeMargin = 0.5f;

    [Header("Flashlight")]
    [SerializeField] public float flashlightRange = 6f;
    [SerializeField] public float flashlightAngle = 45f;
    [Tooltip("How long enemies stay frozen after being lit (seconds).")]
    [SerializeField] public float flashlightFreezeTime = 0.35f;

    [Header("Shooting")]
    [SerializeField] public float bulletSpeed = 15f;
    [SerializeField] public float missReloadTime = 3f;
    [SerializeField] public GameObject bulletPrefab;

    private Camera mainCam;
    private bool hasBullet = true;
    private float reloadTimer = 0f;
    private bool isReloading = false;

    public Vector2 FlashlightDirection { get; private set; }
    public float FlashlightFreezeTime => flashlightFreezeTime;
    public bool HasBullet => hasBullet;
    public bool IsReloading => isReloading;
    public float ReloadProgress => isReloading ? (1f - (reloadTimer / missReloadTime)) : 1f;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        // WASD Movement
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 move = new Vector2(h, v).normalized;
        Vector2 newPos = (Vector2)transform.position + move * moveSpeed * Time.deltaTime;

        // Clamp to camera view (screen edges) OR arena
        if (clampToCameraView)
            newPos = ClampToCameraView(newPos, cameraEdgeMargin);
        else if (Arena.Instance != null)
            newPos = Arena.Instance.ClampToArena(newPos);

        transform.position = newPos;

        // Flashlight aims at mouse
        Vector2 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        FlashlightDirection = (mouseWorld - (Vector2)transform.position).normalized;

        // Reload timer (only when reloading from a miss)
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                hasBullet = true;
                isReloading = false;
                reloadTimer = 0;
            }
        }

        // Shooting (left click OR spacebar)
        bool shootPressed = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);
        if (shootPressed && hasBullet && bulletPrefab != null)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        hasBullet = false;

        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Init(this, FlashlightDirection, bulletSpeed);
        }
    }

    /// <summary>
    /// Called by bullet when it hits an enemy - instant reload.
    /// </summary>
    public void OnBulletHit()
    {
        hasBullet = true;
        isReloading = false;
        reloadTimer = 0;
    }

    /// <summary>
    /// Called by bullet when it misses (hits wall) - slow reload.
    /// </summary>
    public void OnBulletMiss()
    {
        isReloading = true;
        reloadTimer = missReloadTime;
    }

    /// <summary>
    /// Check if a position is inside the flashlight cone.
    /// </summary>
    public bool IsInFlashlight(Vector2 worldPos)
    {
        Vector2 toTarget = worldPos - (Vector2)transform.position;
        float dist = toTarget.magnitude;

        if (dist > flashlightRange) return false;

        float angle = Vector2.Angle(FlashlightDirection, toTarget);
        return angle <= flashlightAngle * 0.5f;
    }

    // Draw flashlight cone in Scene view
    private void OnDrawGizmos()
    {
        Vector2 pos = transform.position;
        Vector2 dir = Application.isPlaying ? FlashlightDirection : Vector2.right;

        float halfAngle = flashlightAngle * 0.5f * Mathf.Deg2Rad;
        Vector2 edge1 = Rotate(dir, halfAngle) * flashlightRange;
        Vector2 edge2 = Rotate(dir, -halfAngle) * flashlightRange;

        Gizmos.color = new Color(1f, 1f, 0.3f, 0.5f);
        Gizmos.DrawLine(pos, pos + edge1);
        Gizmos.DrawLine(pos, pos + edge2);
        Gizmos.DrawLine(pos + edge1, pos + edge2);
    }

    private Vector2 Rotate(Vector2 v, float rad)
    {
        float c = Mathf.Cos(rad), s = Mathf.Sin(rad);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }

    private Vector2 ClampToCameraView(Vector2 pos, float margin)
    {
        if (mainCam == null || !mainCam.orthographic) return pos;

        Vector2 camPos = mainCam.transform.position;
        float halfH = mainCam.orthographicSize;
        float halfW = halfH * mainCam.aspect;

        float minX = camPos.x - halfW + margin;
        float maxX = camPos.x + halfW - margin;
        float minY = camPos.y - halfH + margin;
        float maxY = camPos.y + halfH - margin;

        return new Vector2(
            Mathf.Clamp(pos.x, minX, maxX),
            Mathf.Clamp(pos.y, minY, maxY)
        );
    }
}
