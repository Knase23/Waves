using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerOfShips : MonoBehaviour
{
    public int numberOfShips = 0;
    public GameObject shipPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 rand = new Vector3(Random.Range(-75, 75), 0, Random.Range(-40, 40));

            Instantiate(shipPrefab, rand, transform.rotation);
            numberOfShips++;
        }
    }
}
