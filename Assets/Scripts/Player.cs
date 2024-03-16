using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3 targetPosition;
    public float speed = 3.0f;
    public float attackRadius = 0.75f;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private Animator animator;
    private GameObject cameraGameObject;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        animator.SetTrigger("idle"); //just incase

        cameraGameObject = GameObject.FindGameObjectWithTag("MainCamera");

        if (cameraGameObject == null)
        {
            print("Camera object not found!");
            return;
        }

        mainCamera = cameraGameObject.GetComponent<Camera>();
        print(mainCamera);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Vector2 mousePosition = Input.mousePosition;
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            bool didAttack = false;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero); // Casts a ray from mouse position
            if (hit.collider != null)
            {
                print("We clicked something!" + hit);
                float currentDistance = Vector2.Distance(transform.position, hit.transform.position);
                if (hit.transform.CompareTag("enemy") && currentDistance <= attackRadius)
                {
                    Attack();
                    didAttack = true;
                }
                else if (hit.transform.CompareTag("enemy") && currentDistance > attackRadius)
                {
                    Move();
                    Attack();
                    didAttack = true;
                }
            }
            if (!didAttack) Move();
        }

        rb.transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
        animator.SetTrigger("idle");
    }

    private void Attack()
    {
        print("should be attacking");
        //even if the player was moving to another point we still want to update
        //the target to be where we're trying to attack for a smoother experience
        targetPosition = transform.position;
        animator.SetTrigger("swordAttack");
    }

    private void Move()
    {
        animator.SetTrigger("move");
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Freeze movement by setting velocity and angular velocity to zero
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
