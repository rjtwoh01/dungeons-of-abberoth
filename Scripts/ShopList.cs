using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ShopMode
{
    Buy,
    Sell
}

public class ShopList : MonoBehaviour
{
    public GameObject armorOption;
    public GameObject weaponOption;
    public GameObject armorPrefab;
    public GameObject weaponPrefab;
    public GameObject playerObject;
    public Player player;
    public VerticalLayoutGroup layoutGroup;
    public ShopMode shopMode;
    public Button buyModeBtn;
    public Button sellModeBtn;
    public Button prevBtn;
    public Button nextBtn;
    public Button refreshBtn;
    private List<List<Item>> inventoryChunks;
    public List<Item> vendorItems;
    private int currentChunkIndex = 0;
    public AudioClip moneySound;
    [SerializeField] private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        buyModeBtn?.onClick.AddListener(ToggleBuy);
        sellModeBtn?.onClick.AddListener(ToggleSell);
        prevBtn?.onClick.AddListener(ShowPrevChunk);
        nextBtn?.onClick.AddListener(ShowNextChunk);
        refreshBtn?.onClick.AddListener(RefreshList);
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.GetComponent<Player>();
        shopMode = ShopMode.Buy;
        prevBtn.interactable = false;
        nextBtn.interactable = false;
        refreshBtn.interactable = true;
        PopulateList();
    }

    void ToggleBuy()
    {
        if (shopMode == ShopMode.Sell)
        {
            shopMode = ShopMode.Buy;
            prevBtn.interactable = false;
            nextBtn.interactable = false;
            refreshBtn.interactable = true;
            PopulateList();
        }
    }

    void ToggleSell()
    {
        if (shopMode == ShopMode.Buy)
        {
            ClearList();
            shopMode = ShopMode.Sell;
            refreshBtn.interactable = false;
            PopulateList();
        }
    }

    public void PopulateList()
    {
        if (shopMode == ShopMode.Buy)
        {
            ClearList();
            if (inventoryChunks != null)
            {
                inventoryChunks.Clear();
                inventoryChunks = null;
            }
            if (vendorItems != null && vendorItems.Count == 0)
            {
                vendorItems = null;
            }
            if (playerObject == null)
            {
                playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null) player = playerObject.GetComponent<Player>();
            }
            if (vendorItems == null)
            {
                vendorItems = new List<Item>();
                GameObject itemGeneratorObject = new GameObject("gernator");
                itemGeneratorObject.AddComponent<ItemGenerator>();
                ItemGenerator itemGenerator = itemGeneratorObject.GetComponent<ItemGenerator>();
                if (itemGenerator != null)
                {
                    itemGenerator.armorPrefab = armorPrefab;
                    itemGenerator.weaponPrefab = weaponPrefab;
                    vendorItems = itemGenerator.GenerateItems(player.level, 5, false, 1, true);
                }
            }
            BuyList();
        }
        else
        {
            if (inventoryChunks != null)
            {
                inventoryChunks.Clear();
                inventoryChunks = null;
            }
            ClearList();
            inventoryChunks = SplitInventory(player.inventory, 5);
            currentChunkIndex = 0;
            SellList();
        }
    }

    public void RefreshList()
    {
        // foreach (Item item in vendorItems)
        // {
        //     Destroy(item);
        // }
        vendorItems = null;
        ClearList();
        PopulateList();
    }

    List<List<Item>> SplitInventory(List<Item> inventory, int chunkSize)
    {
        List<List<Item>> chunks = new List<List<Item>>();
        for (int i = 0; i < inventory.Count; i += chunkSize)
        {
            List<Item> chunk = new List<Item>();
            for (int j = i; j < Mathf.Min(i + chunkSize, inventory.Count); j++)
            {
                chunk.Add(inventory[j]);
            }
            chunks.Add(chunk);
        }
        return chunks;
    }

    void BuyList()
    {
        print("buying!!");
        if (armorOption == null || weaponOption == null || layoutGroup == null)
        {
            Debug.LogError("Character prefab or Vertical Layout Group not set!");
            return;
        }
        else
        {
            print("should populate inventory");
            foreach (Item item in vendorItems)
            {
                if (item.itemType == ItemType.Weapon)
                {
                    GameObject newWeapon = Instantiate(weaponOption, layoutGroup.transform);
                    newWeapon.GetComponent<WeaponOption>().SetItem(item as Weapon, ShopMode.Buy);
                }
                else
                {
                    GameObject newArmor = Instantiate(armorOption, layoutGroup.transform);
                    newArmor.GetComponent<ArmorOption>().SetItem(item as Armor, ShopMode.Buy);
                }
            }
        }
    }

    void ShowPrevChunk()
    {
        if (currentChunkIndex > 0)
        {
            ClearList();
            currentChunkIndex--;
            SellList();
        }
    }

    void ShowNextChunk()
    {
        if (currentChunkIndex < inventoryChunks.Count - 1)
        {
            ClearList();
            currentChunkIndex++;
            SellList();
        }
    }

    public void ClearList()
    {
        foreach (Transform child in layoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdateNavigationButtons()
    {
        prevBtn.interactable = currentChunkIndex > 0;
        nextBtn.interactable = currentChunkIndex < inventoryChunks.Count - 1;
    }

    public void SellList()
    {
        print("selling!");
        if (armorOption == null || weaponOption == null || layoutGroup == null)
        {
            Debug.LogError("Character prefab or Vertical Layout Group not set!");
            return;
        }
        else
        {
            print("should populate inventory");

            List<Item> currentChunk = inventoryChunks[currentChunkIndex];
            foreach (Item item in currentChunk)
            {
                if (item.itemType == ItemType.Weapon)
                {
                    GameObject newWeapon = Instantiate(weaponOption, layoutGroup.transform);
                    newWeapon.GetComponent<WeaponOption>().SetItem(item as Weapon, ShopMode.Sell);
                }
                else
                {
                    GameObject newArmor = Instantiate(armorOption, layoutGroup.transform);
                    newArmor.GetComponent<ArmorOption>().SetItem(item as Armor, ShopMode.Sell);
                }
            }
        }

        UpdateNavigationButtons();
    }

    public void PlaySound()
    {
        audioSource.clip = moneySound;
        audioSource.Play();
    }
}
