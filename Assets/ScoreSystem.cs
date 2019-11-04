using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }
    //Lista med alla scoreHandler
    public Dictionary<long, ScoreHandler> memberScorePair = new Dictionary<long, ScoreHandler>();

    public void AddPair(long id,ScoreHandler scoreHandler)
    {
        if(memberScorePair.ContainsKey(id))
        {
            return;
        }
        memberScorePair.Add(id, scoreHandler);
    }

}
