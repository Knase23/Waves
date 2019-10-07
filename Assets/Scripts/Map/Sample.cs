using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for Sample, inherent this class to setup up rules for objects you want to validate there placement.
/// </summary>
public abstract class Sample : ScriptableObject
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
            // Checks so the object in the search area hits some requirements for this object
            if (!Validation(position, listOfHits[i]))
            {
                return false;
            }

        }
        if (!PostValidation(position))
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


    public virtual bool CompareWithOtherSample(Vector3 position, Collider otherSample)
    {
        return true;
    }
}
