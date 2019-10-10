using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NetworkStatus : MonoBehaviour
{
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        GameManager.OnJoinedLobby += OnOnline;
        GameManager.OnDisconnectLobby += OnOffline;

        if(GameManager.Instance.gameState == GameManager.GameState.IN_LOBBY)
        {
            OnOnline();
        }
        else if (GameManager.Instance.gameState == GameManager.GameState.NOT_IN_LOBBY)
        {
            OnOffline();
        }

    }
    private void OnOnline()
    {
        text.text = "Online";
        text.color = Color.green;

    }
    private void OnOffline()
    {
        text.text = "Offline";
        text.color = Color.red;
    }


}
