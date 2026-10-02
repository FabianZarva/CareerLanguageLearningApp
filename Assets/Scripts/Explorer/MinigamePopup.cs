using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinigamePopup : MonoBehaviour
{
    public string MinigameSceneName = "MinigameScene";
    public Camera GameplayCamera;      // The camera rendering to RenderTexture
    public RawImage RawImageUI;        // The UI RawImage showing the RenderTexture

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GameplayCamera == null)
            {
                Debug.LogError("GameplayCamera not assigned!");
                return;
            }

            if (RawImageUI == null)
            {
                Debug.LogError("RawImageUI not assigned!");
                return;
            }

            Vector2 localPoint;
            RectTransform rawImageRect = RawImageUI.rectTransform;

            // Convert screen point to local point in RawImage rect
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rawImageRect,
                Input.mousePosition,
                null,      // Assuming overlay canvas; use your event camera if needed
                out localPoint))
            {
                return;
            }

            // Normalize local point to UV coords (0 to 1)
            Vector2 uv = new Vector2(
                (localPoint.x + rawImageRect.rect.width * 0.5f) / rawImageRect.rect.width,
                (localPoint.y + rawImageRect.rect.height * 0.5f) / rawImageRect.rect.height
            );

            // Check if click is inside RawImage bounds (UV between 0 and 1)
            if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
                return;

            // Get RenderTexture from RawImage texture
            RenderTexture rt = RawImageUI.texture as RenderTexture;
            if (rt == null)
            {
                Debug.LogError("RawImage texture is not a RenderTexture!");
                return;
            }

            // Convert UV to RenderTexture pixel coords
            Vector3 rtPoint = new Vector3(uv.x * rt.width, uv.y * rt.height, GameplayCamera.nearClipPlane);

            // Convert RenderTexture pixel coords to world point via camera
            Vector3 worldPoint = GameplayCamera.ScreenToWorldPoint(rtPoint);

            // Raycast at this world position to detect your popup
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // ── AUDIO ──
                AudioManager.Instance?.PlaySFX("UIClick");

                if (GameManager.Instance.DialogueEnabled)
                {
                    SceneManager.LoadScene(MinigameSceneName + " Dialogue");
                }
                else
                {
                    Debug.Log("Popup clicked. Loading minigame...");
                    SceneManager.LoadScene(MinigameSceneName + " Intro");
                }
            }
        }
    }
}