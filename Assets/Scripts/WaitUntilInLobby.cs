using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitUntilInLobby : MonoBehaviour
{
    public Image image;
    public float timeToActAfterJoinedLobby = 0.6f;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        GameManager.OnJoinedLobby += GameManager_OnJoinedLobby;
    }
    private void GameManager_OnJoinedLobby()
    {
        Invoke("DisableImage", timeToActAfterJoinedLobby);
    }
    public void DisableImage()
    {
        image.enabled = false;
    }
}
