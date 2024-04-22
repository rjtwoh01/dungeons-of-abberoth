using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.TextCore.Text;
using System;

[System.Serializable]
public class PlayerData
{
    public List<CharacterData> characters = new List<CharacterData>();

    public void SavePlayerData(CharacterData character)
    {
        int index = characters.FindIndex(c => c.playerName == character.playerName);
        if (index != -1)
        {
            characters[index] = character;
        }
        else characters.Add(character);


        string json = JsonUtility.ToJson(this);
        string filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        // Save 'json' string to PlayerPrefs or a file
        // PlayerPrefs.SetString("PlayerData", json);
        try
        {
            // Write the JSON data to the file
            File.WriteAllText(filePath, json);
            Debug.Log("Character data saved to file: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving character data to file: " + e.Message);
        }
    }

    public void DeleteCharacter(CharacterData character)
    {
        int index = characters.FindIndex(c => c.playerName == character.playerName);
        if (index != -1)
        {
            Debug.Log("character in list");
            characters.RemoveAt(index);
        }
        else Debug.Log("character not in list");

        string json = JsonUtility.ToJson(this);
        string filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        // Save 'json' string to PlayerPrefs or a file
        // PlayerPrefs.SetString("PlayerData", json);
        try
        {
            // Write the JSON data to the file
            File.WriteAllText(filePath, json);
            Debug.Log("Character data saved to file: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving character data to file: " + e.Message);
        }
    }

    public void LoadPlayerData()
    {
        // Specify the file path in the user's Documents directory
        string filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");

        if (File.Exists(filePath))
        {
            try
            {
                // Read the JSON data from the file
                string json = File.ReadAllText(filePath);

                // Deserialize the JSON data into player data object
                JsonUtility.FromJsonOverwrite(json, this);

                // Find the character by name
                // CharacterData character = playerData.characters.Find(c => c.name == characterName);

                // if (character != null)
                // {
                //     // Load the character's data
                //     // For example: level, experience, equipment, etc.
                // }
                // else
                // {
                //     Debug.LogWarning("Character not found: " + characterName);
                // }
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading character data from file: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Player data file not found at path: " + filePath);
        }
    }


    // public void LoadPlayerData()
    // {
    //     string json = PlayerPrefs.GetString("PlayerData");
    //     JsonUtility.FromJsonOverwrite(json, this);
    // }
}