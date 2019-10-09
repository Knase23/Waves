using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LevelGenerator))]
public class VisualLevelGenerator : MonoBehaviour
{
    public bool ShowPresentationSphere = true;
    public float presentationSphereSize = 5;
    private Vector3 presentationPosition = Vector3.zero;
    private Vector3 presentationOtherComparisonPosition = Vector3.zero;
    private Color presentationColor = Color.white;
    private Color presentationComparisonColor = Color.white;
    public bool displayPrenentation = true;

    Coroutine generateCoroutine = null;
    Coroutine coroutine = null;
    LevelGenerator generator;

    private void Start()
    {
        generator = GetComponent<LevelGenerator>();
        displayPrenentation = false;
    }
    public void GenerateLevel()
    {
        generator.ClearLevel();
        
        if (generateCoroutine == null)
        {
            generateCoroutine = StartCoroutine(StartGenerate());
        }
    }
    public IEnumerator StartGenerate()
    {
        //Large
        coroutine = StartCoroutine(VisualRepresentationOfSampling(generator.numberOfLargeWanted, generator.largeAstroidPrefabs.GetComponent<Asteriod>().placementRules, generator.largeAstroidPrefabs, generator.maxPosition, generator.minPosition, generator.largeTransform, numberOfRejections: generator.maxNumberOfFailedPlacements));
        yield return new WaitUntil(() => coroutine == null);
        //Medium
        coroutine = StartCoroutine(VisualRepresentationOfSampling(generator.numberOfMediumWanted, generator.mediumAstroidPrefabs.GetComponent<Asteriod>().placementRules, generator.mediumAstroidPrefabs, generator.maxPosition, generator.minPosition, generator.mediumTransform, numberOfRejections: generator.maxNumberOfFailedPlacements));
        yield return new WaitUntil(() => coroutine == null);
        //Small
        coroutine = StartCoroutine(VisualRepresentationOfSampling(generator.numberOfSmallWanted, generator.smallAstroidPrefabs.GetComponent<Asteriod>().placementRules, generator.smallAstroidPrefabs, generator.maxPosition, generator.minPosition, generator.smallTransform, numberOfRejections: generator.maxNumberOfFailedPlacements));
        yield return new WaitUntil(() => coroutine == null);
        Debug.Log("Level Generation Complete");
        generateCoroutine = null;
        yield break;
    }
    #region SAMPLING : Showing What the code does Visualy in the Editor;
    public IEnumerator VisualRepresentationOfSampling(int numberOfActualDesiredObjects, Sample sampleRules, GameObject prefab, Vector3 maxPosition, Vector3 minPosition, Transform parent = null, float sampleMultiplier = 1.5f, int numberOfRejections = 10)
    {
        displayPrenentation = true;
        Asteriod preFabAsteriod = prefab.GetComponent<Asteriod>();
        presentationSphereSize = preFabAsteriod.placementRules.radius;
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

            presentationColor = Color.yellow;

            do
            {
                // Doing this for readablity sake.
                float x = UnityEngine.Random.Range(minPosition.x, maxPosition.x);
                float y = UnityEngine.Random.Range(minPosition.y, maxPosition.y);
                float z = UnityEngine.Random.Range(minPosition.z, maxPosition.z);

                randomPosition = new Vector3(x, y, z);
                presentationPosition = randomPosition;
                presentationOtherComparisonPosition = randomPosition;
                // The For Loop of the SearhRadius

                // Placement is valid, you can place the Asteriod there
                bool valid = true;
                //Pre-validation
                if (sampleRules.PreValidation(randomPosition))
                {
                    //Check so it does not going to collide on a Collider
                    if (!Physics.CheckSphere(randomPosition, sampleRules.radius, sampleRules.mask, sampleRules.queryTriggerInteraction))
                    {
                        Collider[] listOfHits = Physics.OverlapSphere(randomPosition, sampleRules.searchRadius, sampleRules.mask, sampleRules.queryTriggerInteraction);
                        for (int j = 0; j < listOfHits.Length; j++)
                        {
                            presentationComparisonColor = Color.yellow;
                            presentationOtherComparisonPosition = listOfHits[j].transform.position;
                            yield return new WaitForSecondsRealtime(2f / numberOfSamples);
                            // Checks so the object in the search area hits some requirements
                            if (!sampleRules.Validation(randomPosition, listOfHits[j]))
                            {
                                valid = false;
                                presentationComparisonColor = Color.red;
                                yield return new WaitForSecondsRealtime(2f / numberOfSamples);
                                break;
                            }
                            presentationComparisonColor = Color.green;
                            yield return new WaitForSecondsRealtime(2f / numberOfSamples);
                        }
                        if (!sampleRules.PostValidation(randomPosition))
                        {
                            valid = false;
                        }
                    }
                    else
                    {
                        valid = false;
                    }
                }
                else
                {
                    valid = false;
                }

                if (valid)
                {
                    validPositionState = true;
                    presentationColor = Color.green;
                    continue;
                }



                // End ot the Loop For Searh Radius
                // Exit if it does not find a valid place
                if (countOfFailedPlacement >= numberOfRejections)
                {
                    rejected = true;
                    presentationColor = Color.red;
                    continue;
                }
                countOfFailedPlacement++;
                yield return new WaitForSecondsRealtime(1f / numberOfSamples);
            } while (!validPositionState && !rejected);

            if (rejected)
            {
                numberOfDesiredObjects--;
                yield return new WaitForSecondsRealtime(2f / numberOfSamples);
                continue;
            }

            //This if can be removed, but its here for understanding the code better;
            if (validPositionState)
            {
                //Spawn in Object
                yield return new WaitForSecondsRealtime(3f / numberOfSamples);
                GameObject.Instantiate(prefab, randomPosition, UnityEngine.Random.rotation, parent);
            }
        }
        displayPrenentation = false;
        coroutine = null;
        yield break;
    }
    #endregion
    private void OnDrawGizmos()
    {
        if (ShowPresentationSphere && displayPrenentation)
        {
            Gizmos.color = presentationColor;
            Gizmos.DrawSphere(presentationPosition, presentationSphereSize > 2 ? presentationSphereSize:2);
            Gizmos.color = presentationComparisonColor;
            Gizmos.DrawLine(presentationPosition, presentationOtherComparisonPosition);

        }
    }
}
