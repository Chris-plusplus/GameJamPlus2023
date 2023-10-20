using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class cController : MonoBehaviour
{
    [Header("Functional Options")]
    [SerializeField] public bool canLook = true;
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private bool willSlideOnSlopes = true;
    [SerializeField] private bool canZoom = true;
    [SerializeField] private bool useFlashlight = true;
    [SerializeField] private bool canInteract = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode persChangeKey = KeyCode.Q;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 7.5f;
    [SerializeField] private float slopeSlideSpeed = 10.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 90)] private float upperLookLimit = 88.0f;
    [SerializeField, Range(1, 90)] private float lowerLookLimit = 88.0f;
    //[SerializeField] private Transform cameraRoot;
    //[SerializeField] private Transform Camera;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;
    [SerializeField] private float multiJumpTimes = 2;
    private bool justJumped = false;

    [Header("Cinemashine")]
    [SerializeField] private GameObject cinemashineCameraTarget;
    [SerializeField] private float topCameraClamp = 89.5f;
    [SerializeField] private float bottomCameraClamp = -89.5f;
    [SerializeField] private CinemachineVirtualCamera fpsCamera;
    [SerializeField] private CinemachineVirtualCamera fpsAimCamera;
    [SerializeField] private CinemachineVirtualCamera tpsCamera;
    [SerializeField] private CinemachineVirtualCamera tpsAimCamera;

    [Header("Animation Parameters")]
    //[SerializeField] private Animator playerAnimator;
    //[SerializeField] private Animator shadowPlayerAnimator;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.5f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 1.0f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Footstep Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float sprintStepMultipier = 0.6f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;
    private float footstepTimer = 0;

    private Camera playerCamera;
    private CharacterController characterController;
    public Vector3 moveDirection;
    public Vector2 currentInput;
    private float rotationX = 0;

    public bool CanMove { get; private set; } = true;
    public bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private float GetCurrentOffset => IsSprinting ? baseStepSpeed * sprintStepMultipier : baseStepSpeed;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleMouseLook();

        ApplyFinalMovements();
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;

        /*playerAnimator.SetFloat("xVelocity", currentInput.y);
        playerAnimator.SetFloat("yVelocity", currentInput.x);
        shadowPlayerAnimator.SetFloat("xVelocity", currentInput.y);
        shadowPlayerAnimator.SetFloat("yVelocity", currentInput.x);*/
        Debug.Log(currentInput);
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        cinemashineCameraTarget.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void ApplyFinalMovements()
    {
        //if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        //if (willSlideOnSlopes && IsSliding)
        {
            //moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSlideSpeed;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }
}
