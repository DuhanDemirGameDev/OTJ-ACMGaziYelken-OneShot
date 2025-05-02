using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DedektifController : MonoBehaviour
{
    [Header("Movement Settings")] public float walkSpeed = 5f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Keep mouse locked
    }

    void Update()
    {
        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate movement relative to player's facing direction
        Vector3 move = transform.right * x + transform.forward * z;

        // Apply movement
        controller.Move(move * walkSpeed * Time.deltaTime);
    }
}
