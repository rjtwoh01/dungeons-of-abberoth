using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string nextLevelSceneName;
    public TextMeshProUGUI levelNameText;

    void Start()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (levelNameText) levelNameText.text = "To " + nextLevelSceneName + "...";
        if (nextLevelSceneName == "The Field" && !player.completedStory) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the collider is the player
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        // Check if the next level scene name is valid
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            Debug.LogError("Next level scene name is not set in LevelManager!");
        }
    }
}
