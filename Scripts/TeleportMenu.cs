using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportMenu : MonoBehaviour
{
    public GameObject teleportUI;
    public GameObject teloportListObject;
    public TeleportList teleportList;
    public Button exitButton;
    private Player player;
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        exitButton?.onClick.AddListener(CloseTeleport);
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<Player>();
        teleportList = teloportListObject.GetComponent<TeleportList>();
        isOpen = false;
        teleportUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.M))
        {
            isOpen = !isOpen;
            player.teleportOpen = isOpen;
            teleportUI.SetActive(isOpen);
            teleportList.PopulateList();
        }
    }

    void CloseTeleport()
    {
        isOpen = false;
        player.teleportOpen = isOpen;
        teleportUI.SetActive(isOpen);
    }
}
