using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public Transform playerBody; // Assign the player's body/head transform
    private float xRotation = 0f;
    
    private bool cameraMode = false;
     public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    private void CameraMode()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            cameraMode = !cameraMode;
        }

        if (cameraMode)
        {
            text1.gameObject.SetActive(false);
            text2.gameObject.SetActive(true);
        }
        else
        {
            text1.gameObject.SetActive(true);
            text2.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        text1.gameObject.SetActive(true);
        text2.gameObject.SetActive(false);
    }

    void Update()
    {
       HandleMouseLook();
       CameraMode();
    }

    private void HandleMouseLook()
    {
        // Handle mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    
}