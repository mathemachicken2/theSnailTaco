using UnityEngine;

public class PlateStation : MonoBehaviour
{
    public Transform platePoint;

    public void ServeTaco(GameObject taco)
    {
        taco.transform.parent = null;
        taco.transform.position = platePoint.position;
        taco.transform.rotation = platePoint.rotation;

        Debug.Log("Taco served!");
    }
}