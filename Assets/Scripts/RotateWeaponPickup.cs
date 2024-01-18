using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWeaponPickup : MonoBehaviour
{
    // Public variable to control rotation speed
    public float rotationSpeed = 50.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
