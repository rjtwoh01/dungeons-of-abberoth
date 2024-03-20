using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Potion
}

public abstract class Item : MonoBehaviour
{
    public string itemName;
    public int itemID;
    public ItemType itemType; // Add this property

    // Common properties and methods for all items can go here
    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}