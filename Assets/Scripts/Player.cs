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

    // Player states

    private State state;
    private enum State {  Normal, HookShotFlyingPlayer }

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
        if(TestInputDownHookShot())
        {
            RaycastHit hit;

            if(Physics.Raycast(myCameraHead.position, myCameraHead.forward, out hit))
            {
                hitPointTransform.position = hit.point;
                hookShotPosition = hit.point;
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

        if(Vector3.Distance(transform.position, hookShotPosition) < 2f)
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
            float extraMomentum = 40f, jumpSpeedUp = 40f;
            flyingCharacterMomentum = extraMomentum * hookShotSpeed * hookShotDirection;
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
