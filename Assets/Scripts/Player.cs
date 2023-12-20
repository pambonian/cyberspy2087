using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 12.5f;

    // adding Gravity
    public Vector3 velocity;
    public float gravityModifier;

    public CharacterController myController;
    public Transform myCameraHead;

    public float mouseSensitivity = 100f;
    private float cameraVerticalRotation;

    public GameObject bullet;
    public Transform firePosition;

    public GameObject muzzleFlash, bulletHole, waterLeak;

    // jumping values
    public float jumpHeight = 10f;
    private bool readyToJump;
    public Transform ground;
    public LayerMask groundLayer;
    public float groundDistance = 0.5f;

    // crouching values
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float crouchSpeed = 6f;
    private bool isCrouching = false;

    // Start is called before the first frame update
    void Start()
    {
        // get initial player scale for crouching
        playerScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        CameraMovement();
        Jump();
        Shoot();
        Crouching();
    }

    // crouching method
    private void Crouching()
    {
        // start crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCrouching();
        } 

        // end crouch
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            StopCrouching();
        }
    }

    private void StartCrouching()
    {
        transform.localScale = crouchScale;
        isCrouching= true;
    }

    private void StopCrouching()
    {
        transform.localScale = playerScale;
        isCrouching= false;
    }

    // jumping method
    void Jump()
    {
        readyToJump = Physics.OverlapSphere(ground.position, groundDistance, groundLayer).Length > 0;

        if(Input.GetButtonDown("Jump") && readyToJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y) * Time.deltaTime;
        }

        myController.Move(velocity);
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if(Physics.Raycast(myCameraHead.position, myCameraHead.forward, out hit, 100f))
            {
                if (Vector3.Distance(myCameraHead.position, hit.point) >2f)
                {

                    firePosition.LookAt(hit.point);

                    if(hit.collider.CompareTag("Shootable"))
                    {
                        Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
                    }

                    if(hit.collider.CompareTag("WaterLeaker"))
                    {
                        Instantiate(waterLeak, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                }

                if (hit.collider.CompareTag("Enemy"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
            
            else
            {
                firePosition.LookAt(myCameraHead.position + (myCameraHead.forward * 50f));
            }

            Instantiate(muzzleFlash, firePosition.position, firePosition.rotation, firePosition);
            Instantiate(bullet, firePosition.position, firePosition.rotation);
        }
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

        if (isCrouching)
        {
            movement = movement * crouchSpeed * Time.deltaTime;
        }
        else
        {
            movement = movement * speed * Time.deltaTime;
        }

        myController.Move(movement);

       
        // gravity (stay the hell away from me)

        velocity.y += Physics.gravity.y * Mathf.Pow(Time.deltaTime, 2) * gravityModifier;

        if(myController.isGrounded)
        {
            velocity.y = Physics.gravity.y * Time.deltaTime;
        }

        myController.Move(velocity);
    }
}
