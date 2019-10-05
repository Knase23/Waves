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
    public float sampleMultiplier = 1.5f;
    public int numberOfLargeWanted = 50;
    public int numberOfMediumWanted = 50;
    public int numberOfSmallWanted = 50;
    [Header("Debugging")]
    public int totalOfConfirmedObjects = 0;
    public bool ShowPresentationSphere = true;
    public float presentationSphereSize = 5;
    private Vector3 presentationPosition = Vector3.zero;
    private Vector3 presentationOtherComparisonPosition = Vector3.zero;
    private Color presentationColor = Color.white;
    private Color presentationComparisonColor = Color.white;
    private bool displayPrenentation = false;

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
        totalOfConfirmedObjects = 0;
        //Large
        Sampling.SampleGenerating(numberOfLargeWanted, largeAstroidPrefabs.GetComponent<Asteriod>().placementRules, largeAstroidPrefabs, new Vector3(mapSize.x, 0, mapSize.y), new Vector3(-mapSize.x, 0, -mapSize.y), largeTransform, numberOfRejections: maxNumberOfFailedPlacements);
        //Medium
        Sampling.SampleGenerating(numberOfMediumWanted, mediumAstroidPrefabs.GetComponent<Asteriod>().placementRules, mediumAstroidPrefabs, new Vector3(mapSize.x, 0, mapSize.y), new Vector3(-mapSize.x, 0, -mapSize.y), mediumTransform, numberOfRejections: maxNumberOfFailedPlacements);
        //Small
        Sampling.SampleGenerating(numberOfSmallWanted, smallAstroidPrefabs.GetComponent<Asteriod>().placementRules, smallAstroidPrefabs, new Vector3(mapSize.x, 0, mapSize.y), new Vector3(-mapSize.x, 0, -mapSize.y), smallTransform, numberOfRejections: maxNumberOfFailedPlacements);
    }
    public void VisualGenerateLevel()
    {
        ClearLevel();
        totalOfConfirmedObjects = 0;
        GenerateLevel();
        //StartCoroutine(StartGenerate());

    }
    //Coroutine coroutine = null;
    //public IEnumerator StartGenerate()
    //{

    //    coroutine = StartCoroutine(VisualRepresentationOfSampling(numberOfLargeWanted, largeAstroidPrefabs, largeTransform, maxNumberOfFailedPlacements));
    //    yield return new WaitUntil(() => coroutine == null);

    //    coroutine = StartCoroutine(VisualRepresentationOfSampling(numberOfMediumWanted, mediumAstroidPrefabs, mediumTransform, maxNumberOfFailedPlacements));

    //    yield return new WaitUntil(() => coroutine == null);

    //    coroutine = StartCoroutine(VisualRepresentationOfSampling(numberOfSmallWanted, smallAstroidPrefabs, smallTransform, maxNumberOfFailedPlacements));
    //    yield return new WaitUntil(() => coroutine == null);
    //    Debug.Log("Level Generation Complete");
    //}

    //public void SampleGeneration(int numberOfActualDesiredObjects, GameObject prefab, Transform parent, int numberOfRejections = 10)
    //{
    //    int numberOfSamples = (int)(numberOfActualDesiredObjects * sampleMultiplier);
    //    Debug.Log("Number Of Samples for " + prefab.name + " :" + numberOfSamples);
    //    Asteriod preFabAsteriod = prefab.GetComponent<Asteriod>();
    //    Vector3 position;
    //    int numberOfDesiredObjects = numberOfActualDesiredObjects;
    //    int countOfConfirmed = 0;

    //    for (int i = 0; i < numberOfSamples && countOfConfirmed < numberOfDesiredObjects; i++)
    //    {
    //        bool validPlacement = false;
    //        bool rejected = false;
    //        int countOfFailedPlacement = 0;
    //        do
    //        {
    //            position = new Vector3(UnityEngine.Random.Range(-mapSize.x, mapSize.x), 0, UnityEngine.Random.Range(-mapSize.y, mapSize.y));

    //            if (preFabAsteriod.ValidatePlacement(position))
    //            {
    //                validPlacement = true;
    //                continue;
    //            }

    //            if (countOfFailedPlacement >= numberOfRejections)
    //            {
    //                validPlacement = true;
    //                rejected = true;
    //                continue;
    //            }
    //            countOfFailedPlacement++;

    //        } while (!validPlacement);

    //        if (rejected)
    //        {
    //            numberOfDesiredObjects--;
    //            continue;
    //        }

    //        countOfConfirmed++;
    //        totalOfConfirmedObjects++;
    //        confirmedPlacements.Add(SpawnInPrefab(prefab, position, parent));
    //    }

    //}

    GameObject SpawnInPrefab(GameObject prefab, Vector3 position, Transform parent)
    {
        GameObject gameObject = Instantiate(prefab, position, UnityEngine.Random.rotation, parent);
        return gameObject;
    }
    //This sould have the same code as SampleLevel
    #region SAMPLING : Showing What the code does Visualy in the Editor;
    //public IEnumerator VisualRepresentationOfSampling(int numberOfActualDesiredObjects, GameObject preFab, Transform hieararcyHolder, int numberOfRejections = 10)
    //{
    //    displayPrenentation = true;
    //    int numberOfSamples = (int)(numberOfActualDesiredObjects * sampleMultiplier);
    //    Debug.Log("Number Of Samples for " + preFab.name + " :" + numberOfSamples);
    //    Asteriod preFabAsteriod = preFab.GetComponent<Asteriod>();
    //    presentationSphereSize = preFabAsteriod.radius;
    //    Vector3 position;
    //    int numberOfDesiredObjects = numberOfActualDesiredObjects;
    //    int countOfConfirmed = 0;
    //    for (int i = 0; i < numberOfSamples && countOfConfirmed < numberOfDesiredObjects; i++)
    //    {
    //        bool validPlacement = false;
    //        bool rejected = false;
    //        int countOfFailedPlacement = 0;

    //        //Only for the Visual
    //        presentationColor = Color.yellow;

    //        do
    //        {
    //            position = new Vector3(UnityEngine.Random.Range(-mapSize.x, mapSize.x), 0, UnityEngine.Random.Range(-mapSize.y, mapSize.y));
    //            presentationPosition = position;
    //            presentationOtherComparisonPosition = position;
    //            #region Code From Asteriod.ValidatePlacement(Vector3 Position)
    //            bool result = true;
    //            //Code that is inside Asteriod.ValidatePlacement(Vector3 position);
    //            if (!Physics.CheckSphere(position, preFabAsteriod.radius))
    //            {
    //                if (preFab == smallAstroidPrefabs)
    //                {
    //                    SmallAsteriod smallAsteriod = preFabAsteriod as SmallAsteriod;
    //                    smallAsteriod.largeOrMediumNearIt = 0;
    //                }
    //                Collider[] listOfHits = Physics.OverlapSphere(position, preFabAsteriod.searchRadius);
    //                for (int k = 0; k < listOfHits.Length; k++)
    //                {
    //                    presentationComparisonColor = Color.yellow;
    //                    if (listOfHits[k].gameObject == gameObject)
    //                    {
    //                        continue;
    //                    }
    //                    if (listOfHits[k].tag != "Asteriod")
    //                    {
    //                        continue;
    //                    }
    //                    presentationOtherComparisonPosition = listOfHits[k].transform.position;
    //                    yield return new WaitForSecondsRealtime(2f / numberOfSamples);
    //                    if (!preFabAsteriod.ValidationOfPlacement(position, listOfHits[k].GetComponent<Asteriod>()))
    //                    {
    //                        result = false;
    //                        presentationComparisonColor = Color.red;
    //                        yield return new WaitForSecondsRealtime(2f / numberOfSamples);
    //                        break;
    //                    }
    //                    presentationComparisonColor = Color.green;
    //                    yield return new WaitForSecondsRealtime(2f / numberOfSamples);
    //                }
    //                if (preFab == smallAstroidPrefabs)
    //                {
    //                    SmallAsteriod smallAsteriod = preFabAsteriod as SmallAsteriod;
    //                    if (smallAsteriod.largeOrMediumNearIt < 1)
    //                    {
    //                        result = false;
    //                    }
    //                }

    //            }
    //            else
    //            {
    //                result = false;
    //            }
    //            // Placement is valid, you can place the Asteriod there
    //            #endregion

    //            //Here it tries to Validate it
    //            if (result)
    //            {
    //                validPlacement = true;
    //                continue;
    //            }

    //            if (countOfFailedPlacement >= numberOfRejections)
    //            {
    //                validPlacement = true;
    //                rejected = true;
    //                continue;
    //            }
    //            countOfFailedPlacement++;

    //            //Only for the Visual
    //            yield return new WaitForSecondsRealtime(1f / numberOfSamples);

    //        } while (!validPlacement);

    //        if (rejected)
    //        {
    //            numberOfDesiredObjects--;

    //            //Only for the Visual
    //            presentationColor = Color.red;
    //            yield return new WaitForSecondsRealtime(2f / numberOfSamples);

    //            continue;
    //        }
    //        //Only for the Visual
    //        presentationColor = Color.green;
    //        yield return new WaitForSecondsRealtime(3f / numberOfSamples);

    //        countOfConfirmed++;
    //        totalOfConfirmedObjects++;
    //        confirmedPlacements.Add(Instantiate(preFab, position, UnityEngine.Random.rotation, hieararcyHolder));
    //    }
    //    displayPrenentation = false;
    //    coroutine = null;
    //    yield break;
    //}
    #endregion

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
    private void OnDrawGizmos()
    {
        if (ShowPresentationSphere && displayPrenentation)
        {

            Gizmos.color = presentationColor;
            Gizmos.DrawSphere(presentationPosition, presentationSphereSize);
            Gizmos.color = presentationComparisonColor;
            Gizmos.DrawLine(presentationPosition, presentationOtherComparisonPosition);

        }
    }


}
