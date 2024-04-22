using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponOption : MonoBehaviour
{
    public Weapon weapon;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI weaponDescription;
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

    public void SetItem(Weapon newWeapon, ShopMode newType)
    {
        weapon = newWeapon;
        weaponName.text = newWeapon.name.Split("(")[0];
        weaponDescription.text = "Damage: " + newWeapon.damage.ToString();
        gold.text = newWeapon.gold.ToString();
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
            if (player.gold >= newWeapon.gold && player.inventory.Count < player.maxInventorySize) barterBtn.interactable = true;
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
            player.gold -= weapon.gold;
            weapon.gold = Mathf.RoundToInt(weapon.gold * 0.75f);
            player.inventory.Add(weapon);
            shopList.vendorItems.Remove(weapon);
            shopList.PopulateList();
        }
        else
        {
            player.gold += weapon.gold;
            player.inventory.Remove(weapon);
            Destroy(weapon);
            shopList.PopulateList();
        }

    }
}
