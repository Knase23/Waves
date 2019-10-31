using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public delegate void JoinedLobby();
    public delegate void DisconnectedLobby();
    public static event JoinedLobby OnJoinedLobby;
    public static event DisconnectedLobby OnDisconnectLobby;
    public GameState gameState = GameState.NOT_IN_LOBBY;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
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
        OnDisconnectLobby += GameManager_OnDisconnectLobby;
        OnJoinedLobby += GameManager_OnJoinedLobby;
    }

    private void GameManager_OnJoinedLobby()
    {
        SetGameState(GameState.IN_LOBBY);
    }

    private void GameManager_OnDisconnectLobby()
    {
        SetGameState(GameState.NOT_IN_LOBBY);
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
    public void LoadGameScene()
    {
        if(DiscordLobbyService.INSTANCE.IsTheHost())
        {
            //Send Message to load scene for clients
            LoadScenePackage loadScenePackage = new LoadScenePackage(1);
            DiscordNetworkLayerService.INSTANCE.SendMessegeToAllOthers(NetworkChannel.LOADSCENE,loadScenePackage.ToBytes());
        }
        SceneManager.LoadScene(1);
    }
    public void LoadScene(LoadScenePackage package)
    {
        SceneManager.LoadScene(package.index);
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
    public enum GameState
    {
        NOT_IN_LOBBY,
        IN_LOBBY,
        LOADING_MAP,
        DONE_LOADING_MAP,
        GAME_IN_SESSION,
        GAME_END
    }
    public void SetGameState(GameState state)
    {
        gameState = state;
        //What should happen when the state is changed
        switch (state)
        {
            case GameState.NOT_IN_LOBBY:
                break;
            case GameState.IN_LOBBY:
                break;
            case GameState.LOADING_MAP:
                //When we are currently 
                break;
            case GameState.DONE_LOADING_MAP:
                //Sets so we know the client is done loading in the map
                break;
            case GameState.GAME_IN_SESSION:
                Time.timeScale = 1;
                //Everything is running until GameTime is zero
                break;
            case GameState.GAME_END:
                // Display Score for all current players and a disconnect for all players
                Time.timeScale = 0;
                break;
            default:
                break;
        }
    }

}

public struct LoadScenePackage
{
    public int index;
    public LoadScenePackage(int index)
    {
        this.index = index; 
    }
    public LoadScenePackage(byte[] data)
    {
        this.index = BitConverter.ToInt32(data,0);
    }
    public byte[] ToBytes()
    {
        List<byte> vs = new List<byte>();
        vs.AddRange(BitConverter.GetBytes(index));
        return vs.ToArray();
    }
}
