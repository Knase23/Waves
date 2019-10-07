﻿using System.Collections;
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
        // Make sure we are the Host, if not wait until we recieve a message


        // Clean up Scene, so there is a no asteriods in the scene
        ClearLevel();


        // Start Generating the Level using Sampling
        totalOfConfirmedObjects = 0;

        //List of all created Objects, used later for Networking
        List<GameObject> allGeneratedObjects = new List<GameObject>();

        //Setup the area for were we can spawn in the Asteriods
        Vector3 maxPosition = new Vector3(mapSize.x, 0, mapSize.y);
        Vector3 minPosition = new Vector3(-mapSize.x, 0, -mapSize.y);
        
        
        //Spawn in Large Asteriods with there placement rules. within setup area.
        allGeneratedObjects.AddRange(Sampling.SampleGenerating(numberOfLargeWanted, largeAstroidPrefabs.GetComponent<Asteriod>().placementRules, largeAstroidPrefabs, maxPosition, minPosition, largeTransform, numberOfRejections: maxNumberOfFailedPlacements));
        
        //Spawn in Medium Asteriods with there placement rules. within setup area.
        allGeneratedObjects.AddRange(Sampling.SampleGenerating(numberOfMediumWanted, mediumAstroidPrefabs.GetComponent<Asteriod>().placementRules, mediumAstroidPrefabs, maxPosition, minPosition, mediumTransform, numberOfRejections: maxNumberOfFailedPlacements));
        
        //Spawn in Small Asteriods with there placement rules. within setup area.
        allGeneratedObjects.AddRange(Sampling.SampleGenerating(numberOfSmallWanted, smallAstroidPrefabs.GetComponent<Asteriod>().placementRules, smallAstroidPrefabs, maxPosition, minPosition, smallTransform, numberOfRejections: maxNumberOfFailedPlacements));
        totalOfConfirmedObjects = allGeneratedObjects.Count;

        // Done with Obsticles in level
        
        // Generate Locations for spawn positions for all players
        
        // Generate Pick Up spawn positions

        // Done with all level Setups
        
        // Send a Network Package to all Clients, this must be TCP, We need to have identical maps.
        /*
         * Network Code Stuff
         * 
         */
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
