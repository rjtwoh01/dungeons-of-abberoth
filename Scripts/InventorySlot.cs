using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemIcon;
    public Sprite armorIcon;
    public Sprite weaponIcon;

    private Item slotItem;
    private int slotNumber;

    private Player player;
    ItemTooltip tooltipScript;

    private bool equippedSlot = false;

    // [SerializeField] private GameObject tooltipPrefab;
    public GameObject itemTooltip = null;
    // [SerializeField] private Vector3 tooltipOffset;

    public bool isHovering = false;

    private InventoryUI inventoryController;

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (itemTooltip) tooltipScript = itemTooltip.GetComponent<ItemTooltip>();
        itemTooltip.SetActive(false);
        player = playerObject.GetComponent<Player>();
        inventoryController = FindAnyObjectByType<InventoryUI>();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (isHovering)
    //     {
    //         UpdateTooltipPosition();
    //     }
    // }

    public void SetItem(Item item, int inventoryNumber = 0, bool equipped = false)
    {
        if (item != null)
        {
            if (item.itemType == ItemType.Armor)
            {
                itemIcon.sprite = armorIcon;
            }
            else if (item.itemType == ItemType.Weapon)
            {
                itemIcon.sprite = weaponIcon;
            }
            itemIcon.color = new Color(1, 1, 1, 1);

            slotItem = item;

            slotNumber = inventoryNumber;
            equippedSlot = equipped;

            // if (tooltipPrefab != null)
            // {
            //     Vector3 offset = new Vector3(-3, 0, 0);
            // itemTooltip = Instantiate(tooltipPrefab, transform.position + offset, Quaternion.identity);
            // ItemTooltip tooltipScript = itemTooltip.GetComponent<ItemTooltip>();
            if (tooltipScript != null)
            {
                tooltipScript.SetItem(item);
            }
            // }
        }
    }

    public void SetEmpty()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        slotItem = null;
        if (tooltipScript) tooltipScript.SetItem(null);
        if (itemTooltip) itemTooltip.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2 || eventData.button == PointerEventData.InputButton.Right) // Check for double click or right click
        {
            if (slotItem != null && !equippedSlot)
            {
                print("Double-clicked on slot with item: " + slotItem.itemType);
                player.EquipItem(slotItem, slotNumber);
                if (inventoryController != null)
                {
                    inventoryController.Setup();
                }
            }
            else
            {
                print("Double-clicked on empty slot.");
                // Handle the action for double-clicked empty slot
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("hovering " + slotItem + " " + itemTooltip);
        if (slotItem != null && itemTooltip != null)
        {
            if (tooltipScript) SetItem(slotItem);
            isHovering = true;
            itemTooltip.SetActive(true);
            UpdateTooltipPosition();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (itemTooltip != null)
        {
            itemTooltip.SetActive(false);
        }
    }

    private void UpdateTooltipPosition()
    {
        if (itemTooltip != null)
        {
            // Vector3 offset = new Vector3(-3, 0, 0);
            // itemTooltip.transform.position = transform.position + offset;
        }
    }
}
