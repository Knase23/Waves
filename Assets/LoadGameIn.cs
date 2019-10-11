using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameIn : MonoBehaviour
{
    bool allReady = false;
    LevelGenerator generator;
    LevelDetailsPackage levelDetails;
    List<long> usersThatAreReady = new List<long>();

    public LevelDetailsPackage progressOfLevelDetail;
    public float progress;


    private void Start()
    {
        generator = FindObjectOfType<LevelGenerator>();
    }

    public void GetMapDetails(LevelDetailsPackage package)
    {
        levelDetails = package;
        progressOfLevelDetail = new LevelDetailsPackage();
    }

    public void ShowProgress()
    {
        float maxProgress = levelDetails.numberOfLarge + levelDetails.numberOfMedium + levelDetails.numberOfSmall + levelDetails.numberOfSpawnPoints;

        progress = (progressOfLevelDetail.numberOfLarge + progressOfLevelDetail.numberOfMedium + progressOfLevelDetail.numberOfSmall + progressOfLevelDetail.numberOfSpawnPoints)/maxProgress;



        if(progress >= 1)
        {
            allReady = true;
        }
    }

    
}
