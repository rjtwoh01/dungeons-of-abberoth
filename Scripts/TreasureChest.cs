using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TreasureChest : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject armorPrefab;
    public GameObject weaponPrefab;
    private Tilemap floorTilemap;
    private Tilemap objectsTilemap;
    public Player player;
    public bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<Player>();
        floorTilemap = GameObject.FindGameObjectWithTag("FloorMap").GetComponent<Tilemap>();
        objectsTilemap = GameObject.FindGameObjectWithTag("ObjectMap").GetComponent<Tilemap>();
    }

    public void OpenChest()
    {
        if (!isOpen)
        {
            int tileCount = floorTilemap.GetUsedTilesCount();
            BoundsInt bounds = floorTilemap.cellBounds;
            TileBase[] allTiles = floorTilemap.GetTilesBlock(bounds);
            isOpen = true;

            int lootLevel = player.level;
            List<Item> lootItems = new List<Item>();
            GameObject itemGeneratorObject = new GameObject("fieldLootGenerator");
            itemGeneratorObject.AddComponent<ItemGenerator>();
            ItemGenerator itemGenerator = itemGeneratorObject.GetComponent<ItemGenerator>();

            itemGenerator.armorPrefab = armorPrefab;
            itemGenerator.weaponPrefab = weaponPrefab;
            int legendaryChance = 2;
            int numOfItems = Random.Range(1, 6);

            lootItems = itemGenerator.GenerateItems(lootLevel, numOfItems, false, legendaryChance, true);

            foreach (Item item in lootItems)
            {
                Vector2 randomOffset = Random.insideUnitCircle.normalized * 1.2f;
                Vector3 randomPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
                bool spotFound = false;
                while (!spotFound)
                {
                    randomOffset = Random.insideUnitCircle.normalized * 1.2f;
                    randomPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
                    Vector3Int tilePosInObjectsMap = objectsTilemap.WorldToCell(randomPosition);
                    TileBase objectTile = objectsTilemap.GetTile(tilePosInObjectsMap);
                    Vector3Int tilePosInFloorMap = floorTilemap.WorldToCell(randomPosition);
                    TileBase floorTile = floorTilemap.GetTile(tilePosInFloorMap);
                    if (objectTile == null && floorTile != null) spotFound = true;
                }

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

            Destroy(gameObject);
        }
    }
}
