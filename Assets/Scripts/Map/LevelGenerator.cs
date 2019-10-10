using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LevelGenerator : MonoBehaviour
{
    public bool GenerateOnStart = true;
    
    public Transform mapTransform;
    public Transform spawnPointTransform;
    public Vector3 maxPosition = new Vector3(100, 0, 100);
    public Vector3 minPosition = new Vector3(-100, 0, -100);

    [Header("Astriod prefabs")]
    public GameObject largeAstroidPrefabs;
    public GameObject mediumAstroidPrefabs;
    public GameObject smallAstroidPrefabs;

    [Header("Organization For Asteriods")]
    public Transform smallTransform;
    public Transform mediumTransform;
    public Transform largeTransform;

    [Header("Sampling Parameters")]
    public float sampleMultiplier = 1.5f;
    public int maxNumberOfFailedPlacements = 10;
    public int numberOfLargeWanted = 50;
    public int numberOfMediumWanted = 50;
    public int numberOfSmallWanted = 50;
    public int numberOfSpawnPoints = 10;
    [Header("Debugging")]
    public int totalOfConfirmedObjects = 0;
    
    private List<GameObject> confirmedPlacements = new List<GameObject>();
    public List<GameObject> spawnpoints = new List<GameObject>();
    SpawnLocationHandler spawnHandler;
    private void Start()
    {
        if(GenerateOnStart && SceneManager.GetActiveScene().name == "StartMenu")
            GenerateLevel();

        if(SceneManager.GetActiveScene().name == "GameScene" && DiscordLobbyService.INSTANCE.IsTheHost())
        {
            //GenerateLevel();
            //Start a function that waits until every LobbyMember have completed loading.
        }
        else
        {
            //ClearLevel();
            //Start a function that waits until every LobbyMember have completed loading.
        }

    }
    /// <summary>
    /// Generates a map of astiods with the given prefabs
    /// </summary>
    public void GenerateLevel()
    {
        // Make sure we are the Host, if not wait until we recieve a message


        // Clean up Scene, so there is a no asteriods in the scene
        ClearLevel();


        // Start Generating the Level using Sampling
        totalOfConfirmedObjects = 0;

        //List of all created Objects, used later for Networking
        List<GameObject> allGeneratedObjects = new List<GameObject>();

        //Spawn in Large Asteriods with there placement rules. within setup area.
        List<GameObject> largeAsteriodsCreated = Sampling.SampleGenerating(numberOfLargeWanted, largeAstroidPrefabs.GetComponent<Asteriod>().placementRules, largeAstroidPrefabs, maxPosition, minPosition, largeTransform, numberOfRejections: maxNumberOfFailedPlacements);
        allGeneratedObjects.AddRange(largeAsteriodsCreated);

        //Spawn in Medium Asteriods with there placement rules. within setup area.
        List<GameObject> mediumAsteriodsCreated = Sampling.SampleGenerating(numberOfMediumWanted, mediumAstroidPrefabs.GetComponent<Asteriod>().placementRules, mediumAstroidPrefabs, maxPosition, minPosition, mediumTransform, numberOfRejections: maxNumberOfFailedPlacements);
        allGeneratedObjects.AddRange(mediumAsteriodsCreated);

        //Spawn in Small Asteriods with there placement rules. within setup area.
        List<GameObject> smallAsteriodsCreated = Sampling.SampleGenerating(numberOfSmallWanted, smallAstroidPrefabs.GetComponent<Asteriod>().placementRules, smallAstroidPrefabs, maxPosition, minPosition, smallTransform, numberOfRejections: maxNumberOfFailedPlacements);
        allGeneratedObjects.AddRange(smallAsteriodsCreated);

        totalOfConfirmedObjects = allGeneratedObjects.Count;

        // Done with Obsticles in level

        // Generate Locations for spawn positions for all players
        if(spawnHandler == null)
        {
            spawnHandler = GetComponent<SpawnLocationHandler>();
        }
        if (GetComponent<SpawnLocationHandler>().enabled)
        {
            spawnpoints.Clear();
            spawnpoints.AddRange(Sampling.SampleGenerating(numberOfSpawnPoints, spawnHandler.placementRules, spawnHandler.spawnPointPrefab, maxPosition, minPosition, spawnPointTransform));
        }
        // Generate Pick Up spawn positions

        // Done with all level Setups

        if (DiscordLobbyService.INSTANCE.IsTheHost())
        {
            LevelDetailsPackage levelDetailsPackage = new LevelDetailsPackage(largeAsteriodsCreated.Count,mediumAsteriodsCreated.Count,smallAsteriodsCreated.Count,spawnpoints.Count);
            DiscordNetworkLayerService.INSTANCE.SendMessegeToAllOthers(NetworkChannel.START_LOADING_MAP, levelDetailsPackage.ToBytes());
        }
        // Send a Network Package to all Clients, this must be Relaiable, We need to have identical maps.
        /*
         * Network Code Stuff
         * 
         */
    }

    IEnumerable SendLevel()
    {
        //Make sure we get a respond from everyone that they are ready to recive the data

        //Start Sending LargeAsteriodsCreated
        //Start Sending MediumAsteriodsCreated
        //Start Sending SmallAsteriodsCreated
        //Start Sending SpawnPointsCreated

        //Wait for messeges of everyone completed
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
    private List<GameObject> GetAstriodsFromTransform(Transform transform)
    {
        List<GameObject> list = new List<GameObject>();
        // This is a Recursive Function, it will go though all gameobjects under the given transform and added them to the list
        AddGameObjectWithTagFormTransform("Asteriod", transform, ref list);
        return list;
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
}
public struct LevelDetailsPackage
{
    public int numberOfLarge;
    public int numberOfMedium;
    public int numberOfSmall;
    public int numberOfSpawnPoints;

    public LevelDetailsPackage(int large, int medium,int small,int spawnPoint)
    {
        numberOfLarge = large;
        numberOfMedium = medium;
        numberOfSmall = small;
        numberOfSpawnPoints = spawnPoint;
    }
    public LevelDetailsPackage(byte[] data)
    {
        numberOfLarge = BitConverter.ToInt32(data,0);
        numberOfMedium = BitConverter.ToInt32(data, 4);
        numberOfSmall = BitConverter.ToInt32(data, 8);
        numberOfSpawnPoints = BitConverter.ToInt32(data, 12);
    }
    public byte[] ToBytes()
    {
        List<byte> vs = new List<byte>();

        vs.AddRange(BitConverter.GetBytes(numberOfLarge));
        vs.AddRange(BitConverter.GetBytes(numberOfMedium));
        vs.AddRange(BitConverter.GetBytes(numberOfSmall));
        vs.AddRange(BitConverter.GetBytes(numberOfSpawnPoints));

        return vs.ToArray();
        
    }

}
