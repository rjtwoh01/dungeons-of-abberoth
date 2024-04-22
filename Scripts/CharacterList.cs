using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterList : MonoBehaviour
{
    public GameObject characterPrefab;
    public PlayerData playerData;
    public List<CharacterData> characterDataList;
    public VerticalLayoutGroup layoutGroup;
    public Button prevBtn;
    public Button nextBtn;
    private List<List<CharacterData>> characterChunks;
    private int currentChunkIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        prevBtn?.onClick.AddListener(ShowPrevChunk);
        nextBtn?.onClick.AddListener(ShowNextChunk);
        playerData = new PlayerData();
        playerData.LoadPlayerData();
        characterDataList = playerData.characters;
        // PopuplateCharacterList();
        prevBtn.interactable = false;
        nextBtn.interactable = false;
        PopulateList();
    }

    public void PopulateList()
    {
        if (characterChunks != null)
        {
            characterChunks.Clear();
            characterChunks = null;
        }
        ClearList();
        characterChunks = SplitCharacters(characterDataList, 5);
        currentChunkIndex = 0;
        PopuplateCharacterList();
    }

    List<List<CharacterData>> SplitCharacters(List<CharacterData> characterData, int chunkSize)
    {
        List<List<CharacterData>> chunks = new List<List<CharacterData>>();
        for (int i = 0; i < characterData.Count; i += chunkSize)
        {
            List<CharacterData> chunk = new List<CharacterData>();
            for (int j = i; j < Mathf.Min(i + chunkSize, characterData.Count); j++)
            {
                chunk.Add(characterData[j]);
            }
            chunks.Add(chunk);
        }
        return chunks;
    }

    public void ClearList()
    {
        foreach (Transform child in layoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void ShowPrevChunk()
    {
        if (currentChunkIndex > 0)
        {
            ClearList();
            currentChunkIndex--;
            PopuplateCharacterList();
        }
    }

    void ShowNextChunk()
    {
        if (currentChunkIndex < characterChunks.Count - 1)
        {
            ClearList();
            currentChunkIndex++;
            PopuplateCharacterList();
        }
    }

    void UpdateNavigationButtons()
    {
        prevBtn.interactable = currentChunkIndex > 0;
        nextBtn.interactable = currentChunkIndex < characterChunks.Count - 1;
    }

    void PopuplateCharacterList()
    {
        if (characterPrefab == null || layoutGroup == null)
        {
            Debug.LogError("Character prefab or Vertical Layout Group not set!");
            return;
        }
        List<CharacterData> currentChunk = characterChunks[currentChunkIndex];
        foreach (CharacterData character in currentChunk)
        {
            GameObject newCharacter = Instantiate(characterPrefab, layoutGroup.transform);
            newCharacter.GetComponent<LoadCharacter>().SetCharacterInfo(character);
        }
        UpdateNavigationButtons();
    }

    public void ResetCharacterList()
    {
        playerData.LoadPlayerData();
        characterDataList = playerData.characters;
        PopulateList();
    }
}
