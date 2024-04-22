using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LocationOption : MonoBehaviour
{
    public string teleportLocation;
    public TextMeshProUGUI locationName;
    public Button teleportBtn;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        teleportBtn?.onClick.AddListener(Teleport);
    }

    public void SetLocation(string location)
    {
        teleportLocation = location;
        locationName.text = location;
    }

    void Teleport()
    {
        if (!string.IsNullOrEmpty(teleportLocation))
        {
            GameObject teleportMenu = GameObject.FindGameObjectWithTag("TeleportMenu");
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null) player = playerObject.GetComponent<Player>();
            if (teleportMenu != null)
            {
                teleportMenu.SetActive(false);
                if (player) player.teleportOpen = false;
            }
            SceneManager.LoadScene(teleportLocation);
        }
    }
}
