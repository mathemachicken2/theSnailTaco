using UnityEngine;
using TMPro;

public class PickupUIManager : MonoBehaviour
{
    public static PickupUIManager Instance;

    public TextMeshProUGUI pickupText;

    void Awake()
    {
        Instance = this;
        pickupText.gameObject.SetActive(false);
    }

    // Generic function to show any text
    public void Show(string text)
    {
        pickupText.text = text;
        pickupText.gameObject.SetActive(true);
    }

    public void Hide()
    {
        pickupText.gameObject.SetActive(false);
    }
}