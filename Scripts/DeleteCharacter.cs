using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCharacter : MonoBehaviour
{
    CharacterData characterData;
    public Button yesBtn;
    public CharacterList characterList;

    // Start is called before the first frame update
    void Start()
    {
        yesBtn?.onClick.AddListener(Delete);
        characterList = GameObject.FindGameObjectWithTag("CharacterList").GetComponent<CharacterList>();
    }

    public void SetCharacterInfo(CharacterData characterInfo)
    {
        characterData = characterInfo;
    }

    void Delete()
    {
        PlayerData playerData = new PlayerData();
        playerData.LoadPlayerData();
        playerData.DeleteCharacter(characterData);
        characterList.ResetCharacterList();
    }
}
