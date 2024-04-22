using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player instance;
    public string playerName = "";
    public Vector3 targetPosition;
    public float speed = 3.0f;
    public float attackRadius = 2.0f;
    public int xp = 0;
    public int level = 1;
    public int xpNeededToLevel = 100;
    public int maxLevel = 100;
    public bool isAlive = true;
    public List<Item> inventory;
    public int potionCount = 0;
    public bool takingPotion = false;
    public string lastVisitedLocation = "MainMenu";
    public int gold = 0;
    public List<string> visitedLocations = new List<string>();

    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int maxMana = 100;
    [SerializeField] public int minDamage = 1;
    [SerializeField] public int maxDamage = 5;
    [SerializeField] public int criticalHitChance = 10;
    [SerializeField] public int criticalHitDamage = 2;
    [SerializeField] public int defense = 0;
    [SerializeField] public float attackDuration = 1f;
    [SerializeField] public float sellItemDuration = 60f;
    [SerializeField] public int levelUpTextDuration = 3;
    [SerializeField] public int potionHealAmount = 75; //percentage of life
    [SerializeField] public int maxInventorySize = 30;
    [SerializeField] public int manaPerHit = 25;
    private float damageTextDuration = 2;

    private bool isAttacking = false;
    private bool isRespawning = false;
    [SerializeField] public int currentHealth;
    [SerializeField] public int currentMana;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private Animator animator;
    private Camera mainCamera;
    private GameObject cameraGameObject;

    [SerializeField] private Slider healthBarSlider;
    private GameObject xpBarObject;
    private Slider xpBarSlider;
    [SerializeField] private Slider manaBarSlider;

    private Bar healthBar, xpBar, manaBar;

    private GameObject playerSpawn;
    public GameObject deathPanel;
    public Button respawnButton;
    private GameObject levelUpObject;
    private GameObject levelObject;
    private TextMeshProUGUI levelUpText;
    private TextMeshProUGUI levelText;
    private TextMeshProUGUI characterNameText;
    private TextMeshProUGUI sceneNameText;
    [SerializeField] private TextMeshProUGUI damageText;
    private GameObject potionTextObject;
    public TextMeshProUGUI potionText;

    public Armor equippedArmor;
    public Weapon equippedWeapon;
    [SerializeField] GameObject startingArmor;
    [SerializeField] GameObject startingWeapon;
    [SerializeField] GameObject armorPrefab;
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] GameObject levelManager;
    private GameObject playerHUD;

    public bool initializing = true;

    public bool inventoryOpen = false;
    public bool teleportOpen = false;
    public bool characterInfoOpen = false;
    public bool canSellItems = false;
    public bool completedStory = false;
    public bool shopOpen = false;

    public AudioClip moveClip;
    public AudioClip attackSound;
    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        inventory = new List<Item>();
        visitedLocations.Add("Starting Tomb");
        equippedArmor = null;
        equippedWeapon = null;
        InitializeComponents();
        InitializeUI();
        InitialGearSetup();
        initializing = false;
    }

    void FixedUpdate()
    {
        // if (EventSystem.current.IsPointerOverGameObject())
        // {
        //     // Player is clicking on a UI element, so they can't move
        //     return;
        // }
        bool isOnMainMenu = SceneManager.GetActiveScene().name == "MainMenu";

        if (!isAttacking && !isRespawning && isAlive && !inventoryOpen && !teleportOpen && !isOnMainMenu && !takingPotion)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0))
                Attack();
            else if (Input.GetKey(KeyCode.Mouse0))
                HandleClick(false);
            else if (Input.GetKey(KeyCode.Mouse1) && currentMana >= 25)
                HandleClick(true);
            else if (Input.GetKey(KeyCode.Q))
                TakePotion();
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
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawner");
        playerHUD = GameObject.FindGameObjectWithTag("PlayerHUD");
        deathPanel = GameObject.FindGameObjectWithTag("DeathPanel");
        respawnButton = GameObject.FindGameObjectWithTag("RespawnBtn").GetComponent<Button>();
        characterNameText = GameObject.FindGameObjectWithTag("CharacterName").GetComponent<TextMeshProUGUI>();
        sceneNameText = GameObject.FindGameObjectWithTag("SceneName").GetComponent<TextMeshProUGUI>();
        targetPosition = rb.transform.position;

        currentHealth = maxHealth;
        currentMana = maxMana;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void InitializeUI()
    {
        animator.SetTrigger("idle");
        deathPanel?.SetActive(false);
        respawnButton?.onClick.AddListener(RespawnPlayer);

        xpBarObject = GameObject.FindGameObjectWithTag("XpBar");
        levelUpObject = GameObject.FindGameObjectWithTag("LevelUpText");
        levelObject = GameObject.FindGameObjectWithTag("LevelText");
        potionTextObject = GameObject.FindGameObjectWithTag("potionCount");

        if (xpBarObject) xpBarSlider = xpBarObject.GetComponent<Slider>();
        if (levelUpObject) levelUpText = levelUpObject.GetComponent<TextMeshProUGUI>();
        if (levelObject) levelText = levelObject.GetComponent<TextMeshProUGUI>();
        if (potionTextObject) potionText = potionTextObject.GetComponent<TextMeshProUGUI>();

        if (levelUpText) levelUpText.enabled = false;
        if (levelText) levelText.text = level.ToString();
        if (potionText) potionText.text = potionCount.ToString();

        xpBar = xpBarSlider.GetComponent<Bar>();
        healthBar = healthBarSlider.GetComponent<Bar>();
        manaBar = manaBarSlider.GetComponent<Bar>();

        healthBar?.UpdateBar((float)currentHealth, (float)maxHealth);
        xpBar?.UpdateBar(xp, xpNeededToLevel);
        manaBar?.UpdateBar(currentMana, maxMana);

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            playerHUD.SetActive(false);
            mainCamera.gameObject.SetActive(false);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (lastVisitedLocation != scene.name) lastVisitedLocation = scene.name;
        if (scene.name != "MainMenu")
        {
            playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawner");
            // rb.transform.position = playerSpawn.transform.position;
            RespawnPlayer();
            cameraGameObject.SetActive(true);
            mainCamera.gameObject.SetActive(true);
            if (characterNameText) characterNameText.text = playerName;
            if (sceneNameText) sceneNameText.text = scene.name;
            int index = visitedLocations.FindIndex(loc => loc == scene.name);
            if (index == -1) visitedLocations.Add(scene.name);
            if (visitedLocations.Contains("Town")) canSellItems = true;
        }
        else
        {
            cameraGameObject.SetActive(false);
            mainCamera.gameObject.SetActive(false);
            if (characterNameText) characterNameText.text = "";
            if (sceneNameText) sceneNameText.text = "";
        }


        if (scene.name == "Town")
        {
            canSellItems = true;
        }
    }


    private void InitialGearSetup()
    {
        if (equippedArmor == null && startingArmor != null)
        {
            GameObject newStartingArmor = Instantiate(startingArmor);
            DontDestroyOnLoad(newStartingArmor);
            newStartingArmor.SetActive(false);
            Armor newArmor = newStartingArmor.GetComponent<Armor>();
            if (newArmor != null)
            {
                newArmor.name = "Basic Armor";
                newArmor.defense = 1;
                EquipItem(newArmor, -1, true);
            }
        }
        if (equippedWeapon == null && startingWeapon != null)
        {
            GameObject newStartingWeapon = Instantiate(startingWeapon);
            DontDestroyOnLoad(newStartingWeapon);
            newStartingWeapon.SetActive(false);
            Weapon newWeapon = newStartingWeapon.GetComponent<Weapon>();
            if (newWeapon != null)
            {
                newWeapon.name = "Basic Sword";
                newWeapon.damage = 1;
                EquipItem(newWeapon, -1, true);
            }
        }
    }

    private void Attack(GameObject enemy = null)
    {
        isAttacking = true;
        targetPosition = transform.position;
        animator.SetTrigger("swordAttack");
        audioSource.clip = attackSound;
        audioSource.Play();

        if (enemy != null)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                int damage = UnityEngine.Random.Range(minDamage, maxDamage);
                int critRoll = UnityEngine.Random.Range(1, 100);
                if (critRoll <= criticalHitChance) damage *= criticalHitDamage;
                currentMana += manaPerHit;
                ClampMana();
                manaBar?.UpdateBar(currentMana, maxMana);
                enemyScript.TakeDamage(damage);
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
            }
            else if (hit.transform.CompareTag("Item") && currentDistance <= attackRadius)
            {
                GameObject item = hit.collider.gameObject;
                PickUpItem(item);
            }
            else if (hit.transform.CompareTag("TreasureChest") && currentDistance <= attackRadius)
            {
                TreasureChest treasureChest = hit.collider.gameObject.GetComponent<TreasureChest>();
                treasureChest.OpenChest();
            }
        }

        if (!didAttack) Move();
    }

    private void SpecialAttack(GameObject enemy)
    {
        isAttacking = true;
        targetPosition = transform.position;
        animator.SetTrigger("swordAttack");
        audioSource.clip = attackSound;
        audioSource.Play();

        if (enemy != null)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                int damage = UnityEngine.Random.Range(minDamage, maxDamage);
                int critRoll = UnityEngine.Random.Range(1, 100);
                if (critRoll <= criticalHitChance) damage *= criticalHitDamage;
                currentMana -= 25;
                ClampMana();
                manaBar?.UpdateBar(currentMana, maxMana);
                enemyScript.TakeDamage(damage * 2);
            }
        }

        StartCoroutine(ResetAttackFlag());
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
        audioSource.clip = moveClip;
        audioSource.Play();
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0f;
    }

    public void MoveToLocation(Vector3 newPosition)
    {
        targetPosition = newPosition;
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
                {
                    potionCount += 1;
                    if (potionText) potionText.text = potionCount.ToString();
                    itemScript.DestroyItem();
                }
                else if ((itemScript.itemType == ItemType.Weapon || itemScript.itemType == ItemType.Armor) && inventory.Count < maxInventorySize)
                {
                    Item newItem = Instantiate(itemScript);
                    inventory.Add(newItem);
                    newItem.gameObject.SetActive(false);
                    DontDestroyOnLoad(newItem);
                    itemScript.DestroyItem();
                }
            }
        }
    }

    public void EquipItem(Item item, int inventoryNumber, bool startingItem = false)
    {
        if (item.itemType == ItemType.Armor)
            EquipArmor(item as Armor);
        else if (item.itemType == ItemType.Weapon)
            EquipWeapon(item as Weapon);

        if (!startingItem) inventory.Remove(item);
    }

    private void EquipArmor(Armor armor)
    {
        Armor oldArmor = equippedArmor;
        equippedArmor = armor;
        defense += armor.defense;
        if (oldArmor != null)
        {
            defense -= oldArmor.defense;
            inventory.Add(oldArmor);
            DontDestroyOnLoad(oldArmor);
        }
        DontDestroyOnLoad(equippedArmor);
    }

    private void EquipWeapon(Weapon weapon)
    {
        Weapon oldWeapon = equippedWeapon;
        equippedWeapon = weapon;
        minDamage += weapon.damage / 2;
        maxDamage += weapon.damage;
        if (oldWeapon != null)
        {
            minDamage -= oldWeapon.damage / 2;
            maxDamage -= oldWeapon.damage;
            inventory.Add(oldWeapon);
            DontDestroyOnLoad(oldWeapon);
        }
        DontDestroyOnLoad(equippedWeapon);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = damage - Mathf.FloorToInt(defense * 1.15f);
        if (damageTaken <= 0) damageTaken = 0;
        currentHealth -= damageTaken;
        if (currentHealth <= 0)
        {
            targetPosition = rb.transform.position;
            isAlive = false;
            deathPanel?.SetActive(true);
        }
        healthBar?.UpdateBar((float)currentHealth, (float)maxHealth);

        damageText.text = damageTaken.ToString();
        StartCoroutine(ResetDamageText());
    }

    void TakePotion()
    {
        if (potionCount != 0 && currentHealth != maxHealth)
        {
            StartCoroutine(ResetPotion());
            takingPotion = true;
            potionCount -= 1;
            if (potionCount <= 0) potionCount = 0;
            int healAmount = (int)Math.Ceiling((double)potionHealAmount / 100 * maxHealth); //x% of max health
            currentHealth += healAmount;
            if (currentHealth >= maxHealth) currentHealth = maxHealth;
            if (potionText) potionText.text = potionCount.ToString();
            healthBar?.UpdateBar(currentHealth, maxHealth);
        }
    }

    public void RespawnPlayer()
    {
        print("respawning player");
        isRespawning = true;
        targetPosition = playerSpawn.transform.position;
        rb.transform.position = playerSpawn.transform.position;
        currentHealth = maxHealth;
        currentMana = maxMana;
        healthBar?.UpdateBar((float)currentHealth, (float)maxHealth);
        manaBar?.UpdateBar(currentMana, maxMana);
        deathPanel?.SetActive(false);
        isAlive = true;
        StartCoroutine(ResetRespawnTag());
    }

    public void AddXp(int newXp)
    {
        if (level < maxLevel)
        {
            if (xp + newXp >= xpNeededToLevel)
            {
                LevelUp(newXp);
            }
            else
            {
                GainXp(newXp);
            }

            xpBar?.UpdateBar(xp, xpNeededToLevel);
        }
        else if (level >= maxLevel) level = maxLevel;
    }

    private void LevelUp(int newXp)
    {
        level++;
        xp += newXp - xpNeededToLevel;
        float xpCurve = 1.25f;
        xpNeededToLevel = Mathf.RoundToInt(100 * Mathf.Pow(level, xpCurve));
        // xpNeededToLevel = Mathf.CeilToInt(1.5f * level) + 100;

        if (level == maxLevel)
        {
            xp = 0;
            xpNeededToLevel = 0;
        }

        if (levelUpText) levelUpText.enabled = true;
        if (levelText) levelText.text = level.ToString();
        minDamage += 2;
        maxDamage += 5;
        maxHealth += 25;
        maxMana += 25;
        manaPerHit += 5;
        currentHealth = maxHealth;
        currentMana = maxMana;
        ClampMana();
        healthBar?.UpdateBar(currentHealth, maxHealth);
        manaBar?.UpdateBar(currentMana, maxMana);
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

    public void SavePlayer()
    {
        CharacterData characterData = new CharacterData
        {
            playerName = playerName,
            lastVisitedLocation = lastVisitedLocation,
            level = level,
            xp = xp,
            xpNeededToLevel = xpNeededToLevel,
            potionCount = potionCount,
            maxHealth = maxHealth,
            maxMana = maxMana,
            minDamage = minDamage,
            maxDamage = maxDamage,
            criticalHitChance = criticalHitChance,
            criticalHitDamage = criticalHitDamage,
            defense = defense,
            currentHealth = currentHealth,
            currentMana = currentMana,
            manaPerHit = manaPerHit,
            gold = gold,
            completedStory = completedStory,
            visitedLocations = visitedLocations
        };
        foreach (var item in inventory)
        {
            ItemData newItem = new ItemData
            {
                name = item.name,
                type = item.itemType,
                gold = item.gold
            };

            if (item.itemType == ItemType.Weapon)
            {
                Weapon weapon = item as Weapon;
                newItem.stat = weapon.damage;
            }
            else if (item.itemType == ItemType.Armor)
            {
                Armor armor = item as Armor;
                newItem.stat = armor.defense;
            }

            characterData.inventory.Add(newItem);
        }

        ItemData armorData = new ItemData
        {
            name = equippedArmor.name,
            type = ItemType.Armor,
            stat = equippedArmor.defense,
            gold = equippedArmor.gold
        };

        ItemData weaponData = new ItemData
        {
            name = equippedWeapon.name,
            type = ItemType.Armor,
            stat = equippedWeapon.damage,
            gold = equippedWeapon.gold
        };

        characterData.equippedArmor = armorData;
        characterData.equippedWeapon = weaponData;

        PlayerData playerData = new PlayerData();
        playerData.LoadPlayerData();
        playerData.SavePlayerData(characterData);
    }

    public void LoadPlayer(string characterName)
    {
        print("loadPlayer getting called: " + characterName);
        inventory.Clear();
        equippedArmor = null;
        equippedWeapon = null;
        PlayerData playerData = new PlayerData();
        playerData.LoadPlayerData();
        CharacterData characterData = playerData.characters.Find(c => c.playerName == characterName);
        print(characterData);

        if (characterData != null)
        {
            visitedLocations.Clear();
            // Load the character's data
            // For example: level, experience, equipment, etc.
            playerName = characterData.playerName;
            level = characterData.level;
            xp = characterData.xp;
            xpNeededToLevel = characterData.xpNeededToLevel;
            potionCount = characterData.potionCount;
            maxHealth = characterData.maxHealth;
            maxMana = characterData.maxMana;
            minDamage = characterData.minDamage;
            maxDamage = characterData.maxDamage;
            criticalHitChance = characterData.criticalHitChance;
            criticalHitDamage = characterData.criticalHitDamage;
            defense = characterData.defense;
            currentHealth = characterData.currentHealth;
            currentMana = characterData.currentMana;
            manaPerHit = characterData.manaPerHit;
            gold = characterData.gold;
            completedStory = characterData.completedStory;
            visitedLocations = characterData.visitedLocations;

            if (level != 1)
            {
                int expectedManaPerHit = 25 + (5 * (level - 1));
                if (manaPerHit != expectedManaPerHit) manaPerHit = expectedManaPerHit;

                int expectedMaxHealth = 100 + (25 * (level - 1));
                if (maxHealth != expectedMaxHealth) maxHealth = expectedMaxHealth;


                float xpCurve = 1.25f;
                int expectedXpNeeded = Mathf.RoundToInt(100 * Mathf.Pow(level, xpCurve));
                if (level == maxLevel)
                {
                    xp = 0;
                    expectedXpNeeded = 0;
                }
                if (xpNeededToLevel != expectedXpNeeded) xpNeededToLevel = expectedXpNeeded;
            }

            if (visitedLocations.Contains("Town")) canSellItems = true;

            foreach (var itemData in characterData.inventory)
            {
                if (itemData.type == ItemType.Armor)
                {
                    GameObject armorObject = Instantiate(armorPrefab);
                    armorObject.SetActive(false);
                    DontDestroyOnLoad(armorObject);
                    Armor inventoryArmor = armorObject.GetComponent<Armor>();
                    inventoryArmor.name = itemData.name;
                    inventoryArmor.itemType = ItemType.Armor;
                    inventoryArmor.defense = itemData.stat;
                    inventoryArmor.gold = itemData.gold;
                    inventory.Add(inventoryArmor);
                }
                else if (itemData.type == ItemType.Weapon)
                {
                    GameObject weaponObject = Instantiate(weaponPrefab);
                    weaponObject.SetActive(false);
                    DontDestroyOnLoad(weaponObject);
                    Weapon iventoryWeapon = weaponObject.GetComponent<Weapon>();
                    iventoryWeapon.name = itemData.name;
                    iventoryWeapon.itemType = ItemType.Weapon;
                    iventoryWeapon.damage = itemData.stat;
                    iventoryWeapon.gold = itemData.gold;
                    inventory.Add(iventoryWeapon);
                }
            }

            GameObject equippedArmorObject = Instantiate(armorPrefab);
            equippedArmorObject.SetActive(false);
            DontDestroyOnLoad(equippedArmorObject);
            Armor armor = equippedArmorObject.GetComponent<Armor>();
            armor.name = characterData.equippedArmor.name;
            armor.itemType = ItemType.Armor;
            armor.defense = characterData.equippedArmor.stat;
            armor.gold = characterData.equippedArmor.gold;

            GameObject equippedWeaponObject = Instantiate(weaponPrefab);
            equippedWeaponObject.SetActive(false);
            DontDestroyOnLoad(equippedWeaponObject);
            Weapon weapon = equippedWeaponObject.GetComponent<Weapon>();
            weapon.name = characterData.equippedWeapon.name;
            weapon.itemType = ItemType.Weapon;
            weapon.damage = characterData.equippedWeapon.stat;
            weapon.gold = characterData.equippedWeapon.gold;

            equippedArmor = armor;
            equippedWeapon = weapon;
            xpBar?.UpdateBar(xp, xpNeededToLevel);
            healthBar?.UpdateBar(currentHealth, maxHealth);
            manaBar?.UpdateBar(currentMana, maxMana);
            if (levelText != null) levelText.text = level.ToString();
            if (potionText != null) potionText.text = potionCount.ToString();
            print(characterData.lastVisitedLocation);
            if (!string.IsNullOrEmpty(characterData.lastVisitedLocation) && SceneManager.GetActiveScene().name != characterData.lastVisitedLocation)
            {
                GameObject newLevelManager = Instantiate(levelManager);
                LevelManager manager = newLevelManager.GetComponent<LevelManager>();
                newLevelManager.SetActive(false);
                manager.nextLevelSceneName = characterData.lastVisitedLocation;
                manager.LoadNextLevel();
                // SceneManager.LoadScene(playerData.lastVisitedLocation);
            }
        }

        // SceneManager.LoadScene(playerData.lastVisitedLocation);
    }

    public void ResetPlayer()
    {
        foreach (Item item in inventory)
        {
            Destroy(item);
        }
        inventory.Clear();
        Destroy(equippedArmor);
        Destroy(equippedWeapon);
        equippedArmor = null;
        equippedWeapon = null;
        visitedLocations.Clear();
        completedStory = false;
        canSellItems = false;
        playerName = "";
        lastVisitedLocation = "Starting Tomb";
        level = 1;
        xp = 0;
        xpNeededToLevel = 100;
        potionCount = 0;
        maxHealth = 100;
        maxMana = 100;
        minDamage = 1;
        maxDamage = 5;
        criticalHitChance = 10;
        criticalHitDamage = 2;
        defense = 0;
        currentHealth = 100;
        currentMana = 100;
        manaPerHit = 15;
        gold = 0;
        characterNameText.text = "";
        sceneNameText.text = "";
        xpBar?.UpdateBar(xp, xpNeededToLevel);
        healthBar?.UpdateBar(currentHealth, maxHealth);
        manaBar?.UpdateBar(currentMana, maxMana);
        if (levelText != null) levelText.text = level.ToString();
        if (potionText != null) potionText.text = potionCount.ToString();
        playerHUD.SetActive(false);
        mainCamera.gameObject.SetActive(false);
        InitialGearSetup();
    }

    public void BeginSellReset()
    {
        StartCoroutine(ResetSellItems());
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

    private IEnumerator ResetPotion()
    {
        yield return new WaitForSecondsRealtime(1);
        takingPotion = false;
    }

    public IEnumerator ResetSellItems()
    {
        yield return new WaitForSecondsRealtime(60);
        canSellItems = true;
    }
}