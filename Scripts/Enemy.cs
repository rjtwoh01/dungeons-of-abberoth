using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private GameObject target;
    public float speed = 2f;
    [SerializeField]
    public float range = 1f;
    private float currentDistance;
    [SerializeField]
    public float viewDistance = 5f;
    [SerializeField]
    private int maxHealth = 15;
    public int currentHealth;
    public int level = 1;

    private Rigidbody2D rb;

    [SerializeField] private Slider healthBarSlider;
    private Bar healthBar;

    private bool isAttacking = false; // Flag to indicate whether an attack is in progress
    [SerializeField]
    private float attackDuration = 2f;

    [SerializeField]
    private int minDamage = 1;
    [SerializeField]
    private int maxDamage = 5;
    [SerializeField]
    private int criticalHitChance = 10;
    private float criticalHitDamage = 1.25f;

    public int xp = 25;

    private Animator animator;

    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI damageText;
    private float damageTextDuration = 2;

    [SerializeField] int lootChance = 25;
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] GameObject armorPrefab;
    [SerializeField] GameObject potionPrefab;
    [SerializeField] int minWeaponDamage = 1;
    [SerializeField] int maxWeaponDamage = 5;

    [SerializeField] int minItemGold = 1;
    [SerializeField] int maxItemGold = 5;

    [SerializeField] int minArmorDefense = 1;
    [SerializeField] int maxArmorDefense = 5;
    [SerializeField] string armorName = "Basic Armor";
    [SerializeField] string weaponName = "Basic Sword";

    public bool scale = false;
    public bool boss = false;
    public int manualLevelScale = 0;
    public bool customBossLoot = false;
    public bool finalBoss = false;
    Player player;
    public AudioClip attackSound;
    [SerializeField] private AudioSource audioSource;

    private int baseHealth;
    private int baseMinDamage;
    private int baseMaxDamage;
    private int baseXp;
    private int playerLevel;

    private Player playerScript;

    public GameObject enemySpawner;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        healthBar = healthBarSlider.GetComponent<Bar>();

        animator = GetComponent<Animator>();
        animator.SetTrigger("idle"); //just in case

        currentHealth = maxHealth;
        healthBar?.UpdateBar((float)currentHealth, (float)maxHealth);
        if (levelText) levelText.text = level.ToString();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            SetTarget(player);
            playerScript = player.GetComponent<Player>();
            playerLevel = player.GetComponent<Player>().level;
        }
        if (scale)
        {
            baseHealth = maxHealth;
            baseMinDamage = minDamage;
            baseMaxDamage = maxDamage;
            baseXp = xp;
            ScaleEnemy();
        }
        enemySpawner = GameObject.FindGameObjectWithTag("EnemySpawner");
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        player = target.GetComponent<Player>();
    }

    public void ScaleEnemy()
    {
        //scale to player power
        // minDamage = (int)(player.minDamage * 0.8f);
        // maxDamage = (int)(player.maxDamage * 0.8f);
        // maxHealth = (int)(player.maxHealth * 0.8f);
        float scaleFactor = 1.3f;
        if (boss) scaleFactor = 1.4f;

        if (manualLevelScale == 0)
        {
            level = player.level;
            if (boss) level += 3;
            if (player.level > 1)
            {
                minDamage = (int)(baseMinDamage * scaleFactor * level);
                maxDamage = (int)(baseMaxDamage * scaleFactor * level);
                maxHealth = (int)(baseHealth * scaleFactor * level) * 2;
                xp = (int)(baseXp + (baseXp * 0.25f * level));
            }
            currentHealth = maxHealth;
        }
        else
        {
            level = manualLevelScale;
            minDamage = (int)(baseMinDamage * scaleFactor * manualLevelScale);
            maxDamage = (int)(baseMaxDamage * scaleFactor * manualLevelScale);
            maxHealth = (int)(baseHealth * scaleFactor * manualLevelScale);
            xp = (int)(baseXp + (baseXp * 0.25f * level));
            currentHealth = maxHealth;
        }
        if (boss)
        {
            maxHealth *= 4;
            minDamage += Mathf.CeilToInt(0.5f * minDamage);
            maxDamage += Mathf.CeilToInt(0.5f * maxDamage);
            xp += Mathf.CeilToInt(0.5f * xp);
            currentHealth = maxHealth;
        }

        levelText.text = level.ToString();
        healthBar?.UpdateBar(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
        healthBar?.UpdateBar((float)currentHealth, (float)maxHealth);
        damageText.text = damage.ToString();
        StartCoroutine(ResetDamageText());
        if (currentHealth <= 0)
        {
            if (target != null)
            {
                if (playerScript != null && playerScript.isAlive)
                {
                    playerScript.AddXp(xp);
                    if (finalBoss) playerScript.completedStory = true;
                }

                if (enemySpawner != null)
                {
                    enemySpawner.GetComponent<EnemySpawner>().EnemyDestroyed(this);
                }
            }
            GenerateLoot();
            Destroy(gameObject);
        }
    }

    private void Attack()
    {
        if (target != null)
        {
            if (playerScript != null && playerScript.isAlive)
            {
                animator.SetTrigger("attack");
                audioSource.clip = attackSound;
                audioSource.Play();
                isAttacking = true; // Set the flag to indicate that an attack is in progress
                int damage = Random.Range(minDamage, maxDamage);
                int critRoll = Random.Range(1, 100);
                if (critRoll <= criticalHitChance) damage = Mathf.CeilToInt(damage * criticalHitDamage);
                playerScript.TakeDamage(damage);
                animator.SetTrigger("idle");
                StartCoroutine(ResetAttackFlag());
            }
        }
        // Assuming the attack animation has a duration, you can reset the isAttacking flag after a delay
    }

    private void GenerateLoot()
    {
        if (!scale || customBossLoot) PredeterminedLoot();
        else ScaledLoot();
    }

    private void PredeterminedLoot()
    {
        int lootRoll = Random.Range(1, 101);
        if (lootRoll <= lootChance)
        {
            int itemRoll = Random.Range(1, 3);
            int itemGold = Random.Range(minItemGold, maxItemGold + 1);
            // 
            if (itemRoll == 1)
            {
                GameObject weaponInstance = Instantiate(weaponPrefab, transform.position, Quaternion.identity);
                Weapon weaponScript = weaponInstance.GetComponent<Weapon>();
                if (weaponScript != null)
                {
                    weaponScript.name = weaponName;
                    weaponScript.damage = Random.Range(minWeaponDamage, maxWeaponDamage + 1);
                    weaponScript.gold = itemGold;
                }
            }
            else if (itemRoll == 2)
            {
                GameObject ArmorInstance = Instantiate(armorPrefab, transform.position, Quaternion.identity);
                Armor armorScript = ArmorInstance.GetComponent<Armor>();
                if (armorScript != null)
                {
                    armorScript.name = armorName;
                    armorScript.defense = Random.Range(minArmorDefense, maxArmorDefense + 1);
                    armorScript.gold = itemGold;
                }
            }
            else
            {
                GameObject PotionInstance = Instantiate(potionPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    private void ScaledLoot()
    {
        int lootRoll = Random.Range(1, 101);
        if (boss) lootChance = 100;
        if (lootRoll <= lootChance)
        {
            int lootLevel = level;
            List<Item> lootItems = new List<Item>();
            GameObject itemGeneratorObject = new GameObject("gernator");
            itemGeneratorObject.AddComponent<ItemGenerator>();
            ItemGenerator itemGenerator = itemGeneratorObject.GetComponent<ItemGenerator>();
            itemGenerator.armorPrefab = armorPrefab;
            itemGenerator.weaponPrefab = weaponPrefab;
            int legendaryChance = 1;
            if (boss) legendaryChance = 10;
            lootItems = itemGenerator.GenerateItems(level, 1, boss, legendaryChance);

            if (lootItems[0].itemType == ItemType.Weapon)
            {
                Weapon weapon = lootItems[0] as Weapon;
                GameObject weaponInstance = Instantiate(weaponPrefab, transform.position, Quaternion.identity);
                Weapon weaponScript = weaponInstance.GetComponent<Weapon>();
                if (weaponScript != null)
                {
                    int damage = weapon.damage;
                    weaponScript.name = weapon.name;
                    weaponScript.damage = damage;
                    weaponScript.gold = weapon.gold;
                }
            }
            else
            {
                Armor armor = lootItems[0] as Armor;
                GameObject ArmorInstance = Instantiate(armorPrefab, transform.position, Quaternion.identity);
                Armor armorScript = ArmorInstance.GetComponent<Armor>();
                if (armorScript != null)
                {
                    int defense = armor.defense;
                    armorScript.name = armor.name;
                    armorScript.defense = defense;
                    armorScript.gold = armor.gold;
                }
            }
        }
    }

    IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false; // Reset the flag to indicate that the attack has finished
    }

    IEnumerator ResetDamageText()
    {
        yield return new WaitForSeconds(damageTextDuration);
        damageText.text = ""; // Reset the flag to indicate that the attack has finished
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null || isAttacking)
            return;

        currentDistance = Vector2.Distance(transform.position, target.transform.position);

        if (currentDistance > range && currentDistance <= viewDistance)
        {
            // Move towards the target's position continuously

            animator.SetTrigger("move");
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
        else if (currentDistance <= range)
        {
            Attack();
        }

        if (playerScript.level != playerLevel && scale)
        {
            playerLevel = playerScript.level;
            ScaleEnemy();
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
