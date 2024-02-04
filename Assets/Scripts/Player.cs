using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 12.5f, runSpeed = 25f;

    // adding Gravity
    public Vector3 velocity;
    public float gravityModifier;

    public CharacterController myController;
    public Transform myCameraHead;
    public Animator myAnimator;

    public float mouseSensitivity = 100f;
    private float cameraVerticalRotation;
    // 
    
    // jumping values
    public float jumpHeight = 10f;
    private bool readyToJump;
    public Transform ground;
    public LayerMask groundLayer;
    public float groundDistance = 0.5f;

    // crouching values
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 bodyScale;
    public Transform myBody;
    private float initialControllerHeight;
    public float crouchSpeed = 6f;
    private bool isCrouching = false;

    // sliding values
    private bool isRunning = false, startSliderTimer;
    private float currentSliderTimer, maxSlideTime = 2f;
    public float slideSpeed = 30f;

    // Hook shot
    public Transform hitPointTransform;
    private Vector3 hookShotPosition;
    public float hookShotSpeed = 5f;
    private Vector3 flyingCharacterMomentum;
    // public Transform grapplingHook;
    private float hookShotSize;
    public GameObject grapplingHookPrefab;
    private GameObject currentGrapplingHook;
    public Transform grapplingHookSpawnPoint;


    // Player states

    private State state;
    private enum State {  Normal, HookShotFlyingPlayer, HookShotThrow }

    // Start is called before the first frame update
    void Start()
    {
        // get initial player body scale for crouching
        bodyScale = myBody.localScale;
        initialControllerHeight = myController.height;

        state = State.Normal;
    }

    // Update is called once per frame

    // This is a comment
    void Update()
    {

        switch(state)
        {
            case State.Normal:
                PlayerMovement();
                CameraMovement();
                Jump();
                Crouching();
                SlideCounter();
                HandleHookShotStart();
                break;

            case State.HookShotThrow:
                PlayerMovement();
                CameraMovement();
                ThrowHook();
                break;

            case State.HookShotFlyingPlayer:
                CameraMovement();
                HandleHookShotMovement();
                break;

            default:
                break;
        }
        
    }

    // crouching method
    private void Crouching()
    {
        // start crouch
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCrouching();
        } 

        // end crouch
        if (Input.GetKeyUp(KeyCode.C) || currentSliderTimer > maxSlideTime)
        {
            StopCrouching();
        }
    }

    private void StartCrouching()
    {
        myBody.localScale = crouchScale;
        myCameraHead.position -= new Vector3(0, 1f, 0);

        myController.height /= 2;
        isCrouching= true;

        if(isRunning)
        {
            velocity = Vector3.ProjectOnPlane(myCameraHead.transform.forward, Vector3.up).normalized * slideSpeed * Time.deltaTime;
            startSliderTimer = true;
        }
    }

    private void StopCrouching()
    {
        currentSliderTimer = 0f;
        velocity = new Vector3(0f, 0f, 0f);
        startSliderTimer = false;

        myBody.localScale = bodyScale;
        myCameraHead.position += new Vector3(0, 1f, 0);

        myController.height = initialControllerHeight;
        isCrouching = false;
    }

    // jumping method
    void Jump()
    {
        readyToJump = Physics.OverlapSphere(ground.position, groundDistance, groundLayer).Length > 0;

        if(Input.GetButtonDown("Jump") && readyToJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y) * Time.deltaTime;
            AudioManager.instance.PlayerSFX(2);
        }

        myController.Move(velocity);
    }

    
    private void CameraMovement()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        cameraVerticalRotation -= mouseY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);
        myCameraHead.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);
    }

    void PlayerMovement() 
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = x * transform.right + z * transform.forward;

        if(Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            movement = movement * runSpeed * Time.deltaTime;
            
            isRunning = true;
        }
        else if (isCrouching)
        {
            movement = movement * crouchSpeed * Time.deltaTime;
        }
        else
        {
            movement = movement * speed * Time.deltaTime;
            
            isRunning = false;
        }

        myAnimator.SetFloat("PlayerSpeed", movement.magnitude);
        // Debug.Log(movement.magnitude);

        movement += flyingCharacterMomentum * Time.deltaTime;

        myController.Move(movement);

       
        // gravity ... (stay the hell away from me)

        velocity.y += Physics.gravity.y * Mathf.Pow(Time.deltaTime, 2) * gravityModifier;

        if(myController.isGrounded)
        {
            velocity.y = Physics.gravity.y * Time.deltaTime;
        }

        myController.Move(velocity);

        if(flyingCharacterMomentum.magnitude > 0f)
        {
            float reductionAmount = 4f;
            flyingCharacterMomentum -= reductionAmount * Time.deltaTime * flyingCharacterMomentum;
            if (flyingCharacterMomentum.magnitude > 5f)
            {
                flyingCharacterMomentum = Vector3.zero;
            }
        }
    }

    private void SlideCounter()
    {
        if(startSliderTimer)
        {
            currentSliderTimer += Time.deltaTime;
        }
    }

    private void HandleHookShotStart()
    {
        if (TestInputDownHookShot())
        {
            RaycastHit hit;

            if (Physics.Raycast(myCameraHead.position, myCameraHead.forward, out hit))
            {
                hitPointTransform.position = hit.point;
                hookShotPosition = hit.point;

                hookShotSize = 0f;

                // Instantiate the grappling hook at the player's position
                if (currentGrapplingHook != null) Destroy(currentGrapplingHook); // Destroy previous grappling hook if it exists
                currentGrapplingHook = Instantiate(grapplingHookPrefab, grapplingHookSpawnPoint.position, grapplingHookSpawnPoint.rotation);


                // Set the player's camera head as the parent of the grappling hook
                currentGrapplingHook.transform.SetParent(grapplingHookSpawnPoint);


                // Optionally, you can reset the local position and rotation of the grappling hook relative to the new parent:
                currentGrapplingHook.transform.localPosition = Vector3.zero; // Or any specific position
                currentGrapplingHook.transform.localRotation = Quaternion.identity; // Or any specific rotation

                state = State.HookShotThrow;
            }
        }
    }

    private void ThrowHook()
    {
        if (currentGrapplingHook != null)
        {
            // Calculate the direction from the spawn point to the target.
            Vector3 directionToTarget = (hookShotPosition - grapplingHookSpawnPoint.position).normalized;

            // Set the grappling hook to face the target.
            currentGrapplingHook.transform.forward = directionToTarget;

            // Calculate how much to scale or move the grappling hook this frame.
            // Ensure this uses the correct axis. If the grappling hook extends along its local Z axis, for example:
            float hookShotThrowSpeed = 500f;
            hookShotSize += hookShotThrowSpeed * Time.deltaTime;

            // If you're scaling the grappling hook, apply the scaling along the correct axis.
            // If it extends along its local Z axis:
            currentGrapplingHook.transform.localScale = new Vector3(1, 1, hookShotSize);

            // If you're moving the grappling hook instead of scaling, move it along the directionToTarget.
            // Example:
            // currentGrapplingHook.transform.position += directionToTarget * hookShotThrowSpeed * Time.deltaTime;

            // Check if the grappling hook has reached the target.
            if (hookShotSize >= Vector3.Distance(grapplingHookSpawnPoint.position, hookShotPosition))
            {
                state = State.HookShotFlyingPlayer;
            }
        }
    }


    private void HandleHookShotMovement()
    {

        // direction of movement
        Vector3 hookShotDirection = (hookShotPosition - transform.position).normalized;

        float hookShotMinSpeed = 12f, hookShotMaxSpeed = 50f;

        float hookShotSpeedModifier = Mathf.Clamp(
            Vector3.Distance(transform.position, hookShotPosition),
            hookShotMinSpeed,
            hookShotMaxSpeed);

        myController.Move(hookShotDirection * hookShotSpeed * hookShotSpeedModifier * Time.deltaTime);

        if (Vector3.Distance(transform.position, hookShotPosition) < 2f || TestInputDownHookShot() || TestInputJump())
        {
            state = State.Normal;
            ResetGravity();

            if (currentGrapplingHook != null)
            {
                Destroy(currentGrapplingHook); // Destroy the grappling hook
            }
        }

        if (Vector3.Distance(transform.position, hookShotPosition) < 2f)
        {
            state = State.Normal;
            ResetGravity();
        }

        if(TestInputDownHookShot()) {
            state = State.Normal;
            ResetGravity();
        }

        if(TestInputJump())
        {
            float extraMomentum = 40f, jumpSpeedUp = 70f;
            flyingCharacterMomentum += extraMomentum * hookShotSpeed * hookShotDirection;
            flyingCharacterMomentum += Vector3.up * jumpSpeedUp;

            state = State.Normal;
            ResetGravity();

        }
    }

    private bool TestInputJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private bool TestInputDownHookShot()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    private void ResetGravity()
    {
        velocity.y = 0f;
    }
}
