using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject menuUI;
    public Button resumeButton;
    public Button exitButton;
    public Button saveButton;
    public Button loadButton;
    public Button newCharacterButton;
    public Button closeCreateCharacterBtn;
    public Button closeSelectCharacterBtn;
    public Button selectCharacterBtn;
    public Button mainMenuButton;
    public Button infoButton;
    public Button infoExitBtn;
    public Player player;
    GameObject playerObject;
    public bool mainMenu = false;
    public PlayerData playerData;
    public GameObject characterSelectPanel;
    public GameObject createCharacterPanel;
    public GameObject playerPrefab;
    public GameObject infoPanel;
    private bool isPaused = false;
    private bool infoOpen = false;


    void Start()
    {
        resumeButton?.onClick.AddListener(ResumeGame);
        exitButton?.onClick.AddListener(ExitGame);
        saveButton?.onClick.AddListener(SaveGame);
        loadButton?.onClick.AddListener(LoadGame);
        newCharacterButton?.onClick.AddListener(NewCharacter);
        closeCreateCharacterBtn?.onClick.AddListener(CloseCreateCharacter);
        closeSelectCharacterBtn?.onClick.AddListener(CloseSelectCharacter);
        selectCharacterBtn?.onClick.AddListener(SelectCharacter);
        mainMenuButton?.onClick.AddListener(LoadMainMenu);
        infoButton?.onClick.AddListener(ToggleInfo);
        infoExitBtn?.onClick.AddListener(ToggleInfo);

        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<Player>();
        }

        if (mainMenu)
        {
            playerData = new PlayerData();
            playerData.LoadPlayerData();
        }
    }

    void Update()
    {
        // Check for the Esc key press
        if (Input.GetKeyDown(KeyCode.Escape) && !mainMenu && !player.shopOpen && !player.characterInfoOpen)
        {
            // Toggle the pause state
            isPaused = !isPaused;

            // Activate/deactivate the menu UI
            menuUI.SetActive(isPaused);

            // Pause/unpause the game
            if (isPaused)
            {
                Time.timeScale = 0f; // Pause the game

                // Adjust the pause menu size to match the screen size
                RectTransform rectTransform = menuUI.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                Time.timeScale = 1f; // Unpause the game
            }
        }

        if (player && player.initializing && loadButton && loadButton.IsInteractable())
        {
            loadButton.interactable = false;
        }
        else if (player && !player.initializing && loadButton && !loadButton.IsInteractable())
        {
            loadButton.interactable = true;
        }

        if (Input.GetKey(KeyCode.Escape) && mainMenu)
        {
            createCharacterPanel.SetActive(false);
            characterSelectPanel.SetActive(false);
            infoPanel.SetActive(false);
            infoOpen = false;
        }
    }

    public void NewCharacter()
    {
        // SceneManager.LoadScene("StartingTomb");
        createCharacterPanel.SetActive(true);
    }

    public void CloseCreateCharacter()
    {
        createCharacterPanel.SetActive(false);
    }

    // Function to resume the game
    public void ResumeGame()
    {
        isPaused = false;
        menuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SaveGame()
    {
        if (player != null)
        {
            player.SavePlayer();
        }
    }

    public void LoadGame()
    {
        if (player != null)
        {
            player.LoadPlayer(player.name);
            ResumeGame();
        }
    }

    public void LoadMainMenu()
    {
        player.ResetPlayer();
        ResumeGame();
        SceneManager.LoadScene("MainMenu");
        // createCharacterPanel.SetActive(true);
    }

    public void SelectCharacter()
    {
        characterSelectPanel.SetActive(true);
    }

    public void CloseSelectCharacter()
    {
        characterSelectPanel.SetActive(false);
    }

    public void ToggleInfo()
    {
        infoOpen = !infoOpen;
        infoPanel.SetActive(infoOpen);
    }

    // Function to exit the game
    public void ExitGame()
    {
        // Close the application
        Application.Quit();
    }
}