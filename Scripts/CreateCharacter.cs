using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateCharacter : MonoBehaviour
{
    public GameObject nameObject;
    public TMP_InputField nameText;
    public Button playBtn;

    // Start is called before the first frame update
    void Start()
    {
        playBtn?.onClick.AddListener(NewCharacter);
        nameText = nameObject.GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (nameText.text == "" || nameText.text == null) playBtn.interactable = false;
        else playBtn.interactable = true;
    }

    void NewCharacter()
    {
        string characterName = nameText.text;
        PlayerData playerData = new PlayerData();
        playerData.LoadPlayerData();
        int index = playerData.characters.FindIndex(c => c.playerName == characterName);
        characterName = validatedName(playerData, characterName);

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Player player = playerObject.GetComponent<Player>();
            if (player != null)
            {
                player.playerName = characterName;
                SceneManager.LoadScene("Starting Tomb");
            }
        }
    }

    string validatedName(PlayerData playerData, string characterName)
    {
        string finalName = characterName;
        int num = 0;
        int index = playerData.characters.FindIndex(c => c.playerName == finalName);
        while (index != -1)
        {
            num += 1;
            finalName = characterName.Split("-")[0];
            finalName = finalName += "-" + num;
            index = playerData.characters.FindIndex(c => c.playerName == finalName);
        }

        return finalName;
    }
}
