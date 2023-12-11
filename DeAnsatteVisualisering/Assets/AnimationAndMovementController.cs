using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    PlayerInput playerInputs;
    CharacterController characterController;
    Animator animator;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;
    float rotationFactorPerFrame = 3.0f;
    float movementSpeed = 3.0f;

    void Awake()
    {
        playerInputs = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerInputs.CharacterControls.Move.started += onMovementInput;
        playerInputs.CharacterControls.Move.canceled += onMovementInput;
        playerInputs.CharacterControls.Move.performed += onMovementInput;
    }

    void onMovementInput (InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void handleAnimation()
    {
        bool isWalking = animator.GetBool("isWalking");

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }

        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool("isWalking", false);
        }
    }


    void Update()
    {
        handleRotation();
        characterController.Move(currentMovement * movementSpeed * Time.deltaTime);
        handleAnimation();
    }

    void OnEnable()
    {
        playerInputs.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInputs.CharacterControls.Disable();
    }
}
