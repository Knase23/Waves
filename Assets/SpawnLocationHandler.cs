using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LevelGenerator))]
public class SpawnLocationHandler : MonoBehaviour
{

    public GameObject shipPrefab;


    
    public Dictionary<long, InputHandler> userToInput = new Dictionary<long, InputHandler>();
    DiscordLobbyService lobbyService;
    LevelGenerator level;



    // Start is called before the first frame update
    void Start()
    {
        level = GetComponent<LevelGenerator>();

        lobbyService = DiscordLobbyService.INSTANCE;
        lobbyService.lobbyManager.OnMemberConnect += LobbyManager_OnMemberConnect;
        lobbyService.lobbyManager.OnMemberDisconnect += LobbyManager_OnMemberDisconnect;
    }

    private void LobbyManager_OnMemberDisconnect(long lobbyId, long userId)
    {
        Debug.Log("Lobby ID: " + lobbyId + "-  User ID: " + userId + " - Member Disconnected");
        //Disable the ship controlled by the user, if the user is not connected after a certain amount, we will destroy the ship and its data. 
    }

    private void LobbyManager_OnMemberConnect(long lobbyId, long userId)
    {
        Debug.Log("Lobby ID: " + lobbyId + "-  User ID: " + userId + " - Member Connected");
        

        //Only when we are in a Session. 
        // Check if the userId already have a ship in the session. 
        // If he have a ship do nothing
        // Otherwise Create a new ship
        
    }

    public void SpawnInShipsForAllMembers()
    {
        //Run for the current lobby
        //Spawn in a ship for each member on the determined spawn locations given from LevelGenerator
        foreach (var user in lobbyService.GetLobbyMembers())
        {
            SpawnInOneShipForUser(user.Id);
        } 

    }

    public void SpawnInOneShipForUser(long userID)
    {
        if (userToInput.ContainsKey(userID))
        {
            return;
        }
        GameObject obj = Instantiate(shipPrefab, Vector3.zero, Quaternion.identity);
        InputHandler inputHandler = obj.GetComponent<InputHandler>();
        inputHandler.userID = userID;
        userToInput.Add(userID, inputHandler);
    }
}
