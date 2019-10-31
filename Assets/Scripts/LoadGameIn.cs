using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadGameIn : MonoBehaviour
{
    LevelGenerator generator;
    public LevelDetailsPackage levelDetails;
    List<long> usersThatAreReady = new List<long>();

    public Canvas canvas;
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    public LevelDetailsPackage progressOfLevelDetail;
    public float progress;


    private void Start()
    {
        generator = FindObjectOfType<LevelGenerator>();
    }
    private void Update()
    {
        ShowProgress();
    }

    public void GetMapDetails(LevelDetailsPackage package)
    {
        levelDetails = package;
        progressOfLevelDetail = new LevelDetailsPackage();
    }

    public void ShowProgress()
    {
        float maxProgress = 4;

        float largeAsteriodProgress = progressOfLevelDetail.numberOfLarge / (float)levelDetails.numberOfLarge;
        float mediumAsteriodProgress = progressOfLevelDetail.numberOfMedium / (float)levelDetails.numberOfMedium;
        float smallAsteriodProgress = progressOfLevelDetail.numberOfSmall / (float)levelDetails.numberOfSmall;
        float spawmProgress = progressOfLevelDetail.numberOfSpawnPoints / (float)levelDetails.numberOfSpawnPoints;

        progress = (largeAsteriodProgress + mediumAsteriodProgress + smallAsteriodProgress + spawmProgress) / maxProgress;

        progressBar.maxValue = 1;
        progressBar.value = progress;

        if (progress >= 1)
        {
            //SpawnInShips
            SpawnLocationHandler spawnLocation = FindObjectOfType<SpawnLocationHandler>();
            GameManager.Instance.SetGameState(GameManager.GameState.DONE_LOADING_MAP);
            spawnLocation.SpawnInShipsForAllMembers();
            canvas.gameObject.SetActive(false);
            enabled = false;
        }
        else
        {
            GameManager.Instance.SetGameState(GameManager.GameState.LOADING_MAP);
        }
    }

    public void IncreaseProgress(LevelGenerator.LevelObject levelObject)
    {
        switch (levelObject)
        {
            case LevelGenerator.LevelObject.LargeAsteriod:
                progressOfLevelDetail.numberOfLarge++;
                progressText.text = "Spawning in Large Asteriods";
                break;
            case LevelGenerator.LevelObject.MediumAsteriod:
                progressOfLevelDetail.numberOfMedium++;
                progressText.text = "Spawning in Medium Asteriods";
                break;
            case LevelGenerator.LevelObject.SmallAsteriod:
                progressOfLevelDetail.numberOfSmall++;
                progressText.text = "Spawning in Small Asteriods";
                break;
            case LevelGenerator.LevelObject.SpawnPoint:
                progressOfLevelDetail.numberOfSpawnPoints++;
                progressText.text = "Spawning in Spawnpoints";
                break;
            default:
                break;
        }
    }
}
