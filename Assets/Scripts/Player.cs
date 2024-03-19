using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Vector3 targetPosition;
    public float speed = 3.0f;
    public float attackRadius = 1.5f;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private Animator animator;
    private GameObject cameraGameObject;
    private Camera mainCamera;

    private bool isAttacking = false; // Flag to indicate whether an attack is in progress
    private bool isRespawning = false;
    [SerializeField]
    private float attackDuration = 1f;

    [SerializeField]
    private int maxHealth = 100;
    private int currentHealth;

    private HealthBar healthBar;
    private XpBar xpBar;

    public GameObject playerSpawn;

    public GameObject deathPanel; // Reference to the UI canvas with death panel
    public Button respawnButton; // Reference to the respawn button in the death panel

    public bool isAlive = true;

    public int xp = 0;
    public int level = 1;
    public int xpNeededToLevel = 100;

    [SerializeField]
    private TextMeshProUGUI levelUpText;
    private int levelUpTextDuration = 3;

    [SerializeField]
    private TextMeshProUGUI levelText;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        animator.SetTrigger("idle"); //just in case
        deathPanel.SetActive(false);
        respawnButton.onClick.AddListener(RespawnPlayer);
        print(respawnButton);

        cameraGameObject = GameObject.FindGameObjectWithTag("MainCamera");

        if (cameraGameObject == null)
        {
            print("Camera object not found!");
            return;
        }

        mainCamera = cameraGameObject.GetComponent<Camera>();
        targetPosition = rb.transform.position;

        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        xpBar = GetComponentInChildren<XpBar>();
        healthBar.UpdateHealthBar((float)currentHealth, (float)maxHealth);
        xpBar.UpdateXpBar(xp, xpNeededToLevel);
        if (levelUpText) levelUpText.enabled = false;
        if (levelText) levelText.text = level.ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isAttacking && !isRespawning && isAlive)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                print("left shift and click attack");
                Attack();
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                bool didAttack = false;
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero); // Casts a ray from mouse position
                if (hit.collider != null)
                {
                    float currentDistance = Vector2.Distance(transform.position, hit.transform.position);
                    if (hit.transform.CompareTag("enemy") && currentDistance <= attackRadius)
                    {
                        GameObject enemy = hit.collider.gameObject;
                        Attack(enemy);
                        didAttack = true;
                    }
                }
                if (!didAttack) Move();
            }
        }

        rb.transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
        animator.SetTrigger("idle");
    }

    private void Attack(GameObject enemy = null)
    {
        isAttacking = true; // Set the flag to indicate that an attack is in progress
        targetPosition = transform.position;
        animator.SetTrigger("swordAttack");
        if (enemy != null)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(5);
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

    IEnumerator ResetRespawnTag()
    {
        // Delay for the duration of the attack animation or as needed
        // yield return new WaitForSeconds(attackDuration)(/*duration of the attack animation*/);
        yield return new WaitForSeconds(1);
        isRespawning = false; // Reset the flag to indicate that the attack has finished
    }

    IEnumerator ResetLevelUpText()
    {
        // Delay for the duration of the attack animation or as needed
        // yield return new WaitForSeconds(attackDuration)(/*duration of the attack animation*/);
        yield return new WaitForSecondsRealtime(levelUpTextDuration);
        print("should disable levelup text");
        if (levelUpText) levelUpText.enabled = false; // Reset the flag to indicate that the attack has finished
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            print("you died sucka");
            targetPosition = rb.transform.position;
            isAlive = false;
            deathPanel.SetActive(true);
        }
        healthBar.UpdateHealthBar((float)currentHealth, (float)maxHealth);
    }

    public void RespawnPlayer()
    {
        isRespawning = true;
        print("respawn player clicked");
        targetPosition = playerSpawn.transform.position;
        rb.transform.position = playerSpawn.transform.position;
        currentHealth = maxHealth;
        healthBar.UpdateHealthBar((float)currentHealth, (float)maxHealth);
        deathPanel.SetActive(false);
        isAlive = true;
        StartCoroutine(ResetRespawnTag());
    }

    public void AddXp(int newXp)
    {
        if (xp + newXp >= xpNeededToLevel)
        {
            level += 1;
            xp = newXp - (xpNeededToLevel - xp);
            xpNeededToLevel = Convert.ToInt32(Math.Ceiling(1.5 * level)) + 100;

            if (levelUpText) levelUpText.enabled = true;
            if (levelText) levelText.text = level.ToString();
            StartCoroutine(ResetLevelUpText());
        }
        else
        {
            xp += newXp;
        }

        xpBar.UpdateXpBar(xp, xpNeededToLevel);
    }
}