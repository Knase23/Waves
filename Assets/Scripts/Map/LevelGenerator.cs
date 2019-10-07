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

    GameObject SpawnInPrefab(GameObject prefab, Vector3 position, Transform parent)
    {
        GameObject gameObject = Instantiate(prefab, position, UnityEngine.Random.rotation, parent);
        return gameObject;
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
