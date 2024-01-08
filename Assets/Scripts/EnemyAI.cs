using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent myAgent;
    public LayerMask whatIsGround;

    public Vector3 destinationPoint;
    bool destinationSet;
    public float destinationRange;


    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Guarding();
    }

    private void Guarding()
    {
        if(!destinationSet)
        {
            SearchForDestination();
        } else
        {
            myAgent.SetDestination(destinationPoint);
        }

        Vector3 distanceToDestination = transform.position - destinationPoint;

        if(distanceToDestination.magnitude < 1f)
        {
            destinationSet = false;
        }
    }

    private void SearchForDestination()
    {
        // Create a random point for our agent to walk towards
        float randPositionZ = Random.Range(-destinationRange, destinationRange);
        float randPositionX = Random.Range(-destinationRange, destinationRange);

        // Set the destination
        destinationPoint = new Vector3(
            transform.position.x + randPositionX,
            transform.position.y,
            transform.position.z + randPositionZ);

        if(Physics.Raycast(destinationPoint, -transform.up, 2f, whatIsGround))
        {
            destinationSet = true;
        }
    }
}
