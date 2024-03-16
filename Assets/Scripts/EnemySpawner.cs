using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int numberOfEnemies = 1;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject enemyInstance = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            // Store references to the instantiated enemies if needed
            // For example, you can store them in an array
            // enemyInstances[i] = enemyInstance;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
