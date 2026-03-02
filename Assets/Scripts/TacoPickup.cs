using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TacoPickup : MonoBehaviour
{
    [Header("Hand")]
    public Transform handPoint;
    public Transform handModel;

    public Animator animator;

    [Header("Prefabs")]
    public GameObject tortillaPrefab;
    public GameObject beanPrefab;
    public GameObject cheesePrefab;
    public GameObject salsaPrefab;
    public GameObject picklePrefab;

    // private GameObject currentIngredient;
    private IngredientItem currentItem;

    private GameObject heldObject;
    private GrillStation currentGrill;

    private Quaternion handStartRotation;
    private Quaternion handModelStartRotation;

    private float cookTimer = 0f;
    private float cookDuration = 6f;
    private bool isCooking = false;

    public Image cookProgressBar;
    public Image cookProgressBackground;

    void Start()
    {
        handStartRotation = handPoint.localRotation;
        handModelStartRotation = handModel.localRotation;

        cookProgressBackground.gameObject.SetActive(false);
    }

    void Update()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;

        if (kb == null || mouse == null) return;

        // ---------- E KEY INTERACTIONS ----------
        if (kb.eKey.wasPressedThisFrame)
        {
            if (currentItem != null)
            {
                PickupItem();
            }
            else if (heldObject != null && currentGrill != null)
            {
                PlaceOnGrill();
            }
        }

        // ---------- COOKING SYSTEM ----------
        if (mouse.leftButton.isPressed && currentGrill != null && heldObject == null)
        {
            if (!isCooking)
            {
                StartCooking();
            }

            if (isCooking)
            {
                cookTimer += Time.deltaTime;

                cookProgressBar.fillAmount = cookTimer / cookDuration;

                if (cookTimer >= cookDuration)
                {
                    currentGrill.FinishCooking();
                    StopCooking();
                }
            }
        }
        else
        {
            if (isCooking)
            {
                StopCooking();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        IngredientItem item = other.GetComponent<IngredientItem>();

        if (item != null)
        {
            currentItem = item;

            if (heldObject != null)
            {

                PickupUIManager.Instance.Show(
                    "Replace with " + currentItem.itemName + " (E)"
                );
            }
            else
            {

                PickupUIManager.Instance.Show(
                    "Pick up " + currentItem.itemName + " (E)"
                );
            }
            return; // Stop here so grill doesn't overwrite UI
        }

        GrillStation grill = other.GetComponent<GrillStation>();

        if (grill != null)
        {
            currentGrill = grill;

            if (heldObject != null)
                PickupUIManager.Instance.Show("Place on Grill (E)");
            else
                PickupUIManager.Instance.Show("Cook (Mouse Click)");
        }
    }

    void OnTriggerExit(Collider other)
    {
        IngredientItem item = other.GetComponent<IngredientItem>();

        if (item != null && item == currentItem)
        {
            currentItem = null;
            PickupUIManager.Instance.Hide();
        }

        GrillStation grill = other.GetComponent<GrillStation>();

        if (grill != null && grill == currentGrill)
        {
            currentGrill = null;
            PickupUIManager.Instance.Hide();
        }
    }


    void PickupItem()
    {
        if (currentItem == null) return;

        GameObject prefabToSpawn = null;

        switch (currentItem.itemName)
        {
            case "tortilla": prefabToSpawn = tortillaPrefab; break;
            case "bean": prefabToSpawn = beanPrefab; break;
            case "cheese": prefabToSpawn = cheesePrefab; break;
            case "salsa": prefabToSpawn = salsaPrefab; break;
            case "pickles": prefabToSpawn = picklePrefab; break;
            default:
                Debug.LogWarning("No prefab assigned for " + currentItem.itemName);
                return;
        }

        if (prefabToSpawn != null)
        {
            // Destroy old held item
            if (heldObject != null)
                Destroy(heldObject);

            // Spawn new item in hand
            heldObject = Instantiate(prefabToSpawn, handPoint.position, handPoint.rotation, handPoint);

            // Trigger pick up animation (same for pickup & place)
            if (animator != null)
                animator.SetBool("PickUp",true);
            animator.SetBool("Idle", false);
            animator.SetBool("Cook", false);
            Debug.Log("Picked up " + currentItem.itemName);

            currentItem = null;
            PickupUIManager.Instance.Hide();

        }
        handModel.localRotation = handModelStartRotation * Quaternion.Euler(0, 0, 90);
    }

    void StartCooking()
    {
        if (currentGrill == null || heldObject != null)
            return;

        if (currentGrill.GetIngredientCount() < 3)
        {
            PickupUIManager.Instance.Show("Needs more ingredients!");
            return;
        }
        isCooking = true;
        cookTimer = 0f;

        animator.SetBool("Cook", true);
        animator.SetBool("Idle", false);
        animator.SetBool("PickUp", false);

        handModel.localRotation = handModelStartRotation * Quaternion.Euler(-12, 180, -60);

        cookProgressBackground.gameObject.SetActive(true);

        Debug.Log("Cooking...");
    }

    void StopCooking()
    {
        isCooking = false;
        cookTimer = 0f;

        animator.SetBool("Cook", false);
        animator.SetBool("Idle", true);

        handModel.localRotation = handModelStartRotation;

        cookProgressBar.fillAmount = 0f; // reset UI
        cookProgressBackground.gameObject.SetActive(false);

        Debug.Log("Cooking stopped");
    }

    void PlaceOnGrill()
    {
        if (heldObject == null || currentGrill == null)
            return;

        // Move item to grill
        heldObject.transform.parent = null;
        heldObject.transform.position = currentGrill.grillPoint.position;
        heldObject.transform.rotation = currentGrill.grillPoint.rotation;

        currentGrill.AddIngredient(heldObject);

        // Trigger same pick up animation
        if (animator != null)
            animator.SetBool("Idle",true);
        animator.SetBool("PickUp", false);
        handModel.localRotation = handModelStartRotation * Quaternion.Euler(0, 0, 90);

        heldObject = null;

        Debug.Log("Placed on grill");
    }
}
