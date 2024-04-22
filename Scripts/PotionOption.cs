using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotionOption : MonoBehaviour
{
    public Button buyBtn;
    public TextMeshProUGUI btnTxt;
    public TextMeshProUGUI gold;
    public GameObject playerObject;
    public Player player;
    public int cost;
    public int amount;
    public AudioClip moneySound;
    [SerializeField] private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        buyBtn?.onClick.AddListener(BuyPotion);
        audioSource.clip = moneySound;
    }

    void Update()
    {
        if (player != null && cost > player.gold && buyBtn.interactable) buyBtn.interactable = false;
    }

    public void SetPotion(int count)
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<Player>();
        switch (count)
        {
            case 0:
                amount = 1;
                break;
            case 1:
                amount = 5;
                break;
            case 2:
                amount = 10;
                break;
            case 3:
                amount = 50;
                break;
            case 4:
                amount = 100;
                break;
            default:
                amount = 1;
                break;
        }

        SetCost();
    }

    void SetCost()
    {

        cost = 5 * player.level * amount;
        gold.text = cost.ToString();
        btnTxt.text = "Buy " + amount.ToString();
        if (cost > player.gold) buyBtn.interactable = false;
    }

    void BuyPotion()
    {
        player.potionCount += amount;
        player.gold -= cost;
        player.potionText.text = player.potionCount.ToString();
        audioSource.Play();
    }
}
