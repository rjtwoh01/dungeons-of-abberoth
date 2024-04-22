using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionShopList : MonoBehaviour
{
    public GameObject potionOption;
    public GameObject playerObject;
    public Player player;
    public VerticalLayoutGroup layoutGroup;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.GetComponent<Player>();
        PopulateList();
    }

    public void PopulateList()
    {
        if (potionOption == null && layoutGroup == null)
        {
            Debug.LogError("Potion option or Vertical Layout Group not set!");
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            GameObject potion = Instantiate(potionOption, layoutGroup.transform);
            potion.GetComponent<PotionOption>().SetPotion(i);
        }
    }
}
