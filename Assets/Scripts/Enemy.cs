using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject target;
    public float speed = 2f;
    [SerializeField]
    private float range = 1f;
    private float currentDistance;
    [SerializeField]
    private float viewDistance = 5f;
    [SerializeField]
    private int maxHealth = 15;
    public int currentHealth;
    public int level = 1;

    private Rigidbody2D rb;

    private HealthBar healthBar;

    private bool isAttacking = false; // Flag to indicate whether an attack is in progress
    [SerializeField]
    private float attackDuration = 2f;

    [SerializeField]
    private int damage = 5;

    public int xp = 25;

    private Animator animator;

    [SerializeField]
    private TextMeshProUGUI levelText;

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
        if (levelText) levelText.text = level.ToString();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player) SetTarget(player);
    }

    public void SetTarget(GameObject newTarget)
    {
        print("setting target");
        target = newTarget;
    }


    public void TakeDamage(int damage)
    {
        print("taking damage! " + damage + " damage");
        currentHealth = currentHealth - damage;
        print("health: " + currentHealth);
        healthBar.UpdateHealthBar((float)currentHealth, (float)maxHealth);
        if (currentHealth <= 0)
        {
            if (target != null)
            {
                Player playerScript = target.GetComponent<Player>();
                if (playerScript != null && playerScript.isAlive)
                {
                    playerScript.AddXp(xp);
                }
            }
            Destroy(gameObject);
        }
    }

    private void Attack()
    {
        print("isAttacking!");
        if (target != null)
        {
            Player playerScript = target.GetComponent<Player>();
            if (playerScript != null && playerScript.isAlive)
            {
                animator.SetTrigger("attack");
                isAttacking = true; // Set the flag to indicate that an attack is in progress
                print(target);
                print(playerScript);
                playerScript.TakeDamage(damage);
                StartCoroutine(ResetAttackFlag());
            }
        }
        // Assuming the attack animation has a duration, you can reset the isAttacking flag after a delay
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
