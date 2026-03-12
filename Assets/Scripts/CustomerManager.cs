using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public Customer[] customers;
    private int currentIndex = -1;

    public Customer CurrentCustomer { get; private set; }
    public bool customerServed = false;


    void Start()
    {
        SpawnNextCustomer();
    }


    public void SpawnNextCustomer()
    {
        currentIndex++;

        if (currentIndex >= customers.Length)
        {
            CurrentCustomer = null;
            return;
        }

        CurrentCustomer = customers[currentIndex];
        CurrentCustomer.MoveToCounter();
    }

    public void CustomerServed()
    {
        if (CurrentCustomer != null)
        {
            customerServed = true;
            Debug.Log("Customer served");
        }
    }
}