using UnityEngine;
using UnityEngine.UI;

public class ReloadUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Image progressFill;
    [SerializeField] private GameObject reloadBarContainer;

    [Header("Position")]
    [SerializeField] private Vector2 offsetFromPlayer = new Vector2(0, -1f);

    [Header("Colors")]
    [SerializeField] private Color reloadingColor = new Color(1f, 0.5f, 0f); // Orange
    [SerializeField] private Color readyColor = Color.green;

    private Camera mainCam;
    private RectTransform canvasRect;

    private void Awake()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();

        mainCam = Camera.main;

        // Get the canvas RectTransform
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            canvasRect = canvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (player == null || progressFill == null) return;

        // Only show when reloading (missed shot)
        bool shouldShow = player.IsReloading;

        if (reloadBarContainer != null)
            reloadBarContainer.SetActive(shouldShow);

        if (!shouldShow) return;

        // Follow player
        if (reloadBarContainer != null && mainCam != null)
        {
            Vector2 playerScreenPos = mainCam.WorldToScreenPoint(player.transform.position);
            Vector2 offset = offsetFromPlayer * 50f; // Scale offset for screen space
            reloadBarContainer.GetComponent<RectTransform>().position = playerScreenPos + offset;
        }

        // Update fill amount
        progressFill.fillAmount = player.ReloadProgress;
        progressFill.color = reloadingColor;
    }
}
