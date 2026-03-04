using UnityEngine;

public class CounterInteraction : MonoBehaviour
{
    public CustomerManager manager;
    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            Debug.Log("Player entered counter trigger");

            if (manager.CurrentCustomer != null)
            {
                Debug.Log("CurrentCustomer is NOT null");

                if (manager.CurrentCustomer.IsAtCounter())
                {
                    Debug.Log("Customer is at counter! Showing UI");
                    PickupUIManager.Instance.Show("Serve customer (E)");
                }
                else
                {
                    Debug.Log("Customer is NOT at counter");
                }
            }
            else
            {
                Debug.Log("CurrentCustomer is null");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            PickupUIManager.Instance.Hide();
        }
    }

    public bool CanInteract()
    {
        return playerInRange &&
               manager.CurrentCustomer != null &&
               manager.CurrentCustomer.IsAtCounter();
    }
}