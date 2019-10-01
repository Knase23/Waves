using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class LevelGenerator : MonoBehaviour
{
    public Transform mapTransform;
    public Transform smallTransform;
    public Transform mediumTransform;
    public Transform largeTransform;

    public Vector2Int mapSize;

    

    //[Header("Perlin noise")]
    //public float noiseZoom;
    //public bool randomOffset = false;
    //public Vector2 perlinOffset;

    //public float levelScale;

    //[Serializable]
    //public struct Range2Float
    //{
    //    public float max;
    //    public float min;
    //}

    //public Range2Float largeRange;
    //public Range2Float mediumRange;
    //public Range2Float smallRange;

    [Header("Astriod prefabs")]
    public GameObject largeAstroidPrefabs;
    public GameObject mediumAstroidPrefabs;
    public GameObject smallAstroidPrefabs;

    [Header("Tolerance")]
    public int maxNumberOfFailedPlacements = 10;
    public int numberOfLargeSamples = 50;

    [Header("Debugging")]
    public int countOfConfirmed = 0;

    private List<GameObject> confirmedPlacements = new List<GameObject>();
    /// <summary>
    /// Generates a map of astiods with the given prefabs
    /// </summary>
    public void GenerateLevel()
    {
        // Use Poisson-disc distribution method do spawn in a level.
        ClearLevel();
        /* The function takes in: int numberOfSamples, Gameobject objectToPlace (That have a Asteriod script on it) ,int NumberOfRejections = 10 
         *  Randomize a postion, within the playing area
         *  Check if its a valid position.
         *      - The check sees there is no other confirmed samples within its area of placment Influence.
         *  If valid, its position is set. And that sample is confirmed.
         */
        countOfConfirmed = 0;
        SampleLevel(numberOfLargeSamples, largeAstroidPrefabs, largeTransform, maxNumberOfFailedPlacements);


        

        // All astriod can have a minimum distance from the borders by 1 unit
        // Asteriods have a set Radius, determined by their prefabs Asteriod scripts placement radius

        //Asteriods placement order

        // Large 
        /*
         * They have a set Radius, determined by the prefabs Asteriod scripts placement radius
         * There minimum distance from a Another Large Asteriod can be there Placement Radius + Tolerence 
         */


        // Medium
        /*
        * There minimum distance from a Large Asteriod can be there Placement Radius + LargeTolerence 
        * There minimum distance from a Medium Asteriod can be there Placement Radius + MediumTolerence
        */
        // Small
        /*
         * The Small must have atleast 1 Medium or Large asteriod near it
         * 
         * There minimum distance from a Large Asteriod can be there Placement Radius + LargeMinTolerence 
         * There maximum distance from a Large Asteriod can be there Placement Radius + LargeMaxTolerence 
         * 
         * There minimum distance from a Medium Asteriod can be there Placement Radius + MediumMinTolerence
         * There maximum distance from a Medium Asteriod can be there Placement Radius + MediumMaxTolerence
         * 
         * There minimum distance from a Small Asteriod can be there Placement Radius + Tolerence 
         */

        // All of them will be added in a list, that is then sent out to all clients, so they can replicate the scene!

    }
    public void VisualGenerateLevel()
    {
        ClearLevel();
        countOfConfirmed = 0;
        StartCoroutine(VisualRepresentationOfSampling(numberOfLargeSamples, largeAstroidPrefabs, largeTransform, maxNumberOfFailedPlacements));
    }
    public void SampleLevel(int numberOfSamples,GameObject preFab,Transform hieararcyHolder ,int numberOfRejections = 10)
    {
        Asteriod preFabAsteriod = preFab.GetComponent<Asteriod>();

        Vector3 position;// = new Vector3(UnityEngine.Random.Range(-mapSize.x, mapSize.x), 0 ,UnityEngine.Random.Range(-mapSize.y, mapSize.y));

        for (int i = 0; i < numberOfSamples; i++)
        {
            bool validPlacement = false;
            bool rejected = false;
            int countOfFailedPlacement = 0;
            do
            {
                position = new Vector3(UnityEngine.Random.Range(-mapSize.x, mapSize.x), 0, UnityEngine.Random.Range(-mapSize.y, mapSize.y));

                if (preFabAsteriod.ValidatePlacement(position))
                {
                    validPlacement = true;
                    continue;
                }

                if (countOfFailedPlacement >= numberOfRejections)
                {
                    validPlacement = true;
                    rejected = true;
                    continue;
                }
                countOfFailedPlacement++;

            } while (!validPlacement);

            if (rejected)
            {
                continue;
            }

            countOfConfirmed++;
            confirmedPlacements.Add(Instantiate(preFab,position,UnityEngine.Random.rotation,hieararcyHolder));
        }
        
    }

    public IEnumerator VisualRepresentationOfSampling(int numberOfSamples, GameObject preFab, Transform hieararcyHolder, int numberOfRejections = 10)
    {
        Asteriod preFabAsteriod = preFab.GetComponent<Asteriod>();

        Vector3 position;
        GameObject presentationObject = Instantiate(preFab);

        for (int i = 0; i < numberOfSamples; i++)
        {
            bool validPlacement = false;
            bool rejected = false;
            int countOfFailedPlacement = 0;
            do
            {
                position = new Vector3(UnityEngine.Random.Range(-mapSize.x, mapSize.x), 0, UnityEngine.Random.Range(-mapSize.y, mapSize.y));
                presentationObject.transform.position = position;

                //Here it tries to Validate it
                if (preFabAsteriod.ValidatePlacement(position))
                {
                    validPlacement = true;
                    continue;
                }

                if (countOfFailedPlacement >= numberOfRejections)
                {
                    validPlacement = true;
                    rejected = true;
                    continue;
                }
                countOfFailedPlacement++;
                yield return new WaitForSecondsRealtime(0.1f);

            } while (!validPlacement);


            //Make the presentation object green if it found a place to put spawn a asteriod on. 
            // Red if it rejected it-
            // Yellow when it is searching for a valid place
            if (rejected)
            {
                continue;
            }

            countOfConfirmed++;
            confirmedPlacements.Add(Instantiate(preFab, position, UnityEngine.Random.rotation, hieararcyHolder));
        }

        Destroy(presentationObject);

        yield break;
    }

    /// <summary>
    /// Clears all astriods inside the Astriod list. AKA makes the map empty
    /// </summary>
    public void ClearLevel()
    {
        List<GameObject> allChilds = GetAstriodsFromTransform(mapTransform);
        foreach (var child in allChilds)
        {
            DestroyImmediate(child);
        }
    }


    /// <summary>
    /// Recursive Function, go though all transforms children until a the tag matches and then adds it to the list
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="transform"></param>
    /// <param name="gameObjects"></param>
    private void AddGameObjectWithTagFormTransform(string tag, Transform transform, ref List<GameObject> gameObjects)
    {
        if (transform.tag == tag)
        {
            gameObjects.Add(transform.gameObject);

            return;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            AddGameObjectWithTagFormTransform(tag, transform.GetChild(i), ref gameObjects);
        }
    }

    private List<GameObject> GetAstriodsFromTransform(Transform transform)
    {
        List<GameObject> list = new List<GameObject>();
        // This is a Recursive Function, it will go though all gameobjects under the given transform and added them to the list
        AddGameObjectWithTagFormTransform("Asteriod", transform, ref list);
        return list;
    }


}
