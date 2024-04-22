using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public Button closeShop;
    public TextMeshProUGUI playerGold;
    public GameObject playerObject;
    public Player player;
    private GameObject playerHUD;
    public GameObject exitLocatioObject;
    private Vector3 exitLocation;
    private int prevGoldAmount;
    // public Button openShop;


    // Start is called before the first frame update
    void Start()
    {
        closeShop?.onClick.AddListener(CloseShop);
        // openShop?.onClick.AddListener(OpenShop);
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.GetComponent<Player>();
        shopPanel.SetActive(false);
        playerHUD = GameObject.FindGameObjectWithTag("PlayerHUD");
        exitLocation = exitLocatioObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && shopPanel.activeSelf)
        {
            CloseShop();
        }

        if (shopPanel.activeSelf && player && playerGold && prevGoldAmount != player.gold)
        {
            prevGoldAmount = player.gold;
            playerGold.text = player.gold.ToString();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the collider is the player
        {
            OpenShop();
        }
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        player.shopOpen = false;
        playerHUD.SetActive(true);
        Time.timeScale = 1f; // Unpause the game
        player.MoveToLocation(exitLocation);
    }

    public void OpenShop()
    {
        print("shop clicked");
        shopPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        player.shopOpen = true;
        if (player && playerGold)
        {
            prevGoldAmount = player.gold;
            playerGold.text = player.gold.ToString();
        }
        playerHUD = GameObject.FindGameObjectWithTag("PlayerHUD");
        playerHUD.SetActive(false);
    }
}
