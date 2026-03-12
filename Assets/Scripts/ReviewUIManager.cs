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
    public GameObject[] reviewImages;
    private int currentReviewIndex = 0;
    private int reviewsUnlocked = 0;

    void Start()
    {
        // Panel starts offscreen and inactive
        closedPos = reviewsPanel.anchoredPosition;
        openPos = new Vector2(closedPos.x, 0);

        reviewsPanel.gameObject.SetActive(false);
        foreach (GameObject img in reviewImages)
        {
            img.SetActive(false);
        }
    }

    public void AddReview()
    {
        reviewsUnlocked++;
    }
    void ShowCurrentReview()
    {
        foreach (GameObject img in reviewImages)
            img.SetActive(false);

        int index = reviewsUnlocked - 1;

        if (index >= 0 && index < reviewImages.Length)
            reviewImages[index].SetActive(true);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.tabKey.wasPressedThisFrame)
        {
            if (!isOpen)
            {
                if (reviewsUnlocked == 0)
                    return;

                reviewsPanel.gameObject.SetActive(true);
                isOpen = true;
                ShowCurrentReview();
            }
            else
            {
                isOpen = false;

                if (customerManager != null && customerManager.customerServed)
                {
                    customerManager.SpawnNextCustomer();
                    customerManager.customerServed = false;
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