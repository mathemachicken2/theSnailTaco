using UnityEngine;
using System.Collections.Generic;

public class GrillStation : MonoBehaviour
{
    public Transform grillPoint;
    public GameObject cookedTacoPrefab; // assign in Inspector

    private List<GameObject> placedIngredients = new List<GameObject>();

    public void AddIngredient(GameObject ingredient)
    {
        placedIngredients.Add(ingredient);
    }

    public int GetIngredientCount()
    {
        return placedIngredients.Count;
    }

    public void FinishCooking()
    {
        // Destroy ingredients
        foreach (GameObject obj in placedIngredients)
        {
            if (obj != null)
                Destroy(obj);
        }

        placedIngredients.Clear();

        // Spawn cooked taco
        Instantiate(cookedTacoPrefab, grillPoint.position, grillPoint.rotation);
    }
}