using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyOrc;
    private bool spawn = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && spawn)
        {
            Instantiate<GameObject>(enemyOrc);
            Instantiate<GameObject>(enemyOrc);
            spawn = false;
        }
    }
}
