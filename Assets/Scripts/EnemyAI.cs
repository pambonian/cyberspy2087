using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent myAgent;
    public LayerMask whatIsGround, whatIsPlayer;

    public Transform player;
    Animator myAnimator;

    public Transform firePosition;

    // Guarding
    public Vector3 destinationPoint;
    bool destinationSet;
    public float destinationRange;

    // Chasing
    public float chaseRange;
    private bool playerInChaseRange;

    // Attacking
    public float attackRange, attackTime;
    private bool playerInAttackRange, readyToAttack = true;
    public GameObject attackProjectile;

    // Melee Attack
    public bool meleeAttacker;
    public int meleeDamageAmount = 2;



    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        player = FindObjectOfType<Player>().transform;
        myAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInChaseRange = Physics.CheckSphere(transform.position, chaseRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInChaseRange && !playerInAttackRange)
        {
            Guarding();
        } 
        if (playerInChaseRange && !playerInAttackRange) 
        {
            ChasingPlayer();
        }
        if(playerInChaseRange && playerInAttackRange)
        {
            AttackingPlayer();
        }
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

    private void ChasingPlayer()
    {
        myAgent.SetDestination(player.position);
    }

    private void AttackingPlayer()
    {
        myAgent.SetDestination(transform.position);
        transform.LookAt(player);

        if(readyToAttack && !meleeAttacker)
        {
            myAnimator.SetTrigger("Attack");

            firePosition.LookAt(player);

            Instantiate(attackProjectile, firePosition.position, firePosition.rotation);

            readyToAttack = false;
            StartCoroutine(ResetAttack());
        }
        else if (readyToAttack && meleeAttacker)
        {
            myAnimator.SetTrigger("Attack");
        }

    }

    public void MeleeDamage()
    {
        if(playerInAttackRange)
        {
            player.GetComponent<PlayerHealthSystem>().TakeDamage(meleeDamageAmount);
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

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackTime);
        readyToAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
