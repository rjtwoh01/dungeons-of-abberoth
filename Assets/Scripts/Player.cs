using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public Vector3 targetPosition;
    public float speed = 3.0f;
    public float attackRadius = 1.5f;
    public int xp = 0;
    public int level = 1;
    public int xpNeededToLevel = 100;
    public bool isAlive = true;
    public List<Item> inventory = new List<Item>();
    public int potionCount = 0;

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int maxMana = 100;
    [SerializeField] private int damage = 5;
    [SerializeField] private float attackDuration = 1f;
    [SerializeField] private int levelUpTextDuration = 3;
    [SerializeField] private int potionHealAmount = 25; //percentage of life
    private float damageTextDuration = 2;

    private bool isAttacking = false;
    private bool isRespawning = false;
    private int currentHealth;
    private int currentMana;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private Animator animator;
    private Camera mainCamera;
    private GameObject cameraGameObject;

    private HealthBar healthBar;
    private XpBar xpBar;
    private ManaBar manaBar;

    public GameObject playerSpawn;
    public GameObject deathPanel;
    public Button respawnButton;
    public TextMeshProUGUI levelUpText;
    public TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI potionText;


    void Start()
    {
        InitializeComponents();
        InitializeUI();
    }

    void FixedUpdate()
    {
        if (!isAttacking && !isRespawning && isAlive)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0))
                Attack();
            else if (Input.GetKey(KeyCode.Mouse0))
                HandleClick(false);
            else if (Input.GetKey(KeyCode.Mouse1) && currentMana >= 25)
                HandleClick(true);
            else if (Input.GetKey(KeyCode.Q)) {
                TakePotion();
            }
        }

        rb.transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
        animator.SetTrigger("idle");
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        cameraGameObject = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera = cameraGameObject?.GetComponent<Camera>();
        targetPosition = rb.transform.position;

        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    private void InitializeUI()
    {
        animator.SetTrigger("idle");
        deathPanel?.SetActive(false);
        respawnButton?.onClick.AddListener(RespawnPlayer);

        if (levelUpText) levelUpText.enabled = false;
        if (levelText) levelText.text = level.ToString();
        if (potionText) potionText.text = potionCount.ToString();

        healthBar = GetComponentInChildren<HealthBar>();
        xpBar = GetComponentInChildren<XpBar>();
        manaBar = GetComponentInChildren<ManaBar>();

        healthBar?.UpdateHealthBar((float)currentHealth, (float)maxHealth);
        xpBar?.UpdateXpBar(xp, xpNeededToLevel);
        manaBar?.UpdateManaBar(currentMana, maxMana);
    }

    private void Attack(GameObject enemy = null)
    {
        isAttacking = true;
        targetPosition = transform.position;
        animator.SetTrigger("swordAttack");

        if (enemy != null)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
                currentMana += 25;
                ClampMana();
                manaBar?.UpdateManaBar(currentMana, maxMana);
            }
        }

        StartCoroutine(ResetAttackFlag());
    }

    private void HandleClick(bool isSpecialAttack)
    {
        bool didAttack = false;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            float currentDistance = Vector2.Distance(transform.position, hit.transform.position);
            if (hit.transform.CompareTag("enemy") && currentDistance <= attackRadius)
            {
                GameObject enemy = hit.collider.gameObject;
                if (isSpecialAttack)
                    SpecialAttack(enemy);
                else
                    Attack(enemy);
                didAttack = true;
            } else if (hit.transform.CompareTag("Item") && currentDistance <= attackRadius) {
                GameObject item = hit.collider.gameObject;
                print(item);
                PickUpItem(item);
            }
        }

        if (!didAttack) Move();
    }

    private void SpecialAttack(GameObject enemy)
    {
        isAttacking = true;
        targetPosition = transform.position;
        animator.SetTrigger("swordAttack");

        if (enemy != null)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage * 2);
                currentMana -= 25;
                ClampMana();
                manaBar?.UpdateManaBar(currentMana, maxMana);
            }
        }

        StartCoroutine(ResetAttackFlag());
    }

    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    private IEnumerator ResetRespawnTag()
    {
        yield return new WaitForSeconds(1);
        isRespawning = false;
    }

    private IEnumerator ResetLevelUpText()
    {
        yield return new WaitForSecondsRealtime(levelUpTextDuration);
        if (levelUpText) levelUpText.enabled = false;
    }

    IEnumerator ResetDamageText()
    {
        // Delay for the duration of the attack animation or as needed
        // yield return new WaitForSeconds(attackDuration)(/*duration of the attack animation*/);
        yield return new WaitForSeconds(damageTextDuration);
        damageText.text = ""; // Reset the flag to indicate that the attack has finished
    }

    private void Move()
    {
        animator.SetTrigger("move");
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0f;
    }

    private void PickUpItem(GameObject item)
    {
        if (item != null)
        {
            Item itemScript = item.GetComponent<Item>();
            if (itemScript != null)
            {
                if (itemScript.itemType == ItemType.Potion)
                { // Corrected this line
                    potionCount += 1;
                    if (potionText) potionText.text = potionCount.ToString();
                    itemScript.DestroyItem();
                    print("current potion count: " + potionCount);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            targetPosition = rb.transform.position;
            isAlive = false;
            deathPanel?.SetActive(true);
        }
        healthBar?.UpdateHealthBar((float)currentHealth, (float)maxHealth);

        damageText.text = damage.ToString();
        StartCoroutine(ResetDamageText());
    }

    void TakePotion() {
        if (potionCount != 0) {
            potionCount -= 1;
            if (potionCount <= 0) potionCount = 0;
            int healAmount = (int)Math.Ceiling((double)potionHealAmount / 100 * maxHealth); //x% of max health
            currentHealth += healAmount;
            if (currentHealth >= maxHealth) currentHealth = maxHealth;
            if (potionText) potionText.text = potionCount.ToString();
            healthBar?.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    public void RespawnPlayer()
    {
        isRespawning = true;
        targetPosition = playerSpawn.transform.position;
        rb.transform.position = playerSpawn.transform.position;
        currentHealth = maxHealth;
        healthBar?.UpdateHealthBar((float)currentHealth, (float)maxHealth);
        deathPanel?.SetActive(false);
        isAlive = true;
        StartCoroutine(ResetRespawnTag());
    }

    public void AddXp(int newXp)
    {
        if (xp + newXp >= xpNeededToLevel)
        {
            LevelUp(newXp);
        }
        else
        {
            GainXp(newXp);
        }

        xpBar?.UpdateXpBar(xp, xpNeededToLevel);
    }

    private void LevelUp(int newXp)
    {
        level++;
        xp = newXp - (xpNeededToLevel - xp);
        xpNeededToLevel = Mathf.CeilToInt(1.5f * level) + 100;

        if (levelUpText) levelUpText.enabled = true;
        if (levelText) levelText.text = level.ToString();
        damage += 5;
        maxHealth += 10;
        maxMana += 25;
        currentHealth = maxHealth;
        currentMana = maxMana;
        healthBar?.UpdateHealthBar(currentHealth, maxHealth);
        manaBar?.UpdateManaBar(currentMana, maxMana);
        StartCoroutine(ResetLevelUpText());
    }

    private void GainXp(int newXp)
    {
        xp += newXp;
    }


    private void ClampMana()
    {
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
    }
}