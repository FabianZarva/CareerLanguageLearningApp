using UnityEngine;

public class RaycastTester : MonoBehaviour
{
    public Camera GameplayCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GameplayCamera == null)
            {
                Debug.LogError("GameplayCamera not assigned!");
                return;
            }

            Vector2 mouseWorldPos = GameplayCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            Debug.Log("Clicked at world position: " + mouseWorldPos);

            if (hit.collider != null)
            {
                Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("Raycast hit nothing.");
            }
        }
    }
}

