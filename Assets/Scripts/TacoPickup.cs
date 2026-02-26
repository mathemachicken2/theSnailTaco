using UnityEngine;
using UnityEngine.InputSystem;

public class TacoPickup : MonoBehaviour
{
    [Header("Hand")]
    public Transform handPoint;

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



    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

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
        }

        GrillStation grill = other.GetComponent<GrillStation>();

        if (grill != null && heldObject != null)
        {
            currentGrill = grill;
            PickupUIManager.Instance.Show("Place on Grill (E)");
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

    void PlaceOnGrill()
    {
        if (heldObject == null || currentGrill == null)
            return;

        heldObject.transform.parent = null;

        heldObject.transform.position = currentGrill.grillPoint.position;
        heldObject.transform.rotation = currentGrill.grillPoint.rotation;

        heldObject = null;

        Debug.Log("Placed on grill");
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
            
            if (heldObject != null)
                Destroy(heldObject);

            
            heldObject = Instantiate(prefabToSpawn, handPoint.position, handPoint.rotation, handPoint);

            
            currentItem = null;

           
            PickupUIManager.Instance.Hide();
        }
    }
}