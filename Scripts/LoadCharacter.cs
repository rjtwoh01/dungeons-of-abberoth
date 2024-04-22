using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoadCharacter : MonoBehaviour, IPointerClickHandler
{
    CharacterData characterData;
    public Button playBtn;
    public Button noDeleteBtn;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public GameObject deleteCharacterCanvas;
    public GameObject deleteCharacterPanel;

    // Start is called before the first frame update
    void Start()
    {
        playBtn?.onClick.AddListener(LoadSelectedCharacter);
        noDeleteBtn?.onClick.AddListener(() =>
        {
            deleteCharacterCanvas.SetActive(false);
            deleteCharacterPanel.SetActive(false);
        });
        deleteCharacterPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (characterData == null) playBtn.interactable = false;
        else playBtn.interactable = true;

        if (Input.GetKeyDown(KeyCode.Escape) && deleteCharacterPanel.activeSelf)
        {
            deleteCharacterCanvas.SetActive(false);
            deleteCharacterPanel.SetActive(false);
        }
    }

    public void SetCharacterInfo(CharacterData character)
    {
        characterData = character;
        nameText.text = character.playerName;
        levelText.text = character.level.ToString();
    }

    public void LoadSelectedCharacter()
    {
        string characterName = characterData.playerName;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Player player = playerObject.GetComponent<Player>();
            if (player != null)
            {
                player.LoadPlayer(characterName);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            deleteCharacterCanvas.SetActive(true);
            deleteCharacterPanel.SetActive(true);
            deleteCharacterPanel.GetComponent<DeleteCharacter>().SetCharacterInfo(characterData);
        }
    }
}
