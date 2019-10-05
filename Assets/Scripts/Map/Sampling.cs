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
                if(!sampleRules.PreValidation(randomPosition))
                {

                }

                if (sampleRules.ValidatePlacement(randomPosition))
                {
                    validPositionState = true;
                    continue;
                }

                if (!sampleRules.PostValidation(randomPosition))
                {

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
                GameObject.Instantiate(prefab, randomPosition, Quaternion.identity,parent);
            }
        }
    }
}
/// <summary>
/// Base class for Sample, inherent this class to setup up rules for objects you want to validate there placement.
/// </summary>
public abstract class Sample :ScriptableObject
{
    /// <summary>
    /// Radius of nono-zone
    /// </summary>
    [SerializeField]
    public float radius = 1;

    /// <summary>
    /// Radius of Search area for objects to compare with
    /// </summary>
    [SerializeField]
    public float searchRadius = 10;

    /// <summary>
    /// Layers on witch the SearchArea looks in
    /// </summary>
    [SerializeField]
    public LayerMask mask = 1 << 9;

    [SerializeField]
    public QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

    public bool ValidatePlacement(Vector3 position)
    {
        //Pre-validation
        if (!PreValidation(position))
        {
            return false;
        }
        //Check so it does not going to collide on a Collider
        if (Physics.CheckSphere(position, radius, mask, queryTriggerInteraction))
        {
            return false;
        }
        Collider[] listOfHits = Physics.OverlapSphere(position, searchRadius, mask, queryTriggerInteraction);
        for (int i = 0; i < listOfHits.Length; i++)
        {
            // Checks so the object in the search area hits some requirements
            if (!Validation(position, listOfHits[i]))
            {
                return false;
            }
        }
        if(!PostValidation(position))
        {
            return false;
        }
        // Placement is valid, you can place the Asteriod there
        return true;
    }
    /// <summary>
    /// Checks if the samples rules for its placement
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="collider">Other object found in search area</param>
    /// <returns>Returns true if everything passes</returns>
    public abstract bool Validation(Vector3 position, Collider collider);

    /// <summary>
    /// Setups or Checks any other rules before validation
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <returns>Returns true if everything passes</returns>
    public virtual bool PreValidation(Vector3 position)
    {
        return true;
    }
    /// <summary>
    /// Setups or Checks any other rules after validation
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <returns>Returns true if everything passes</returns>
    public virtual bool PostValidation(Vector3 position)
    {
        return true;
    }

    /// <summary>
    /// True if it is intersecting
    /// </summary>
    /// <param name="position"></param>
    /// <param name="other"></param>
    /// <param name="distanceThreshold"></param>
    /// <returns>Is it intersecting with that object</returns>
    public bool SpehereIntersecting(Vector3 position, Asteriod other, float distanceThreshold)
    {
        Vector3 vectorBetweenAsteriods = other.transform.position - position;
        float diffrence = Mathf.Abs(other.placementRules.radius + radius - vectorBetweenAsteriods.magnitude);
        if (diffrence >= distanceThreshold)
        {
            return false;
        }

        return true;
    }
}
