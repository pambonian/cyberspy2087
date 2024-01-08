using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasicMove : MonoBehaviour
{

    Animator myAnimator;
    Transform player;

    public bool move, rotate;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();

        player = FindObjectOfType<Player>().transform;

        // myAnimator.SetBool("Move", true);

        // Set initial animation states
        myAnimator.SetBool("Move", move);
        myAnimator.SetBool("Rotating", rotate);
    }

    // Update is called once per frame
    void Update()
    {
        // look at player
        transform.LookAt(player);
    }
}
