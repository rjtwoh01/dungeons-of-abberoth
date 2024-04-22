using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public GameObject potionPrefab;
    public GameObject weaponPrefab;
    public GameObject armorPrefab;
    public int numberOfEnemies = 10;
    public int numofPotions = 25;
    List<Enemy> enemiesList = new List<Enemy>();
    List<int> bossIndexList = new List<int>();
    public int numOfEnemiesRemaining; // Variable to track remaining enemies
    public int numOfBossesRemaining; // Variable to track remaining enemies
    public TextMeshProUGUI numOfEnemiesTxt;
    public TextMeshProUGUI numOfBossesTxt;
    public TextMeshProUGUI incomingTxt;
    public float distanceThreshold = 10f;


    // Start is called before the first frame update
    void Start()
    {
        SpawnRandomEnemies();
        SpawnRandomHealthPotions();
        incomingTxt.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CheckEnemyDistance()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        foreach (Enemy enemy in enemiesList)
        {
            if (Vector2.Distance(enemy.gameObject.transform.position, playerTransform.position) > distanceThreshold)
            {
                // Move the enemy to a point that is 'distanceThreshold' away from the player
                Vector2 direction = (enemy.gameObject.transform.position - playerTransform.position).normalized;
                Vector2 targetPosition = (Vector2)playerTransform.position + direction * distanceThreshold;
                enemy.gameObject.transform.position = targetPosition;

                // enemy.viewDistance = 500f;
            }
        }
    }

    void SpawnRandomEnemies()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogError("Enemy Prefabs list is empty! Please assign enemy prefabs.");
            return;
        }

        Collider2D spawnerCollider = GetComponent<Collider2D>();
        if (spawnerCollider == null)
        {
            Debug.LogError("No BoxCollider2D component found on the EnemySpawner GameObject.");
            return;
        }


        int numberOfBosses = Random.Range(1, 6);
        while (bossIndexList.Count < numberOfBosses)
        {
            int bossEnemy = Random.Range(0, numberOfEnemies);
            if (!bossIndexList.Contains(bossEnemy))
            {
                bossIndexList.Add(bossEnemy);
            }
        }
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(spawnerCollider.bounds.min.x, spawnerCollider.bounds.max.x), Random.Range(spawnerCollider.bounds.min.y, spawnerCollider.bounds.max.y));

            GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            GameObject instantiatedEnemy = Instantiate(randomEnemyPrefab, randomPosition, Quaternion.identity);

            Enemy enemy = instantiatedEnemy.GetComponent<Enemy>();
            enemy.scale = true;
            if (bossIndexList.Contains(i))
            {
                enemy.boss = true;
                instantiatedEnemy.transform.localScale = new Vector3(3f, 3f, 1f);
                if (randomEnemyPrefab.name.Split("(")[0] == "EnemySkeleton") instantiatedEnemy.transform.localScale = new Vector3(6f, 6f, 1f);
            }
            enemiesList.Add(enemy);
        }
        numOfEnemiesRemaining = numberOfEnemies;
        numOfBossesRemaining = bossIndexList.Count;

        numOfEnemiesTxt.text = numOfEnemiesRemaining.ToString();
        numOfBossesTxt.text = numOfBossesRemaining.ToString();
    }

    void SpawnRandomHealthPotions()
    {
        Collider2D spawnerCollider = GetComponent<Collider2D>();
        if (spawnerCollider == null)
        {
            Debug.LogError("No BoxCollider2D component found on the EnemySpawner GameObject.");
            return;
        }

        for (int i = 0; i < numofPotions; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(spawnerCollider.bounds.min.x, spawnerCollider.bounds.max.x), Random.Range(spawnerCollider.bounds.min.y, spawnerCollider.bounds.max.y));

            Instantiate(potionPrefab, randomPosition, Quaternion.identity);
        }
    }

    public void EnemyDestroyed(Enemy enemy)
    {
        if (enemiesList.Contains(enemy))
        {
            enemiesList.Remove(enemy);
            numOfEnemiesRemaining--;
            if (enemy.boss) numOfBossesRemaining--;

            if (numOfEnemiesRemaining == 20)
            {
                distanceThreshold = 20f;
                CheckEnemyDistance();
                incomingTxt.enabled = true;
                StartCoroutine(ResetIncomingText());
            }
            else if (numOfEnemiesRemaining == 10)
            {
                distanceThreshold = 10f;
                CheckEnemyDistance();
                incomingTxt.enabled = true;
                StartCoroutine(ResetIncomingText());
            }
            else if (numOfEnemiesRemaining == 5)
            {
                distanceThreshold = 5f;
                CheckEnemyDistance();
                incomingTxt.enabled = true;
                StartCoroutine(ResetIncomingText());
            }
            else if (numOfEnemiesRemaining == 0)
            {
                Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                int newXp = Mathf.CeilToInt(0.25f * player.xpNeededToLevel);
                player.AddXp(newXp);
                SpawnLoot();
            }

            numOfEnemiesTxt.text = numOfEnemiesRemaining.ToString();
            numOfBossesTxt.text = numOfBossesRemaining.ToString();
        }
    }

    public void SpawnLoot()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        int lootLevel = player.level;
        List<Item> lootItems = new List<Item>();
        GameObject itemGeneratorObject = new GameObject("fieldLootGenerator");
        itemGeneratorObject.AddComponent<ItemGenerator>();
        ItemGenerator itemGenerator = itemGeneratorObject.GetComponent<ItemGenerator>();
        itemGenerator.armorPrefab = armorPrefab;
        itemGenerator.weaponPrefab = weaponPrefab;
        int legendaryChance = 5;
        lootItems = itemGenerator.GenerateItems(lootLevel, 6, true, legendaryChance);
        foreach (Item item in lootItems)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * 1f;
            Vector3 randomPosition = player.gameObject.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
            if (item.itemType == ItemType.Weapon)
            {
                Weapon weapon = item as Weapon;
                GameObject weaponInstance = Instantiate(weaponPrefab, randomPosition, Quaternion.identity);
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
                Armor armor = item as Armor;
                GameObject ArmorInstance = Instantiate(armorPrefab, randomPosition, Quaternion.identity);
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

    private IEnumerator ResetIncomingText()
    {
        yield return new WaitForSecondsRealtime(5);
        if (incomingTxt) incomingTxt.enabled = false;
    }
}
