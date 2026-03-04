using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public Customer[] customers;
    private int currentIndex = -1;

    public Customer CurrentCustomer { get; private set; }

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
            CurrentCustomer.LeaveCounter();
            Debug.Log("Customer served");
            SpawnNextCustomer();
        }
    }
}