using System;
using UnityEngine;
public static class Sampling
{
    public static void SampleGenerating(int numberOfActualDesiredObjects,Sample sampleRules, GameObject prefab, Vector3 maxPosition, Vector3 minPosition, Transform parent = null, float sampleMultiplier = 1.5f, int numberOfRejections = 10)
    {
        // Get the gameobjects Sample
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
                GameObject.Instantiate(prefab, randomPosition, UnityEngine.Random.rotation,parent);
            }
        }
    }
}