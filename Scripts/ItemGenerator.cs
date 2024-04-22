using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public GameObject weaponPrefab;
    public GameObject armorPrefab;
    public string[] armorNames = { "Iron Armor", "Steel Armor", "Leather Armor", "Chainmail Armor", "Plate Armor", "Basic Armor" };
    public string[] weaponNames = { "Iron Sword", "Steel Sword", "Wooden Sword", "Basic Sword", "Dull Sword" };

    public List<Item> GenerateItems(int playerLevel, int itemAmount, bool bossLoot = false, int legendaryChance = 1, bool chestItem = false)
    {
        if (armorPrefab != null && weaponPrefab != null)
        {


            List<Item> itemList = new List<Item>();

            // Generate items with stats based on the player's level
            for (int i = 0; i < itemAmount; i++) // Generate 5 items for example
            {
                // string itemName = "Item " + i;
                int baseStat = Random.Range(1, 5); // Random base stat for variety
                int baseGold = Random.Range(1, 5);
                int itemRoll = Random.Range(1, 3);
                int legendaryRoll = Random.Range(1, 1001);
                bool legendary = false;
                if (legendaryRoll <= legendaryChance)
                {
                    int statMod = Random.Range(2, 6);
                    baseStat = baseStat * statMod;
                    baseGold = baseStat * statMod;
                    legendary = true;
                }
                if (bossLoot && Random.Range(1, 101) > 5)
                {
                    baseStat = baseStat * 2;
                    baseGold = baseGold * 2;
                }
                if (chestItem && Random.Range(1, 101) > 80)
                {
                    baseStat = baseStat * 2;
                    baseGold = baseGold * 2;
                }
                if (!chestItem && !bossLoot && Random.Range(1, 101) > 95)
                {
                    baseStat = baseStat * 2;
                    baseGold = baseGold * 2;
                }
                if (itemRoll == 1)
                {
                    GameObject weaponObject = Instantiate(weaponPrefab);
                    DontDestroyOnLoad(weaponObject);
                    weaponObject.SetActive(false);
                    Weapon weapon = weaponObject.GetComponent<Weapon>();
                    weapon.name = weaponNames[Random.Range(0, weaponNames.Length)];
                    if (legendary) weapon.name = string.Concat("Legendary ", weapon.name);
                    weapon.damage = Mathf.RoundToInt(baseStat * 0.75f * playerLevel);
                    weapon.gold = baseGold * playerLevel;
                    weapon.legendary = legendary;
                    itemList.Add(weapon);
                }
                else
                {
                    GameObject armorObject = Instantiate(armorPrefab);
                    DontDestroyOnLoad(armorObject);
                    armorObject.SetActive(false);
                    Armor armor = armorObject.GetComponent<Armor>();
                    armor.name = armorNames[Random.Range(0, armorNames.Length)];
                    if (legendary) armor.name = string.Concat("Legendary ", armor.name);
                    armor.defense = Mathf.RoundToInt(baseStat * 0.75f * playerLevel);
                    armor.gold = baseGold * playerLevel;
                    armor.legendary = legendary;
                    itemList.Add(armor);
                }

                // Item newItem = new Item(itemName, playerLevel, baseStat);
                // itemList.Add(newItem);
            }

            return itemList;
        }
        else
        {
            Debug.Log("weapon or armor prefab null");
            List<Item> itemList = new List<Item>();
            return itemList;
        }
    }
}
