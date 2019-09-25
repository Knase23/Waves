using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Amount of astroids")]
    public int astroidsToSpawn = 300;
    public int numberOfLarge = 10;
    public int numberOfMedium = 30;
    public Transform mapTransform;

    [Header("Astriod prefabs")]
    public GameObject largeAstroidPrefabs;
    public GameObject mediumAstroidPrefabs;
    public GameObject smallAstroidPrefabs;

    [Header("Tolerance")]
    public int maxNumberOfFailedPlacements = 10;

    public List<GameObject> astroids = new List<GameObject>();
    
    //Plans/Ideas/Questions
    /*  Astroids should spilt when enough damage is done.
     *  They will have a small velocity when they get hit.
     *  The smaller they are, the more damage.
     *  You can only be damaged by them when you or it have a greater impact velocity.
     *  // Problems that might be later:
     *  - If it goes online multiplayer, how should we sync all astorids states to all clients
     */


    /// <summary>
    /// Generates a map of astiods with the given prefabs
    /// </summary>
    public void GenerateLevel()
    {
        for (int i = 0; i < astroidsToSpawn; i++)
        {
            //Spawn a Astriod
            GameObject ob;
            if (i < numberOfLarge)
            {
                ob = Instantiate(largeAstroidPrefabs, mapTransform);

            }
            else if (i < numberOfMedium + numberOfLarge)
            {
                ob = Instantiate(mediumAstroidPrefabs, mapTransform);
            }
            else
            {
                ob = Instantiate(smallAstroidPrefabs, mapTransform);
            }
            Vector3 position = new Vector3(Random.Range(-95, 95), 0, Random.Range(-95, 95));

            //Find a spot to place the object
            bool notSafeSpot = true;

            //Makes sure so we don't get a Infinite Loop
            int numberOfFailedPlacements = 0;

            do
            {
                notSafeSpot = false;

                //Need some other rules for determineing if we are allowed to place the object there
                /* Example of rules
                 * - Large Astriod cannot be 5 or less units from each other
                 * - Medium Astriod can be placed only 2 -7 units from a large Astriod
                 * - Small Astriods can be placed any were but must have a Astriod 1-4 units from it
                 * - No Astriod should be place inside another astriod
                 */

                // Check if we hit a object on the random location 
                if (Physics.Raycast(position + (2 * Vector3.up), Vector3.down))
                {
                    position = new Vector3(Random.Range(-80, 80), 0, Random.Range(-80, 80));
                    notSafeSpot = true;
                    numberOfFailedPlacements++;
                }
                else
                {
                    numberOfFailedPlacements = 0;
                }

            } while (notSafeSpot || numberOfFailedPlacements > maxNumberOfFailedPlacements);

            if (numberOfFailedPlacements > maxNumberOfFailedPlacements)
            {
                Debug.Log("Could not find a place to place it, will place it on last random position");
            }
            //Place it
            ob.transform.position = position;
            astroids.Add(ob);
        }
    }

    /// <summary>
    /// Clears all astriods inside the Astriod list. AKA makes the map empty
    /// </summary>
    public void ClearLevel()
    {

        foreach (var astroid in astroids)
        {
            DestroyImmediate(astroid);
        }
        astroids.Clear();

        for (int i = 0; i < mapTransform.childCount; i++)
        {
            Transform child = mapTransform.GetChild(i);
            if (child.tag == "Astriod")
            {
                DestroyImmediate(child);
            }
        }
    }


}
