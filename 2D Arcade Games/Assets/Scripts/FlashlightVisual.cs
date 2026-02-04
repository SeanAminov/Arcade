using UnityEngine;

/// <summary>
/// Attach to the Spot Light 2D child of the Player.
/// Makes the light rotate to follow the mouse (same as flashlight detection).
/// </summary>
public class FlashlightVisual : MonoBehaviour
{
    private Camera mainCam;
    private Transform player;

    private void Awake()
    {
        mainCam = Camera.main;
        player = transform.parent; // Assumes this is a child of Player
    }

    private void Update()
    {
        if (player == null || mainCam == null) return;

        Vector2 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorld - (Vector2)player.position;

        if (dir.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
