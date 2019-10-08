using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }
    DiscordLobbyService lobbyService;
    SpawnLocationHandler spawnLocationHandler;
    // Start is called before the first frame update
    void Start()
    {
        lobbyService = GetComponent<DiscordLobbyService>();
        spawnLocationHandler = FindObjectOfType<SpawnLocationHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            lobbyService.CreateLobby();
            UserData userData = FindObjectOfType<UserData>();

            userData.id = lobbyService.GetCurrentUserId();
        }
        if (Input.GetKeyUp(KeyCode.Return))
        { 
            spawnLocationHandler.SpawnInShipsForAllMembers();
        }
    }
}
