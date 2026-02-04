using UnityEngine;

public class Arena : MonoBehaviour
{
    [Header("Arena Size")]
    [SerializeField] public float width = 20f;
    [SerializeField] public float height = 15f;
    [SerializeField] public float borderThickness = 1f;

    [Header("Enemy Spawn Area")]
    [Tooltip("How far inside the arena edge enemies spawn. 0 = at the wall, 2 = 2 units in. Larger = smaller spawn band.")]
    [SerializeField] public float spawnInset = 0.5f;

    [Header("Auto-Generated (Don't Edit)")]
    [SerializeField] private BoxCollider2D topWall;
    [SerializeField] private BoxCollider2D bottomWall;
    [SerializeField] private BoxCollider2D leftWall;
    [SerializeField] private BoxCollider2D rightWall;

    public static Arena Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        CreateWalls();
        CreateFloor();
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        // Only update positions/sizes in editor, don't create/clean up objects
        UpdateWallPositions();
    }

    private void CreateWalls()
    {
        topWall = GetOrCreateWall("TopWall", true);
        bottomWall = GetOrCreateWall("BottomWall", true);
        leftWall = GetOrCreateWall("LeftWall", true);
        rightWall = GetOrCreateWall("RightWall", true);

        UpdateWallPositions();
    }

    private void UpdateWallPositions()
    {
        if (topWall == null || bottomWall == null || leftWall == null || rightWall == null)
            return;

        float halfW = width / 2f;
        float halfH = height / 2f;

        Vector2 topSize = new Vector2(width + borderThickness * 2, borderThickness);
        Vector2 sideSize = new Vector2(borderThickness, height + borderThickness * 2);

        topWall.transform.localPosition = new Vector3(0, halfH + borderThickness / 2f, 0);
        topWall.size = topSize;

        bottomWall.transform.localPosition = new Vector3(0, -halfH - borderThickness / 2f, 0);
        bottomWall.size = topSize;

        leftWall.transform.localPosition = new Vector3(-halfW - borderThickness / 2f, 0, 0);
        leftWall.size = sideSize;

        rightWall.transform.localPosition = new Vector3(halfW + borderThickness / 2f, 0, 0);
        rightWall.size = sideSize;
    }

    private BoxCollider2D GetOrCreateWall(string wallName, bool cleanupSprites = false)
    {
        Transform existing = transform.Find(wallName);
        if (existing != null)
        {
            // Remove any old sprite so no grey boxes (only when actually creating/cleaning up)
            if (cleanupSprites)
            {
                SpriteRenderer oldSr = existing.GetComponent<SpriteRenderer>();
                if (oldSr != null)
                {
                    Destroy(oldSr);
                }
            }
            return existing.GetComponent<BoxCollider2D>();
        }

        GameObject wall = new GameObject(wallName);
        wall.transform.SetParent(transform);
        wall.transform.localPosition = Vector3.zero;
        wall.tag = "Wall";

        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        return col;
    }

    private void CreateFloor()
    {
        Transform existing = transform.Find("Floor");
        if (existing != null) return;

        GameObject floor = new GameObject("Floor");
        floor.transform.SetParent(transform);
        floor.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = floor.AddComponent<SpriteRenderer>();
        sr.sprite = CreateWhiteSprite();
        sr.color = new Color(0.12f, 0.12f, 0.15f);
        sr.sortingOrder = -2;

        // Use same lit material as Player so flashlight lights the floor (no assignment needed)
        Player p = FindFirstObjectByType<Player>();
        if (p != null)
        {
            SpriteRenderer playerSr = p.GetComponent<SpriteRenderer>();
            if (playerSr != null && playerSr.material != null)
                sr.material = playerSr.material;
        }

        floor.transform.localScale = new Vector3(width, height, 1f);
    }

    private Sprite CreateWhiteSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
    }

    /// <summary>
    /// Get a random spawn position inside the arena (near edges)
    /// </summary>
    public Vector2 GetRandomEdgeSpawnPosition()
    {
        float halfW = width / 2f - spawnInset;
        float halfH = height / 2f - spawnInset;

        int side = Random.Range(0, 4);
        return side switch
        {
            0 => new Vector2(Random.Range(-halfW, halfW), halfH),  // Top
            1 => new Vector2(Random.Range(-halfW, halfW), -halfH), // Bottom
            2 => new Vector2(-halfW, Random.Range(-halfH, halfH)), // Left
            _ => new Vector2(halfW, Random.Range(-halfH, halfH))   // Right
        };
    }

    /// <summary>
    /// Check if a position is inside the arena
    /// </summary>
    public bool IsInsideArena(Vector2 pos)
    {
        float halfW = width / 2f;
        float halfH = height / 2f;
        return pos.x > -halfW && pos.x < halfW && pos.y > -halfH && pos.y < halfH;
    }

    /// <summary>
    /// Clamp a position to stay inside the arena
    /// </summary>
    public Vector2 ClampToArena(Vector2 pos, float margin = 0.5f)
    {
        float halfW = width / 2f - margin;
        float halfH = height / 2f - margin;
        return new Vector2(
            Mathf.Clamp(pos.x, -halfW, halfW),
            Mathf.Clamp(pos.y, -halfH, halfH)
        );
    }

    // Draw arena bounds and spawn band in Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));

        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position, new Vector3(width + borderThickness * 2, height + borderThickness * 2, 0));

        // Spawn area (where enemies appear)
        float spawnW = width - spawnInset * 2f;
        float spawnH = height - spawnInset * 2f;
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnW, spawnH, 0));
    }
}
