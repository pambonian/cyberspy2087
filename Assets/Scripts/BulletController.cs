using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed;
    public Rigidbody myRigidBody;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BulletFly();
    }

    private void BulletFly()
    {
        myRigidBody.velocity = transform.forward * speed;
    }
}
