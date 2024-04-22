using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemTooltip : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI goldCount;
    public Item item;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.SetSiblingIndex(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetItem(Item newItem)
    {
        if (newItem != null)
        {
            print("setItem " + newItem);
            item = newItem;
            itemName.text = item.name.Split("(")[0];
            if (item.itemType == ItemType.Weapon)
            {
                Weapon weapon = item as Weapon;
                itemDescription.text = "Damage: " + weapon.damage;
            }
            else if (item.itemType == ItemType.Armor)
            {
                Armor armor = item as Armor;
                itemDescription.text = "Defense: " + armor.defense;
            }
            goldCount.text = newItem.gold.ToString();
        }
        else
        {
            item = null;
            itemName.text = "";
            itemDescription.text = "";
            goldCount.text = "";
        }
    }
}
