using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject menuUI;
    public Button resumeButton;
    public Button exitButton;

    private bool isPaused = false;


    void Start()
    {
        resumeButton?.onClick.AddListener(ResumeGame);
        exitButton?.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        // Check for the Esc key press
        if (Input.GetKeyDown(KeyCode.Escape))
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
    }

    // Function to resume the game
    public void ResumeGame()
    {
        isPaused = false;
        menuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    // Function to exit the game
    public void ExitGame()
    {
        // Close the application
        Application.Quit();
    }
}