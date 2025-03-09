using UnityEngine;
using Photon.Pun;

/// <summary>
/// Handles physics-based interactions for objects in the world.
/// Allows players to pick up, throw, and manipulate objects similar to Garry's Mod.
/// </summary>
public class PhysicsObject : MonoBehaviourPun, IInteractable
{
    [Header("Physics Settings")]
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float grabDistance = 2.5f;
    [SerializeField] private float holdDistance = 2f;
    [SerializeField] private LayerMask grabMask;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float maxGrabMass = 50f;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip impactSound;
    
    // Internal references
    private Rigidbody rb;
    private AudioSource audioSource;
    private PlayerController currentHolder;
    private Transform holdPoint;
    private bool isGrabbed = false;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float originalDrag;
    private float originalAngularDrag;
    private bool wasKinematic;
    private CollisionDetector collisionDetector;
    
    // Network synchronization
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Vector3 networkVelocity;
    private Vector3 networkAngularVelocity;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.maxDistance = 20f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
        
        // Create collision detector
        GameObject detectorObj = new GameObject("CollisionDetector");
        detectorObj.transform.SetParent(transform);
        detectorObj.transform.localPosition = Vector3.zero;
        collisionDetector = detectorObj.AddComponent<CollisionDetector>();
        collisionDetector.Initialize(this);
        
        // Store initial physics properties
        if (rb != null)
        {
            originalDrag = rb.drag;
            originalAngularDrag = rb.angularDrag;
            wasKinematic = rb.isKinematic;
        }
    }
    
    private void Update()
    {
        if (isGrabbed && currentHolder != null && holdPoint != null)
        {
            // Only the player who's holding the object should update its position
            if (photonView.IsMine || (currentHolder.photonView.IsMine))
            {
                UpdateHeldPosition();
                HandleHeldInput();
            }
        }
        else if (!photonView.IsMine)
        {
            // Network position smoothing for non-grabbed objects
            if (rb != null)
            {
                rb.position = Vector3.Lerp(rb.position, networkPosition, Time.deltaTime * 10);
                rb.rotation = Quaternion.Lerp(rb.rotation, networkRotation, Time.deltaTime * 10);
                rb.velocity = networkVelocity;
                rb.angularVelocity = networkAngularVelocity;
            }
        }
    }
    
    /// <summary>
    /// Update position and rotation when being held
    /// </summary>
    private void UpdateHeldPosition()
    {
        // Calculate target position in front of the player
        targetPosition = holdPoint.position + holdPoint.forward * holdDistance;
        
        // Move the object to the target position
        if (rb != null)
        {
            Vector3 direction = targetPosition - rb.position;
            float distance = direction.magnitude;
            
            if (distance > 0.1f)
            {
                float strength = Mathf.Min(10f, distance * 10f);
                rb.velocity = direction.normalized * strength;
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
    }
    
    /// <summary>
    /// Handle input when the object is being held
    /// </summary>
    private void HandleHeldInput()
    {
        if (currentHolder == null) return;
        
        // Rotate the object
        if (Input.GetKey(KeyCode.R))
        {
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            
            if (rb != null)
            {
                rb.MoveRotation(rb.rotation * Quaternion.Euler(rotY, rotX, 0));
            }
        }
        
        // Adjust hold distance
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            holdDistance = Mathf.Clamp(holdDistance + scrollDelta, 1f, 5f);
        }
        
        // Throw the object
        if (Input.GetMouseButtonDown(0))
        {
            photonView.RPC("ThrowObject", RpcTarget.All, holdPoint.forward * throwForce);
        }
        
        // Drop the object
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E))
        {
            photonView.RPC("DropObject", RpcTarget.All);
        }
    }
    
    /// <summary>
    /// Implementation of IInteractable
    /// </summary>
    public void Interact(PlayerController player)
    {
        if (!isGrabbed)
        {
            TryGrab(player);
        }
    }
    
    /// <summary>
    /// Attempt to grab the object
    /// </summary>
    private void TryGrab(PlayerController player)
    {
        if (isGrabbed || player == null) return;
        
        // Check if the object is too heavy
        if (rb != null && rb.mass > maxGrabMass) return;
        
        // Get the camera transform for hold position
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera == null) return;
        
        holdPoint = playerCamera.transform;
        
        // Initiate grab across the network
        photonView.RPC("GrabObject", RpcTarget.All, player.photonView.ViewID);
    }
    
    [PunRPC]
    private void GrabObject(int playerViewID)
    {
        // Find the player with matching view ID
        PhotonView playerView = PhotonView.Find(playerViewID);
        if (playerView != null)
        {
            currentHolder = playerView.GetComponent<PlayerController>();
            
            if (rb != null)
            {
                // Adjust physics properties
                rb.useGravity = false;
                rb.drag = 10f;
                rb.angularDrag = 10f;
                rb.isKinematic = false;
            }
            
            isGrabbed = true;
            
            // Play grab sound
            if (audioSource != null && grabSound != null)
            {
                audioSource.PlayOneShot(grabSound);
            }
        }
    }
    
    [PunRPC]
    private void DropObject()
    {
        if (rb != null)
        {
            // Restore original physics properties
            rb.useGravity = true;
            rb.drag = originalDrag;
            rb.angularDrag = originalAngularDrag;
            rb.isKinematic = wasKinematic;
        }
        
        isGrabbed = false;
        currentHolder = null;
        holdPoint = null;
        
        // Play drop sound
        if (audioSource != null && dropSound != null)
        {
            audioSource.PlayOneShot(dropSound);
        }
    }
    
    [PunRPC]
    private void ThrowObject(Vector3 force)
    {
        if (rb != null)
        {
            // Restore original physics properties
            rb.useGravity = true;
            rb.drag = originalDrag;
            rb.angularDrag = originalAngularDrag;
            rb.isKinematic = wasKinematic;
            
            // Apply throw force
            rb.AddForce(force, ForceMode.Impulse);
        }
        
        isGrabbed = false;
        currentHolder = null;
        holdPoint = null;
        
        // Play drop sound
        if (audioSource != null && dropSound != null)
        {
            audioSource.PlayOneShot(dropSound);
        }
    }
    
    /// <summary>
    /// Called when a collision is detected
    /// </summary>
    public void OnObjectCollision(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 2f)
        {
            // Play impact sound
            if (audioSource != null && impactSound != null)
            {
                float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 10f);
                audioSource.PlayOneShot(impactSound, volume);
            }
        }
    }
    
    /// <summary>
    /// Sync object transform and physics state over the network
    /// </summary>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send position, rotation and physics data
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
            stream.SendNext(isGrabbed);
        }
        else
        {
            // Receive position, rotation and physics data
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkVelocity = (Vector3)stream.ReceiveNext();
            networkAngularVelocity = (Vector3)stream.ReceiveNext();
            isGrabbed = (bool)stream.ReceiveNext();
        }
    }
}

/// <summary>
/// Helper class to detect collisions
/// </summary>
public class CollisionDetector : MonoBehaviour
{
    private PhysicsObject parent;
    
    public void Initialize(PhysicsObject physicsObject)
    {
        parent = physicsObject;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (parent != null)
        {
            parent.OnObjectCollision(collision);
        }
    }
} 