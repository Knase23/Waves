using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int numberOfAstroidsToSpawn = 300;
    public List<GameObject> astroids = new List<GameObject>();
    // Start is called before the first frame update
    public void GenerateLevel()
    {
        for (int i = 0; i < numberOfAstroidsToSpawn; i++)
        {

            // Create Astroids
            GameObject ob = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ob.transform.SetParent(transform);
            Vector3 position = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));

            //Raycast a bit above, and see if the box overlapps with other box.
            // If so, make the existing box bigger and place the other box somewhere else. 
            // Have a limit on how big a box can be, so it it does not take the whole level

            ob.transform.position = position;
            astroids.Add(ob);

            //Planning 
            /*  Astroids should spilt when enough damage is done.
             *  They will have a small velocity when they get hit.
             *  The smaller they are, the more damage.
             *  You can only be damaged by them when you or it have a greater impact velocity.
             *  // Problems that might be later:
             *  - If it goes online multiplayer, how should we sync all astorids states to all clients
             *  - Might go to unitys Entitiy Component System (ECS). Will need to test and reasearch it. 
             *      - If you find a good solution for the pulses in the ECS then it will need to be rebased into it
             */
        }
    }
    public void ClearLevel()
    {
        foreach (var astroid in astroids)
        {
            DestroyImmediate(astroid);
        }
        astroids.Clear();
    }

    
}
