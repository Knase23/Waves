using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class Sampling
{

    // Use Poisson-disc distribution method do spawn in a level.
    /* The function takes in: int numberOfSamples, Gameobject objectToPlace (That have a Asteriod script on it) ,int NumberOfRejections = 10 
         *  Randomize a postion, within the playing area
         *  Check if its a valid position.
         *      - The check sees there is no other confirmed samples within its area of placment Influence.
         *  If valid, its position is set. And that sample is confirmed.
         */
    public static  List<GameObject> SampleGenerating(int numberOfActualDesiredObjects,Sample sampleRules, GameObject prefab, Vector3 maxPosition, Vector3 minPosition, Transform parent = null, float sampleMultiplier = 1.5f, int numberOfRejections = 10)
    {
        // Get the gameobjects Sample
        List<GameObject> result = new List<GameObject>();
        int numberOfSamples = (int)(numberOfActualDesiredObjects * sampleMultiplier);
        Vector3 randomPosition;
        int numberOfDesiredObjects = numberOfActualDesiredObjects;
        int countOfConfirmed = 0;

        for (int i = 0; i < numberOfSamples && countOfConfirmed < numberOfDesiredObjects; i++)
        {
            bool validPositionState = false;
            bool rejected = false;
            int countOfFailedPlacement = 0;
            do
            {
                // Doing this for readablity sake.
                float x = UnityEngine.Random.Range(minPosition.x, maxPosition.x);
                float y = UnityEngine.Random.Range(minPosition.y, maxPosition.y);
                float z = UnityEngine.Random.Range(minPosition.z, maxPosition.z);
                
                randomPosition = new Vector3(x,y,z);

                if (sampleRules.ValidatePlacement(randomPosition))
                {
                    validPositionState = true;
                    continue;
                }
                // Exit if it does not find a valid place
                if (countOfFailedPlacement >= numberOfRejections)
                {
                    rejected = true;
                    continue;
                }
                countOfFailedPlacement++;

            } while (!validPositionState && !rejected);

            if (rejected)
            {
                numberOfDesiredObjects--;
                continue;
            }

            //This if can be removed, but its here for understanding the code better;
            if (validPositionState)
            {
                //Spawn in Object
                countOfConfirmed++;
                result.Add(GameObject.Instantiate(prefab, randomPosition, UnityEngine.Random.rotation,parent));
            }
        }
        return result;
    }
}