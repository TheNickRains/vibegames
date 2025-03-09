using UnityEngine;
using Photon.Pun;

/// <summary>
/// Main player controller that handles movement, physics, and networking.
/// This is designed for a realistic physics-based MMORPG similar to Garry's Mod.
/// </summary>
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;
    
    [Header("Physics Settings")]
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.2f;
    
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Animator animator;
    
    // Private variables
    private CharacterController characterController;
    private Vector3 moveDirection;
    private float verticalRotation = 0f;
    private bool isGrounded;
    private float verticalVelocity;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private bool isRunning;
    
    // Constants
    private const float NETWORK_SMOOTH_TIME = 0.1f;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
        
        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck");
        }
        
        // Lock cursor for first-person control
        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Disable camera for non-local players
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(false);
            }
        }
    }
    
    private void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovement();
            HandleRotation();
            HandleActions();
            UpdateAnimation();
        }
        else
        {
            // Network position smoothing
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * (1f / NETWORK_SMOOTH_TIME));
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * (1f / NETWORK_SMOOTH_TIME));
        }
    }
    
    private void HandleMovement()
    {
        // Check if grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
        
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        isRunning = Input.GetKey(KeyCode.LeftShift);
        
        // Calculate movement
        Vector3 forward = transform.forward * vertical;
        Vector3 right = transform.right * horizontal;
        
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        moveDirection = (forward + right).normalized * currentSpeed;
        
        // Apply gravity
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
        
        // Handle jumping
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = jumpForce;
        }
        
        // Apply movement
        moveDirection.y = verticalVelocity;
        characterController.Move(moveDirection * Time.deltaTime);
    }
    
    private void HandleRotation()
    {
        // Horizontal rotation (body)
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        transform.Rotate(Vector3.up, mouseX);
        
        // Vertical rotation (camera)
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
    
    private void HandleActions()
    {
        // Interact with objects
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                }
            }
        }
    }
    
    private void UpdateAnimation()
    {
        if (animator != null)
        {
            // Set animation parameters
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsRunning", isRunning);
            animator.SetFloat("VerticalSpeed", moveDirection.magnitude);
            animator.SetFloat("VerticalVelocity", verticalVelocity);
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send position and rotation data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(isRunning);
            stream.SendNext(verticalVelocity);
        }
        else
        {
            // Receive position and rotation data
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            isRunning = (bool)stream.ReceiveNext();
            verticalVelocity = (float)stream.ReceiveNext();
        }
    }
}

/// <summary>
/// Interface for all interactable objects in the world
/// </summary>
public interface IInteractable
{
    void Interact(PlayerController player);
} 