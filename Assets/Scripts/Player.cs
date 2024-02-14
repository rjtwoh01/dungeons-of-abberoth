using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3 targetPosition;
    public float speed = 3.0f;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetTrigger("idle"); //just incase
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Mouse0))
        {
            print("should be attacking");
            //even if the player was moving to another point we still want to update
            //the target to be where we're trying to attack for a smoother experience
            targetPosition = transform.position;
            animator.SetTrigger("swordAttack");
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            animator.SetTrigger("move");
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0f;
        }

        rb2D.transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
        animator.SetTrigger("idle");
    }

}
