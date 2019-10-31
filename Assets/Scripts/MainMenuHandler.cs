using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    public LobbyHandler LobbyHandler;
    public SearchLobby SearchLobby;

    // Start is called before the first frame update
    void Start()
    {
        DiscordLobbyService.INSTANCE.OnJoinedLobby += OnJoinedLobby;
        DiscordLobbyService.INSTANCE.OnLeftLobby += LeftLobby;
    }

    private void OnJoinedLobby()
    {
        if (LobbyHandler)
        {
            LobbyHandler.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Have no LobbyHandler", gameObject);
        }
        if (SearchLobby)
        {
            SearchLobby.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Have no LobbyHandler",gameObject);
        }
       

    }
    private void LeftLobby()
    {
        if (LobbyHandler)
        {
            LobbyHandler.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Have no LobbyHandler", gameObject);
        }
        if (SearchLobby)
        {
            SearchLobby.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Have no LobbyHandler", gameObject);
        }
    }
}
