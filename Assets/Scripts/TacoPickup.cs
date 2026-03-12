using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class TacoPickup : MonoBehaviour
{
    [Header("Hand")]
    public Transform handPoint;
    public Transform handModel;
    public Animator animator;

    [Header("Ingredient Prefabs")]
    public GameObject tortillaPrefab;
    public GameObject beanPrefab;
    public GameObject cheesePrefab;
    public GameObject salsaPrefab;
    public GameObject picklePrefab;

    [Header("UI")]
    public Image cookProgressBar;
    public GameObject cookProgressBackground;

    private IngredientItem currentItem;
    private GameObject heldObject;
    private GrillStation currentGrill;

    private Quaternion handModelStartRotation;

    private float cookTimer;
    private const float cookDuration = 6f;
    private bool isCooking;

    private PlateStation currentPlate;

    public CounterInteraction counter;

    public GameObject customerPanel;
    public TMP_Text dialogueText;

    public MonoBehaviour playerMovement;
    private bool isInDialogue = false;

    public GameObject buttonPrefab;
    public Transform buttonContainer;


    [SerializeField] Transform dialogueCameraTarget;
    [SerializeField] float cameraMoveSpeed = 5f;

    Camera playerCamera;
    bool isZooming = false;

    [SerializeField] Transform plateCameraTarget;

    [SerializeField] GameObject tacoHalfPrefab;
    [SerializeField] GameObject tacoAlmostGonePrefab;
    [SerializeField] Transform grillPoint;

    [SerializeField] float eatDuration = 5f;

    [SerializeField] Image fadePanel;
    [SerializeField] float fadeSpeed = 2f;
    [SerializeField] Transform customerZoomPoint;

    //Camerafix
   Vector3 originalLocalPosition;
    Quaternion originalLocalRotation;

    [SerializeField] GameObject bloodPrefab;
    [SerializeField] GameObject bloodPrefab1;
    [SerializeField] Transform bloodSpawnPoint;
    [SerializeField] Image sirenOverlay;
    [SerializeField] float sirenDuration = 4f;

    [SerializeField] private ReviewUIManager reviewUIManager;




    void Start()
    {
        handModelStartRotation = handModel.localRotation;
        cookProgressBackground.SetActive(false);
        customerPanel.SetActive(false);
        playerCamera = Camera.main;


        originalLocalPosition = playerCamera.transform.localPosition;
        originalLocalRotation = playerCamera.transform.localRotation;
    }

    void Update()
    {

        if (isZooming && dialogueCameraTarget != null)
        {
            playerCamera.transform.position = Vector3.Lerp(
                playerCamera.transform.position,
                dialogueCameraTarget.position,
                Time.deltaTime * cameraMoveSpeed
            );

            playerCamera.transform.rotation = Quaternion.Lerp(
                playerCamera.transform.rotation,
                dialogueCameraTarget.rotation,
                Time.deltaTime * cameraMoveSpeed
            );
        }

        var kb = Keyboard.current;
        var mouse = Mouse.current;

        if (kb == null || mouse == null) return;

        HandleEKey(kb);
        HandleCooking(mouse);

        UpdateInteractionUI();

        if (isInDialogue)
        {
            if (kb.escapeKey.wasPressedThisFrame)
            {
                CloseDialogue();
            }

            return; // stop gameplay logic
        }
        if (isInDialogue)
            return;
    }

    // =========================
    // INPUT
    // =========================

    void CloseDialogue()
    {
        isZooming = false;
        customerPanel.SetActive(false);
        for (int i = 0; i < buttonContainer.childCount; i++)
        {
            Destroy(buttonContainer.GetChild(i).gameObject);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerMovement != null)
            playerMovement.enabled = true;

        isInDialogue = false;

        playerCamera.transform.localPosition = originalLocalPosition;
        playerCamera.transform.localRotation = originalLocalRotation;
    }

    IEnumerator FadeToBlackWHenFired(float duration)
    {
        float timer = 0;

        Color c = fadePanel.color;

        while (timer < 1)
        {
            timer += Time.deltaTime * fadeSpeed;
            c.a = timer;
            fadePanel.color = c;

            yield return null;
        }
       
        yield return new WaitForSeconds(duration);
        DeathTextManager.Instance.ShowArrestedMessages();
    }

    IEnumerator FadeToBlackWhenEating(float duration)
    {
        float timer = 0;

        Color c = fadePanel.color;
        Customer currentCustomer = counter.manager.CurrentCustomer;

        while (timer < 1)
        {
            timer += Time.deltaTime * fadeSpeed;
            c.a = timer;
            fadePanel.color = c;
            DeathTextManager.Instance.ShowEatingMessage(currentCustomer.eatingMessage);

            yield return null;
        }

        yield return new WaitForSeconds(duration);
        
    }

    void OpenCustomerPanel()
    {
        dialogueCameraTarget = customerZoomPoint;
        isZooming = true;

        if (counter == null || counter.manager.CurrentCustomer == null)
            return;

        Customer currentCustomer = counter.manager.CurrentCustomer;

       
        

        

        // Freeze player
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Unlock mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        customerPanel.SetActive(true);
        ShowDialogue(currentCustomer);
        PickupUIManager.Instance.Hide();

        isInDialogue = true;
      

        Debug.Log("Opened customer order UI");
    }

    void KillPlayer()
    {
        // Spawn blood at player's position
        Instantiate(bloodPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        
    }
    void ShowDialogue(Customer customer)
    {
        DialogueEntry entry = customer.GetCurrentDialogue();
        if (entry == null) return;

        dialogueText.text = entry.text;

        // Clear old buttons
        // Create new buttons
        for (int i = 0; i < entry.options.Length; i++)
        {
            int index = i; // important for button callbacks

            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer.transform);
            buttonObj.transform.SetAsFirstSibling();
            float startY = 0f;
            float spacing = 30f;
            buttonObj.transform.localPosition = new Vector3(0, startY - spacing * i, 0);
            Debug.Log("Created button for option: " + entry.options[index]);

            TMP_Text btnText = buttonObj.GetComponentInChildren<TMP_Text>();
            btnText.text = entry.options[index];

            Button btn = buttonObj.GetComponent<Button>();

            btn.onClick.AddListener(() =>
            {
                Debug.Log("Button clicked! Index: " + index);
                dialogueText.text = entry.responses[index];
                if (entry.options[index] == "Kill customer")
                {
                    StartCoroutine(KillCustomerSequence());
                    KillPlayer();
                    //DeathTextManager.Instance.ShowArrestedMessages();
                }

                foreach (Transform child in buttonContainer.transform)
                {
                    Destroy(child.gameObject);
                }


            });

        }
    }

    IEnumerator KillCustomerSequence()
    {
        // Spawn blood
        Instantiate(bloodPrefab, bloodSpawnPoint.position, Quaternion.identity);
        Instantiate(bloodPrefab1, bloodSpawnPoint.position, Quaternion.identity);
        GameObject target = GameObject.FindGameObjectWithTag("Killable");

        if (target != null)
        {
            Destroy(target);
        }

        // Start police sirens
        yield return StartCoroutine(PoliceSirenEffect());

        // Fade to black
        yield return StartCoroutine(FadeToBlackWHenFired(3f));
    }

    IEnumerator PoliceSirenEffect()
    {
        float timer = 0;

        sirenOverlay.gameObject.SetActive(true);
        Color redSiren = new Color(1f, 0f, 0f, 0.5f);
        Color blueSiren = new Color(0f, 0f, 1f, 0.5f);

        while (timer < sirenDuration)
        {
            sirenOverlay.color = redSiren;
            yield return new WaitForSeconds(0.2f);

            sirenOverlay.color = blueSiren;
            yield return new WaitForSeconds(0.2f);

            timer += 0.4f;
        }

        sirenOverlay.gameObject.SetActive(false);
    }
    public void CloseCustomerPanel()
    {
        customerPanel.SetActive(false);

        // Re-enable movement
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Lock mouse again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isInDialogue = false;
    }
    bool IsHoldingTaco()
    {
        if (heldObject == null) return false;

        return heldObject.CompareTag("Taco");
    }

    void UpdateInteractionUI()
    {
        if (isCooking)
            return;

       
        if (IsHoldingTaco())
        {
            if (currentPlate != null)
            {
                PickupUIManager.Instance.Show("Serve at counter (E)");
            }
            else
            {
                PickupUIManager.Instance.Show("Serve at counter (E)");
            }

            return;
        }
        if (counter != null && counter.CanInteract())
        {
            PickupUIManager.Instance.Show("Serve customer (E)");
            return;
        }

        // PRIORITY 2: Grill
        if (currentGrill != null)
        {
            UpdateGrillUI();
            return;
        }

        // PRIORITY 3: Ingredient pickup
        if (currentItem != null)
        {
            if (heldObject != null)
                PickupUIManager.Instance.Show("Replace with " + currentItem.itemName + " (E)");
            else
                PickupUIManager.Instance.Show("Pick up " + currentItem.itemName + " (E)");

            return;
        }

        PickupUIManager.Instance.Hide();
    }
    void HandleEKey(Keyboard kb)
    {
        if (!kb.eKey.wasPressedThisFrame) return;

        // Serve taco first
        if (currentPlate != null && IsHoldingTaco())
        {
            ServeTaco();
            return;
        }

        // Taco pickup
        if (currentGrill != null && currentGrill.HasCookedTaco())
        {
            PickupCookedTaco();
            return;
        }

        // Ingredient pickup
        if (currentItem != null)
        {
            PickupItem();
            return;
        }

        // Place ingredient on grill (NOT taco)
        if (heldObject != null && currentGrill != null && !IsHoldingTaco())
        {
            PlaceOnGrill();
            return;
        }
        if (counter != null && counter.CanInteract())
        {
            OpenCustomerPanel();

            return;
        }
    }

    void ServeTaco()
    {
        if (heldObject == null || currentPlate == null)
            return;

        playerMovement.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameObject taco = heldObject;

        currentPlate.ServeTaco(taco);

        heldObject = null;

        StartCoroutine(EatTacoSequence(taco));

        
        reviewUIManager.AddReview();

        Debug.Log("Served taco");
    }

    IEnumerator FadeFromBlack()
    {
        float timer = 1;
        Color c = fadePanel.color;

        while (timer > 0)
        {
            timer -= Time.deltaTime * fadeSpeed;
            c.a = timer;
            fadePanel.color = c;

            yield return null;
        }
    }



    IEnumerator EatTacoSequence(GameObject taco)
    {
        // Move camera to plate
        dialogueCameraTarget = plateCameraTarget;
        isZooming = true;

        yield return new WaitForSeconds(eatDuration / 3f);

        // Half eaten
        Vector3 pos = taco.transform.position;
        Quaternion rot = taco.transform.rotation;

        Destroy(taco);
        GameObject half = Instantiate(tacoHalfPrefab, grillPoint.position + new Vector3(0, .5f, 0), Quaternion.Euler(-90, 0, 0));

        yield return new WaitForSeconds(eatDuration / 3f);

        // Almost gone
        Destroy(half);
        GameObject almost = Instantiate(tacoAlmostGonePrefab, grillPoint.position + new Vector3(0, .5f, 0), Quaternion.Euler(-90, 0, 0));

        yield return new WaitForSeconds(eatDuration / 3f);

        // Finished
        Destroy(almost);

        // Customer leaves
        if (counter.manager.CurrentCustomer != null)
        {
            Destroy(counter.manager.CurrentCustomer.gameObject);
            counter.manager.CustomerServed();
        }
        yield return StartCoroutine(FadeToBlackWhenEating(6f));
        yield return StartCoroutine(FadeFromBlack());
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Lock mouse again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isZooming = false;


        playerCamera.transform.localPosition = originalLocalPosition;
        playerCamera.transform.localRotation = originalLocalRotation;


    }

    void HandleCooking(Mouse mouse)
    {
        if (mouse.leftButton.isPressed &&
            currentGrill != null &&
            heldObject == null &&
            !currentGrill.HasCookedTaco())
        {
            if (!isCooking)
                StartCooking();

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
                StopCooking();
        }
    }

    // =========================
    // TRIGGERS
    // =========================

    void OnTriggerEnter(Collider other)
    {
        IngredientItem item = other.GetComponent<IngredientItem>();
        if (item != null)
        {
            currentItem = item;
            return;
        }

        GrillStation grill = other.GetComponent<GrillStation>();
        if (grill != null)
        {
            currentGrill = grill;
        }

        PlateStation plate = other.GetComponent<PlateStation>();
        if (plate != null)
        {
            currentPlate = plate;
        }

      

    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IngredientItem>() == currentItem)
            currentItem = null;

        if (other.GetComponent<GrillStation>() == currentGrill)
            currentGrill = null;

        if (other.GetComponent<PlateStation>() == currentPlate)
        {
            currentPlate = null;
        }

        PickupUIManager.Instance.Hide();
    }

    // =========================
    // ITEM LOGIC
    // =========================

    void PickupItem()
    {
        GameObject prefab = GetPrefabFromName(currentItem.itemName);
        if (prefab == null) return;

        if (heldObject != null)
            Destroy(heldObject);

        heldObject = Instantiate(prefab, handPoint.position, handPoint.rotation, handPoint);

        animator.SetBool("PickUp", true);
        animator.SetBool("Idle", false);
        animator.SetBool("Cook", false);

        handModel.localRotation = handModelStartRotation * Quaternion.Euler(0, 0, 90);

        currentItem = null;
        PickupUIManager.Instance.Hide();
    }

    void PlaceOnGrill()
    {
        if (heldObject == null || currentGrill == null)
            return;

        heldObject.transform.parent = null;
        heldObject.transform.position = currentGrill.grillPoint.position;
        heldObject.transform.rotation = currentGrill.grillPoint.rotation;

        currentGrill.AddIngredient(heldObject);

        heldObject = null;

        animator.SetBool("Idle", true);
        animator.SetBool("PickUp", false);

        handModel.localRotation = handModelStartRotation;
    }

    void PickupCookedTaco()
    {
        GameObject taco = currentGrill.GetCookedTaco();
        if (taco == null) return;

        if (heldObject != null)
            Destroy(heldObject);

        heldObject = taco;
        taco.transform.SetParent(handPoint);
        taco.transform.localPosition = Vector3.zero;
        taco.transform.localRotation = Quaternion.identity;

        currentGrill.ClearCookedTaco();
    }

    // =========================
    // COOKING
    // =========================

    void StartCooking()
    {
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

        handModel.localRotation = handModelStartRotation * Quaternion.Euler(0, 180f, -60f);

        cookProgressBackground.SetActive(true);
    }

    void StopCooking()
    {
        isCooking = false;
        cookTimer = 0f;

        animator.SetBool("Cook", false);
        animator.SetBool("Idle", true);

        handModel.localRotation = handModelStartRotation;

        cookProgressBar.fillAmount = 0f;
        cookProgressBackground.SetActive(false);
    }

    // =========================
    // UI
    // =========================

    void UpdateGrillUI()
    {
        if (currentGrill == null)
        {
            PickupUIManager.Instance.Hide();
            return;
        }

        if (currentGrill.HasCookedTaco())
        {
            PickupUIManager.Instance.Show("Pick up taco (E)");
            return;
        }

        if (heldObject != null && !IsHoldingTaco())
        {
            PickupUIManager.Instance.Show("Place on Grill (E)");
            return;
        }

        if (currentGrill.GetIngredientCount() >= 3)
        {
            PickupUIManager.Instance.Show("Cook (Mouse Click)");
        }
        else
        {
            PickupUIManager.Instance.Show("Needs more ingredients");
        }
    }

    // =========================
    // HELPER
    // =========================

    GameObject GetPrefabFromName(string name)
    {
        switch (name)
        {
            case "tortilla": return tortillaPrefab;
            case "bean": return beanPrefab;
            case "cheese": return cheesePrefab;
            case "salsa": return salsaPrefab;
            case "pickles": return picklePrefab;
            default: return null;
        }
    }
}