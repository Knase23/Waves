using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{

    public static SpawnObject INSTANCE;
    public List<GameObject> listOfPrefabsToSpawnIn = new List<GameObject>();
    LevelGenerator generator;
    LoadGameIn load;

    private void Awake()
    {
        if(INSTANCE)
        {
            Destroy(this);
        }
        else
        {
            INSTANCE = this;
        }
    }

    private void Start()
    {
        generator = FindObjectOfType<LevelGenerator>();
        load = FindObjectOfType<LoadGameIn>();
    }

    public void SpawnInObject(TransformDataPackage data)
    {
        switch ((ItemSpawned)data.id)
        {
            case ItemSpawned.LargeAsteriod:

                generator.SpawnInLevelObject(LevelGenerator.LevelObject.LargeAsteriod, data.position, data.rotation);
                load.IncreaseProgress(LevelGenerator.LevelObject.LargeAsteriod);
                break;
            case ItemSpawned.MediumAsteriod:
                generator.SpawnInLevelObject(LevelGenerator.LevelObject.MediumAsteriod, data.position, data.rotation);
                load.IncreaseProgress(LevelGenerator.LevelObject.MediumAsteriod);
                break;
            case ItemSpawned.SmallAsteriod:
                generator.SpawnInLevelObject(LevelGenerator.LevelObject.SmallAsteriod, data.position, data.rotation);
                load.IncreaseProgress(LevelGenerator.LevelObject.SmallAsteriod);
                break;
            case ItemSpawned.SpawnPoint:
                generator.SpawnInLevelObject(LevelGenerator.LevelObject.SpawnPoint, data.position, data.rotation);
                load.IncreaseProgress(LevelGenerator.LevelObject.SpawnPoint);
                break;
            default:
                Debug.Log("Not Vaild object to spawn");
                break;
        }
    }
    public enum ItemSpawned
    {
        LargeAsteriod,
        MediumAsteriod,
        SmallAsteriod,
        SpawnPoint,
    }
}
