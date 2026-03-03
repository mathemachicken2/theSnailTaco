using UnityEngine;
using System.Collections.Generic;

public class GrillStation : MonoBehaviour
{
    public Transform grillPoint;
    public GameObject cookedTacoPrefab;

    private List<GameObject> placedIngredients = new List<GameObject>();
    private GameObject cookedTacoInstance;

    public void AddIngredient(GameObject ingredient)
    {
        placedIngredients.Add(ingredient);
    }

    public int GetIngredientCount()
    {
        return placedIngredients.Count;
    }

    public bool HasCookedTaco()
    {
        return cookedTacoInstance != null;
    }

    public GameObject GetCookedTaco()
    {
        return cookedTacoInstance;
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

        // Spawn taco
        cookedTacoInstance = Instantiate(
            cookedTacoPrefab,
            grillPoint.position,
            grillPoint.rotation
        );
    }

    public void ClearCookedTaco()
    {
        cookedTacoInstance = null;
    }
}