using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public delegate void JoinedLobby();
    public delegate void DisconnectedLobby();
    public static event JoinedLobby OnJoinedLobby;
    public static event DisconnectedLobby OnDisconnectLobby;


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

    public static void CheckOnline()
    {
        if (DiscordLobbyService.IsOnline)
        {
            OnJoinedLobby();
        }
        else
        {
            OnDisconnectLobby();
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            lobbyService.CreateLobby();
            UserData userData = FindObjectOfType<UserData>();

            userData.id = DiscordManager.CurrentUser.Id;
        }
    }
}
