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

    private GameObject currentIngredient;



    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.eKey.wasPressedThisFrame && currentIngredient != null)
        {
            PickupItem();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        currentIngredient = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentIngredient)
            currentIngredient = null;
    }

    void PickupItem()
    {
        GameObject prefabToSpawn = null;

        string name = currentIngredient.name.ToLower();

        if (name.Contains("tortilla"))
            prefabToSpawn = tortillaPrefab;

        else if (name.Contains("bean"))
            prefabToSpawn = beanPrefab;

        else if (name.Contains("cheese"))
            prefabToSpawn = cheesePrefab;

        else if (name.Contains("salsa"))
            prefabToSpawn = salsaPrefab;

        else if (name.Contains("pickle"))
            prefabToSpawn = picklePrefab;

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, handPoint.position, handPoint.rotation, handPoint);
            Debug.Log($"Picked up {currentIngredient.name}");
        }
    }
}