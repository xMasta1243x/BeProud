using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Move Settings")]
    public float moveSpeed;
    public float gravityForce;
    private Vector3 moveDirection;

    public Transform cameraTransform;
    private CharacterController characterController;
    private PlayerInput playerInput;
    private UnityEngine.InputSystem.PlayerInput unityPlayerInput;

    [Header("Jump Settings")]
    public float jumpHeight = 5f; 
    public float groundCheckDistance = 0.3f; 
    public LayerMask groundLayer; 
    private bool isGrounded;

    private float xRotation = 0f;
    private float yVelocity = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
        unityPlayerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
    }

    void Update()
    {
        Move();
        LookCamera();
        Jump();
    }

    public void Move()
    {
        isGrounded = Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer);

        Vector2 moveInput = playerInput.MoveInput;
        Vector3 horizontalMove = transform.right * moveInput.x;
        Vector3 verticalMove = transform.forward * moveInput.y;

        moveDirection = horizontalMove + verticalMove;
        moveDirection.Normalize();
        moveDirection *= moveSpeed;

        
        if (isGrounded)
        {
            if (yVelocity < 0)
                yVelocity = -2f; 
        }
        else
        {
            
            if (hit.distance < 1f) 
            {
                yVelocity += Physics.gravity.y * gravityForce * Time.deltaTime * 0.2f; 
            }
            else
            {
                yVelocity += Physics.gravity.y * gravityForce * Time.deltaTime; 
            }
        }

        moveDirection.y = yVelocity;

        if (isGrounded && hit.distance < 1f)  
        {
            float speedFactor = Mathf.Lerp(1f, 0.5f, 1f - hit.distance); 
            moveDirection *= speedFactor;  
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    public void LookCamera()
    {
        Vector2 lookInput = playerInput.LookInput;

        float sensitivity = Mouse.current != null && Mouse.current.delta.IsActuated()
        ? playerInput.mouseSensitivity
        : playerInput.playerSensitivity;

        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

    }
    public void Jump()
    {
        if (isGrounded && playerInput.JumpInput)
        {
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);

        if (isGrounded)
        {
            Gizmos.DrawSphere(transform.position + Vector3.down * groundCheckDistance, 0.1f); 
        }
    }

}
