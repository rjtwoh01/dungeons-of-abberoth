using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    public GameObject characterInfoUI;
    public GameObject playerObject;
    public Player player;
    public Button exitBtn;
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI healthTxt;
    public TextMeshProUGUI maxHealthTxt;
    public TextMeshProUGUI minDamageTxt;
    public TextMeshProUGUI maxDamageTxt;
    public TextMeshProUGUI defenseTxt;
    public TextMeshProUGUI currentManaTxt;
    public TextMeshProUGUI maxManaTxt;
    public TextMeshProUGUI goldTxt;
    public TextMeshProUGUI xpTxt;
    public TextMeshProUGUI xpNeededTxt;
    public TextMeshProUGUI levelTxt;

    private bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<Player>();
        exitBtn?.onClick.AddListener(CloseInfo);
        CloseInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isOpen = !isOpen;
            player.characterInfoOpen = isOpen;
            characterInfoUI.SetActive(isOpen);
            if (nameTxt) nameTxt.text = player.playerName.ToString();
            if (healthTxt) healthTxt.text = player.currentHealth.ToString();
            if (maxHealthTxt) maxHealthTxt.text = player.maxHealth.ToString();
            if (minDamageTxt) minDamageTxt.text = player.minDamage.ToString();
            if (maxDamageTxt) maxDamageTxt.text = player.maxDamage.ToString();
            if (defenseTxt) defenseTxt.text = player.defense.ToString();
            if (currentManaTxt) currentManaTxt.text = player.currentMana.ToString();
            if (maxManaTxt) maxManaTxt.text = player.maxMana.ToString();
            if (goldTxt) goldTxt.text = player.gold.ToString();
            if (xpTxt) xpTxt.text = player.xp.ToString();
            if (xpNeededTxt) xpNeededTxt.text = player.xpNeededToLevel.ToString();
            if (levelTxt) levelTxt.text = player.level.ToString();
        }

        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInfo();
        }
    }

    void CloseInfo()
    {
        isOpen = false;
        characterInfoUI.SetActive(isOpen);
        player.characterInfoOpen = isOpen;
    }
}
