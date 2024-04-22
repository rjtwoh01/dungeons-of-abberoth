using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryUI;
    public Button exitButton;
    public Button sellAllButton;
    private Player player;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public List<InventorySlot> equippedSlots = new List<InventorySlot>();
    public TextMeshProUGUI goldCount;
    public AudioClip moneySound;
    [SerializeField] private AudioSource audioSource;

    private bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<Player>();
        exitButton?.onClick.AddListener(CloseInventory);
        sellAllButton?.onClick.AddListener(SellAll);
        sellAllButton.interactable = player.canSellItems && player.inventory.Count > 0;
        CloseInventory();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen;
            player.inventoryOpen = isOpen;
            inventoryUI.SetActive(isOpen);
            if (goldCount) goldCount.text = player.gold.ToString();
            Setup();
            sellAllButton.interactable = player.canSellItems && player.inventory.Count > 0;
        }
    }

    void CloseInventory()
    {
        isOpen = false;
        player.inventoryOpen = isOpen;
        inventoryUI.SetActive(isOpen);
    }

    public void Setup()
    {
        //Initialize an empty inventory
        foreach (var slot in slots)
        {
            slot.SetEmpty();
            if (!isOpen) slot.isHovering = false;
        }
        //Place items from player's inventory into the GUI
        for (int i = 0; i < player.inventory.Count; i++)
        {
            slots[i].SetItem(player.inventory[i], i);
        }

        //Initialize an empty inventory
        foreach (var slot in equippedSlots)
        {
            slot.SetEmpty();
        }
        //Place items from player's inventory into the GUI
        // for (int i = 0; i < player.inventory.Count; i++)
        // {
        //     print("item going into slot " + i);
        //     print(player.inventory[i]);
        //     slots[i].SetItem(player.inventory[i], i);
        // }
        if (player.equippedArmor != null)
        {
            equippedSlots[0].SetItem(player.equippedArmor, 0, true);
        }
        if (player.equippedWeapon != null)
        {
            equippedSlots[1].SetItem(player.equippedWeapon, 0, true);
        }
    }

    public void SellAll()
    {
        player.canSellItems = false;
        sellAllButton.interactable = false;
        foreach (Item item in player.inventory)
        {
            // player.inventory.Remove(item);
            player.gold += item.gold;
            Destroy(item);
        }
        player.inventory.Clear();
        if (goldCount) goldCount.text = player.gold.ToString();
        audioSource.clip = moneySound;
        audioSource.Play();
        Setup();
        player.BeginSellReset();
    }
}
