using UnityEngine;
using System.Collections.Generic;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public Transform playerBody; // Assign the player's body/head transform
    private float xRotation = 0f;

    [Header("Photo Capture")]
    public KeyCode photoKey = KeyCode.Mouse0; // Left-click to take photo
    public LayerMask occlusionLayers; // Layers that block visibility (e.g., walls)
    public float maxPhotoDistance = 20f; // Max distance to check for objects
    public List<GameObject> targetObjects; // Suspected objects to check

    [Header("Photo Feedback")]
    public AudioClip shutterSound;
    public GameObject photoFlashEffect;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor for FPS
    }

    void Update()
    {
        // Handle mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        // Take photo on key press
        if (Input.GetKeyDown(photoKey))
        {
            TryCapturePhoto();
        }
    }

    private void TryCapturePhoto()
    {
        bool allObjectsSeen = true;
        List<GameObject> missedObjects = new List<GameObject>();

        foreach (GameObject obj in targetObjects)
        {
            if (!IsObjectSeen(obj))
            {
                allObjectsSeen = false;
                missedObjects.Add(obj);
            }
        }

        // Play photo effects
        if (shutterSound) AudioSource.PlayClipAtPoint(shutterSound, transform.position);
        if (photoFlashEffect) Instantiate(photoFlashEffect, transform.position, transform.rotation);

        // Log results
        if (allObjectsSeen)
        {
            Debug.Log("Perfect shot! All evidence captured.");
            // TODO: Trigger success event (e.g., win condition)
        }
        else
        {
            Debug.Log("Missed some evidence: " + string.Join(", ", missedObjects));
            // TODO: Penalize player or allow retry
        }
    }

    public bool IsObjectSeen(GameObject target)
    {
        if (target == null) return false;

        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer == null) return false;

        // Check if object is within camera view
        Vector3 viewportPos = playerCamera.WorldToViewportPoint(target.transform.position);
        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1 || viewportPos.z <= 0)
        {
            return false; // Outside camera view
        }

        // Check for occlusion (if any obstacle blocks the view)
        Ray ray = new Ray(playerCamera.transform.position, (target.transform.position - playerCamera.transform.position).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, maxPhotoDistance, occlusionLayers))
        {
            return hit.collider.gameObject == target; // True if nothing blocks it
        }

        return false;
    }
}