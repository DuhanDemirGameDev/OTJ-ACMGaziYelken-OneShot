using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DedektifController : MonoBehaviour
{
    [Header("Movement Settings")] 
    public float walkSpeed = 4f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    private Vector3 velocity;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Keep mouse locked
    }

    void Update()
    {
        CheckGround();
        
        Movement();
        // Zıplama
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        // Yerçekimi uygulama
        velocity.y += gravity * Time.deltaTime;
        
        controller.Move(velocity * Time.deltaTime);
    }

    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * x + transform.forward * z);
        move.y = 0f;
        
        controller.Move(move * (walkSpeed * Time.deltaTime));
    }
/*
    private void Jump()
    {
        bool jump = Input.GetKeyDown(KeyCode.Space);
        if (jump)
        {
            controller.velocity.y(jumpHeight * -2f * gravity);
        }
        
    }
    */

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Yere yapıştır
        }

    }
}
