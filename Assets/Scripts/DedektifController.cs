using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DedektifController : MonoBehaviour
{
    [Header("Movement Settings")] 
    public float walkSpeed = 2f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    private Vector3 velocity;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    [Header("Foot Step Audio")]
    public AudioSource audioSource;
    public AudioClip[] footstepClips;

    public float stepDelay = 0.5f;
    private float stepTimer = 0f;
    
    public CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Keep mouse locked
    }

    void Update()
    {
       
       
        CheckGround();
        
        Movement();
        Jump();
        
    }

    
    

    void PlayFootstep()
        {
            if (footstepClips.Length > 0)
            {
                int index = Random.Range(0, footstepClips.Length);
                audioSource.PlayOneShot(footstepClips[index]);
            }
        }
    
    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * x + transform.forward * z);
        move.y = 0f;

        bool isWalking = (x != 0 || z != 0) && isGrounded;

        if (isWalking)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepDelay;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        controller.Move(move * (walkSpeed * Time.deltaTime));
    }

    private void Jump()
    {
       if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        // Yerçekimi uygulama
        velocity.y += gravity * Time.deltaTime;
        
        controller.Move(velocity * Time.deltaTime);
    }
    

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Yere yapıştır
        }

    }
}
