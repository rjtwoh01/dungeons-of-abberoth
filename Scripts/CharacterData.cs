using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public List<ItemData> inventory = new List<ItemData>();
    public string playerName;
    public ItemData equippedWeapon;
    public ItemData equippedArmor;
    public string lastVisitedLocation = "Starting Tomb";
    public int level = 1;
    public int xp = 0;
    public int xpNeededToLevel = 100;
    public int potionCount = 0;
    public int maxHealth = 100;
    public int maxMana = 100;
    public int minDamage = 100;
    public int maxDamage = 5;
    public int criticalHitChance = 10;
    public int criticalHitDamage = 2;
    public int defense = 0;
    public int currentHealth;
    public int currentMana;
    public int manaPerHit = 25;
    public int gold = 0;
    public bool completedStory = false;
    public List<string> visitedLocations = new List<string>();
}
