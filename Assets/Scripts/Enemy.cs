using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject target;
    public float speed = 2f;
    [SerializeField]
    private float range = 0.75f;
    private float currentDistance;
    [SerializeField]
    private float viewDistance = 5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentDistance = Vector2.Distance(transform.position, target.transform.position);

        if (currentDistance > range && currentDistance <= viewDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
    }
}
