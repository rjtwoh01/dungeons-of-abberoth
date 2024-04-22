using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmorOption : MonoBehaviour
{
    public Armor armor;
    public TextMeshProUGUI armorName;
    public TextMeshProUGUI armorDescription;
    public TextMeshProUGUI gold;
    public Button barterBtn;
    public TextMeshProUGUI btnText;
    ShopMode shopMode;
    public GameObject shopListObject;
    public ShopList shopList;
    public GameObject playerObject;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {

        barterBtn?.onClick.AddListener(BarterItem);
        // btnText = barterBtn.GetComponent<TextMeshProUGUI>();
        shopListObject = GameObject.FindGameObjectWithTag("ShopList");
        shopList = shopListObject.GetComponent<ShopList>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetItem(Armor newArmor, ShopMode newType)
    {
        armor = newArmor;
        armorName.text = newArmor.name.Split("(")[0]; ;
        armorDescription.text = "Defense: " + newArmor.defense.ToString();
        gold.text = newArmor.gold.ToString();
        shopMode = newType;

        if (newType == ShopMode.Sell)
        {
            barterBtn.interactable = true;
            btnText.text = "Sell";
        }
        else
        {
            btnText.text = "Buy";
            playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject.GetComponent<Player>();
            if (player.gold >= newArmor.gold && player.inventory.Count < player.maxInventorySize) barterBtn.interactable = true;
            else barterBtn.interactable = false;
        }
    }

    public void BarterItem()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<Player>();
        shopList.PlaySound();

        if (shopMode == ShopMode.Buy)
        {
            player.gold -= armor.gold;
            armor.gold = Mathf.RoundToInt(armor.gold * 0.75f);
            player.inventory.Add(armor);
            shopList.vendorItems.Remove(armor);
            shopList.PopulateList();
        }
        else
        {
            player.gold += armor.gold;
            player.inventory.Remove(armor);
            Destroy(armor);
            shopList.PopulateList();
        }
    }
}
