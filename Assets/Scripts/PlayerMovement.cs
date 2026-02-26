using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public Transform playerCamera;
    public float mouseSensitivity = 100f;
    public float maxLookAngle = 90f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    private Vector2 moveInput;

 

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        // --- Input ---
        var kb = Keyboard.current;
        moveInput = Vector2.zero;

        if (kb != null)
        {
            moveInput.y += kb.wKey.isPressed ? 1 : 0;
            moveInput.y -= kb.sKey.isPressed ? 1 : 0;
            moveInput.x -= kb.aKey.isPressed ? 1 : 0;
            moveInput.x += kb.dKey.isPressed ? 1 : 0;
        }

        moveInput = moveInput.normalized;

        // --- Horizontal Movement ---
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // --- Gravity ---
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // --- Jump ---
        if (controller.isGrounded && kb != null && kb.spaceKey.wasPressedThisFrame)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        float mouseX = mouse.delta.x.ReadValue() * mouseSensitivity * Time.deltaTime;
        float mouseY = mouse.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime;

        // Vertical rotation (camera only)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (player body)
        transform.Rotate(Vector3.up * mouseX);
    }
}