using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{

    public static Statistics instance;


    public int numberOfNodes = 0;
    public int numberOfShips = 0;
    public float maxLengthBetweenNodes = 5;
    public bool showWhereCircleSplits = true;
    public int numberOfSplitsLevels = 0;
    

    internal int lastFrameCount = 0;
    internal Color latestColor = Color.black;
    private void Awake()
    {
        if(instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

}
