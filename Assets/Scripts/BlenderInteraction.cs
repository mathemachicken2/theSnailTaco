using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BlenderInteraction : MonoBehaviour
{
    [Header("Hand and Spawn")]
    public Transform handSpawnPoint; // Empty in blender
    public GameObject handModel;     // Player's hand object

    [Header("Timing")]
    public float rotationDuration = 3f;

    [Header("Death Sequence")]
    public DeathTextManager deathManager; // Assign your singleton or object
    [SerializeField] private Image fadePanel;           // Blackout panel in scene

    private bool playerInRange = false;
    private bool sequenceTriggered = false;
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private Transform blenderZoomPoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomSpeed = 5f;


    private bool isZoomed = false;

    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;

    public MonoBehaviour playerMovement;

    [SerializeField] private GameObject blenderParticles;
    [SerializeField] private Image blenderOverlay;

    private void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    
    }


    void Update()
    {
        if (!playerInRange || sequenceTriggered) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.eKey.wasPressedThisFrame)
        {
            sequenceTriggered = true;
            StartCoroutine(BlenderSequence());
            


        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
        if (interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
        interactionPrompt.SetActive(false);
    }
    private IEnumerator BlenderSequence()
    {
        interactionPrompt.SetActive(false);
        // Disable player movement
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Lock mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Camera zoom
        float zoomElapsed = 0f;
        float zoomDuration = 1f;

        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        Vector3 targetPos = blenderZoomPoint.position;
        Quaternion targetRot = Quaternion.LookRotation(blenderZoomPoint.forward);

        while (zoomElapsed < zoomDuration)
        {
            zoomElapsed += Time.deltaTime * zoomSpeed;

            mainCamera.transform.position =
                Vector3.Lerp(startPos, targetPos, zoomElapsed / zoomDuration);

            mainCamera.transform.rotation =
                Quaternion.Slerp(startRot, targetRot, zoomElapsed / zoomDuration);

            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;

        // Move hand into blender
        handModel.transform.position = handSpawnPoint.position;
        handModel.transform.rotation = handSpawnPoint.rotation;
        if (blenderParticles != null)
            Instantiate(blenderParticles, handSpawnPoint.position, handSpawnPoint.rotation);
       
        if (blenderOverlay != null)
        {
            Color c = blenderOverlay.color;
            c.a = 0.2f; // fully visible
            blenderOverlay.color = c;
        }

        // Spin hand
        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            handModel.transform.Rotate(Vector3.up * 720f * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        

        // Fade to black using Image alpha
        if (fadePanel != null)
        {
            float timer = 0f;
            float fadeSpeed = 1f;

            Color c = fadePanel.color;
            c.a = 0f;
            fadePanel.color = c;

            while (timer < 1f)
            {
                timer += Time.deltaTime * fadeSpeed;
                c.a = timer;
                fadePanel.color = c;
                yield return null;
            }
        }

        // Show death message
        if (deathManager != null)
            deathManager.ShowDeathByBlenderMessage();

        // Optional: unlock mouse after sequence
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("MainMenu");
    }
}