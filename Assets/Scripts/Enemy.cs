using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject target;
    public float speed = 2f;
    [SerializeField]
    private float range = 1f;
    private float currentDistance;
    [SerializeField]
    private float viewDistance = 5f;
    [SerializeField]
    private int maxHealth = 15;
    public int currentHealth;

    private Rigidbody2D rb;

    private HealthBar healthBar;

    private bool isAttacking = false; // Flag to indicate whether an attack is in progress
    [SerializeField]
    private float attackDuration = 2f;

    [SerializeField]
    private int damage = 5;


    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        healthBar = GetComponentInChildren<HealthBar>();
        print(healthBar);

        animator = GetComponent<Animator>();
        animator.SetTrigger("idle"); //just in case

        currentHealth = maxHealth;
        healthBar.UpdateHealthBar((float)currentHealth, (float)maxHealth);
    }

    public void TakeDamage(int damage)
    {
        print("taking damage! " + damage + " damage");
        currentHealth = currentHealth - damage;
        print("health: " + currentHealth);
        healthBar.UpdateHealthBar((float)currentHealth, (float)maxHealth);
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Attack()
    {
        animator.SetTrigger("attack");
        isAttacking = true; // Set the flag to indicate that an attack is in progress
        print("isAttacking!");
        if (target != null)
        {
            print(target);
            Player playerScript = target.GetComponent<Player>();
            print(playerScript);
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
            }
        }
        // Assuming the attack animation has a duration, you can reset the isAttacking flag after a delay
        StartCoroutine(ResetAttackFlag());
    }

    IEnumerator ResetAttackFlag()
    {
        // Delay for the duration of the attack animation or as needed
        // yield return new WaitForSeconds(attackDuration)(/*duration of the attack animation*/);
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false; // Reset the flag to indicate that the attack has finished
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null || isAttacking)
            return;

        currentDistance = Vector2.Distance(transform.position, target.transform.position);

        print("current enemy distance: " + currentDistance + ", range: " + range + ", viewDistance: " + viewDistance + ", enemy position: " + transform.position + ", player position: " + target.transform.position);

        print(target);

        if (currentDistance > range && currentDistance <= viewDistance)
        {
            // Move towards the target's position continuously
            print("should be moving");
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
        else if (currentDistance <= range)
        {
            Attack();
        }

        animator.SetTrigger("idle");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Freeze movement by setting velocity and angular velocity to zero
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
