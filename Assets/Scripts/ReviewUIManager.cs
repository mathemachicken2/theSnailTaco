using UnityEngine;
using UnityEngine.InputSystem;

public class ReviewUIManager : MonoBehaviour
{
    public RectTransform reviewsPanel;
    public float slideSpeed = 5f;

    private bool isOpen = false;
    private Vector2 closedPos;
    private Vector2 openPos;
    public CustomerManager customerManager; // assign in inspector
   // private bool phoneOpen = false;

    void Start()
    {
        // Panel starts offscreen and inactive
        closedPos = reviewsPanel.anchoredPosition;
        openPos = new Vector2(closedPos.x, 0);

        reviewsPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.tabKey.wasPressedThisFrame)
        {
            if (!isOpen)
            {
                // Open panel
                reviewsPanel.gameObject.SetActive(true);
                isOpen = true;
            }
            else
            {
                // Close panel
                isOpen = false;

                // Spawn the next customer when the panel is closed
                if (customerManager != null)
                {
                    customerManager.SpawnNextCustomer();
                }
            }
        }

        // Slide panel
        if (reviewsPanel.gameObject.activeSelf)
        {
            Vector2 target = isOpen ? openPos : closedPos;
            reviewsPanel.anchoredPosition = Vector2.Lerp(
                reviewsPanel.anchoredPosition,
                target,
                Time.deltaTime * slideSpeed
            );

            // Fully hidden? Deactivate
            if (!isOpen && Vector2.Distance(reviewsPanel.anchoredPosition, closedPos) < 0.1f)
            {
                reviewsPanel.gameObject.SetActive(false);
            }
        }
    }
}