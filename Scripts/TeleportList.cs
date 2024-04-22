using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportList : MonoBehaviour
{
    public GameObject locationOption;
    private List<List<string>> locationChunks;
    public VerticalLayoutGroup layoutGroup;
    public Button prevBtn;
    public Button nextBtn;
    public GameObject playerObject;
    public Player player;
    private int currentChunkIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        prevBtn?.onClick.AddListener(ShowPrevChunk);
        nextBtn?.onClick.AddListener(ShowNextChunk);
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.GetComponent<Player>();
        prevBtn.interactable = false;
        nextBtn.interactable = false;
        PopulateList();
    }

    public void PopulateList()
    {
        if (playerObject == null || player == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null) player = playerObject.GetComponent<Player>();
        }
        if (locationChunks != null)
        {
            locationChunks.Clear();
            locationChunks = null;
        }
        ClearList();
        locationChunks = SplitLocations(player.visitedLocations, 5);
        currentChunkIndex = 0;
        LocationList();
    }

    List<List<string>> SplitLocations(List<string> locations, int chunkSize)
    {
        print("splitting locations");
        print(locations);
        List<List<string>> chunks = new List<List<string>>();
        for (int i = 0; i < locations.Count; i += chunkSize)
        {
            List<string> chunk = new List<string>();
            for (int j = i; j < Mathf.Min(i + chunkSize, locations.Count); j++)
            {
                chunk.Add(locations[j]);
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
            LocationList();
        }
    }

    void ShowNextChunk()
    {
        if (currentChunkIndex < locationChunks.Count - 1)
        {
            ClearList();
            currentChunkIndex++;
            LocationList();
        }
    }

    void UpdateNavigationButtons()
    {
        prevBtn.interactable = currentChunkIndex > 0;
        nextBtn.interactable = currentChunkIndex < locationChunks.Count - 1;
    }

    void LocationList()
    {
        if (locationOption == null)
        {
            Debug.LogError("Location prefab or Vertical Layout Group not set!");
            return;
        }
        else
        {
            List<string> currentChunk = locationChunks[currentChunkIndex];
            foreach (string loc in currentChunk)
            {

                GameObject newLocation = Instantiate(locationOption, layoutGroup.transform);
                newLocation.GetComponent<LocationOption>().SetLocation(loc);
            }
        }

        UpdateNavigationButtons();
    }
}
