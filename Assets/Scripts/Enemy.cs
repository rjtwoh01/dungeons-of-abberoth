using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject target;
    public float speed = 2f;
    [SerializeField]
    private float range = 0.75f;
    private float currentDistance;
    [SerializeField]
    private float viewDistance = 5f;
    [SerializeField]
    private int maxHealth = 15;
    public int currentHealth;

    private Rigidbody2D rb;

    private HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        healthBar = GetComponentInChildren<HealthBar>();
        print(healthBar);

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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null)
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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Freeze movement by setting velocity and angular velocity to zero
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
