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

    [Header("Perlin noise")]
    public float noiseZoom;
    public bool randomOffset = false;
    public Vector2 perlinOffset;

    public float levelScale;

    [Serializable]
    public struct Range2Float
    {
        public float max;
        public float min;
    }

    public Range2Float largeRange;
    public Range2Float mediumRange;
    public Range2Float smallRange;

    [Header("Astriod prefabs")]
    public GameObject largeAstroidPrefabs;
    public GameObject mediumAstroidPrefabs;
    public GameObject smallAstroidPrefabs;

    [Header("Tolerance")]
    public int maxNumberOfFailedPlacements = 10;

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
        ClearLevel();
        float[,] noiseMap;

        if (randomOffset)
        {
            noiseMap = GenerateNoiseMap(mapSize.x, mapSize.y, noiseZoom, UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
        }
        else
        {
            noiseMap = GenerateNoiseMap(mapSize.x, mapSize.y, noiseZoom, perlinOffset.x, perlinOffset.y);

        }
        Vector3 centerPositionCorrection = new Vector3(transform.position.x - (mapSize.x / 2), transform.position.y, transform.position.z - (mapSize.y / 2));

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                // Small
                if (noiseMap[x, y] < smallRange.max && noiseMap[x, y] > smallRange.min)
                {
                    Vector3 position = new Vector3(centerPositionCorrection.x + x, centerPositionCorrection.y, centerPositionCorrection.z + y) + new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
                    if (EditorApplication.isPlaying)
                    {
                        Instantiate(smallAstroidPrefabs, position, UnityEngine.Random.rotation, smallTransform);
                    }
                    else
                    {
                        GameObject prefab = PrefabUtility.InstantiatePrefab(smallAstroidPrefabs, smallTransform) as GameObject;
                        prefab.transform.position = position;
                        prefab.transform.rotation = UnityEngine.Random.rotation;
                    }
                }

                // Medium
                if (noiseMap[x, y] < mediumRange.max && noiseMap[x, y] > mediumRange.min)
                {
                    Vector3 position = new Vector3(centerPositionCorrection.x + x, centerPositionCorrection.y, centerPositionCorrection.z + y) + new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));

                    if (EditorApplication.isPlaying)
                    {
                        Instantiate(mediumAstroidPrefabs, position , UnityEngine.Random.rotation, mediumTransform);
                    }
                    else
                    {

                        GameObject prefab = PrefabUtility.InstantiatePrefab(mediumAstroidPrefabs, mediumTransform) as GameObject;
                        prefab.transform.position = position;
                        prefab.transform.rotation = UnityEngine.Random.rotation;
                    }
                }
                //Large
                if (noiseMap[x, y] < largeRange.max && noiseMap[x, y] > largeRange.min)
                {
                    Vector3 position = new Vector3(centerPositionCorrection.x + x, centerPositionCorrection.y, centerPositionCorrection.z + y) + new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));

                    if (EditorApplication.isPlaying)
                    {
                        Instantiate(largeAstroidPrefabs, position, UnityEngine.Random.rotation, largeTransform);
                    }
                    else
                    {
                        GameObject prefab = PrefabUtility.InstantiatePrefab(largeAstroidPrefabs, largeTransform) as GameObject;
                        prefab.transform.position = position;
                        prefab.transform.rotation = UnityEngine.Random.rotation;

                    }
                }
            }
        }

        CheckAndRemoveObjectsThatIsInsideEachOther(largeTransform);
        CheckAndRemoveObjectsThatIsInsideEachOther(mediumTransform);
        CheckAndRemoveObjectsThatIsInsideEachOther(smallTransform);

        List<GameObject> medium = GetAstriodsFromTransform(mediumTransform);
        List<GameObject> mediumToRemove = GetListOfIntercetionsBetweenToListsOfObjects(GetAstriodsFromTransform(largeTransform),medium);
        foreach (var item in mediumToRemove)
        {
            DestroyImmediate(item);
        }

        List<GameObject> smallToRemove = GetListOfIntercetionsBetweenToListsOfObjects(medium,GetAstriodsFromTransform(smallTransform));
        foreach (var item in smallToRemove)
        {
            DestroyImmediate(item);
        }
    }


    private List<GameObject>  GetListOfIntercetionsBetweenToListsOfObjects(List<GameObject> list, List<GameObject> list2)
    {
        List<GameObject> smallToRemove = new List<GameObject>();
        for (int i = 0; i < list.Count; i++)
        {
            Bounds mediumBounds = GetGameObjectsRealBounds(list[i]);

            // Want a better way of checking this!
            for (int k = 0; k < list2.Count; k++)
            {
                if (smallToRemove.Contains(list2[k]))
                {
                    continue;
                }
                Bounds smallBounds = GetGameObjectsRealBounds(list2[k]);
                if (mediumBounds.Intersects(smallBounds))
                {
                    //Debug.Log(i +" added "+  k + " Is in the RemoveList");
                    smallToRemove.Add(list2[k]);
                    continue;
                }
            }
        }
        return smallToRemove;
    }

    /// <summary>
    /// Help function for determining bounds in Not PlayMode!
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    private Bounds GetGameObjectsRealBounds(GameObject gameObject)
    {
       return new Bounds(gameObject.transform.position, gameObject.GetComponentInChildren<Collider>().bounds.extents * 1.5f);
    }
    private void CheckAndRemoveObjectsThatIsInsideEachOther(Transform transform)
    {
        List<GameObject> astriodsToRemove = CheckListOfGameObjectsIfTheyIntercets(GetAstriodsFromTransform(transform));
        foreach (var item in astriodsToRemove)
        {
            DestroyImmediate(item);
        }
    }

    private List<GameObject> CheckListOfGameObjectsIfTheyIntercets(List<GameObject> list)
    {
        List<GameObject> astriodsToRemove = new List<GameObject>();
        for (int i = 0; i < list.Count; i++)
        {
            if (astriodsToRemove.Contains(list[i]))
            {
                continue;
            }

            Bounds astriodBounds = GetGameObjectsRealBounds(list[i]);

            // Want a better way of checking this!
            for (int k = i + 1; k < list.Count; k++)
            {
                Bounds realBounds = GetGameObjectsRealBounds(list[k]);

                if (astriodBounds.Intersects(realBounds))
                {
                    //Debug.Log(i +" added "+  k + " Is in the RemoveList");
                    astriodsToRemove.Add(list[k]);
                }

            }

        }
        return astriodsToRemove;
    }
    /// <summary>
    /// Returns a Noise Map that can be used to make a level
    /// </summary>
    /// <param name="mapDepth"></param>
    /// <param name="mapWidth"></param>
    /// <param name="scale"> Acts as a Zoom on the noise map</param>
    /// <returns>Noise Map</returns>
    public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX = 0, float offsetZ = 0)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                // calculate sample indices based on the coordinates and the scale
                float sampleX = (xIndex / scale) + offsetX;
                float sampleZ = (zIndex / scale) + offsetZ;

                // generate noise value using PerlinNoise
                float noise = Mathf.PerlinNoise(sampleX, sampleZ);

                noiseMap[zIndex, xIndex] = noise;
            }
        }
        return noiseMap;
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
    private void AddGameObjectWithTagFormTransform(string tag,Transform transform, ref List<GameObject> gameObjects)
    {
        if(transform.tag == tag)
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
        AddGameObjectWithTagFormTransform("Astriod", transform, ref list);
        return list;
    }


}
