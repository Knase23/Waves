using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LevelGenerator))]
public class SpawnLocationHandler : MonoBehaviour
{
    public GameObject shipPrefab;
    public GameObject spawnPointPrefab;
    public Sample placementRules;

    public static Dictionary<long, InputHandler> userToInput = new Dictionary<long, InputHandler>();
    DiscordLobbyService lobbyService;
    DiscordNetworkLayerService discordNetworkLayerService;
    LevelGenerator level;

    public bool Debugging = false;

    // Start is called before the first frame update
    void Start()
    {
        discordNetworkLayerService = GetComponent<DiscordNetworkLayerService>();
        level = GetComponent<LevelGenerator>();
        lobbyService = DiscordLobbyService.INSTANCE;
        lobbyService.lobbyManager.OnMemberConnect += LobbyManager_OnMemberConnect;
        lobbyService.lobbyManager.OnMemberDisconnect += LobbyManager_OnMemberDisconnect;
        GameManager.OnJoinedLobby += SpawnInShipsForAllMembers;
    }

    private void LobbyManager_OnMemberDisconnect(long lobbyId, long userId)
    {
        if (Debugging)
            Debug.Log("Lobby ID: " + lobbyId + "-  User ID: " + userId + " - Member Disconnected");
        if(userToInput.ContainsKey(userId))
            userToInput[userId].gameObject.SetActive(false);
        //Disable the ship controlled by the user, if the user is not connected after a certain amount, we will destroy the ship and its data.         
    }

    private void LobbyManager_OnMemberConnect(long lobbyId, long userId)
    {
        if(Debugging)
            Debug.Log("Lobby ID: " + lobbyId + "-  User ID: " + userId + " - Member Connected");
        //Only when we are in a Session. 
        // Check if the userId already have a ship in the session. 
        // If he have a ship do nothing
        // Otherwise Create a new ship
        if (!userToInput.ContainsKey(userId))
        {
            Vector3 position = GetAvailableSpawnPoint();
            if (position == Vector3.down)
            {
                position = level.spawnpoints[0].transform.position;
            }
            if (Debugging)
                Debug.Log("Spawned in new ship");
            SpawnInOneShipForUser(userId, position);
        }
        else
        {
            if (Debugging)
                Debug.Log("Activate in old ship for user");
            userToInput[userId].gameObject.SetActive(true);
        }

        // Make sure the member have the right data
        // So all members have a ship, that can take in there TransformData

        //byte[] data = new byte[10]; // Change to a struct of what we will send
        //discordNetworkLayerService.SendMessegeToOneUser(userId, NetworkChannel.CONTROLLER_SYNC, data); // Maybe a new NetworkChannel?
    }

    public void SpawnInShipsForAllMembers()
    {
        //Remove Previous instances if there were any
        foreach (var item in userToInput)
        {
            if (item.Value != null)
            {
                Destroy(item.Value.gameObject);
            }
        }
        userToInput.Clear();
        //Run for the current lobby
        //Spawn in a ship for each member on the determined spawn locations given from LevelGenerator
        IEnumerable<Discord.User> listOfUsers = lobbyService.GetLobbyMembers();
        if(listOfUsers == null)
        {
            return;
        }

        foreach (var user in listOfUsers)
        {
            Vector3 position = GetAvailableSpawnPoint();
            //Choose a spawnLocation

            if (position == Vector3.down)
            {
                // Add the user to a waiting list, or try find a suitable spot to spawn the user in.
                continue;
            }
            
            SpawnInOneShipForUser(user.Id, position);
        }

    }
    private Vector3 GetAvailableSpawnPoint()
    {
        //Check a spawn point if its available
        for (int i = 0; i < level.spawnpoints.Count; i++)
        {
            if (!IsAShipNearSpawnPoint(level.spawnpoints[i].transform.position))
            {
                return level.spawnpoints[i].transform.position;
            }
        }

        return Vector3.down;
    }
    private bool IsAShipNearSpawnPoint(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, placementRules.searchRadius);
        foreach (var item in hits)
        {
            if (item.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    public void SpawnInOneShipForUser(long userID, Vector3 position)
    {
        if (userToInput.ContainsKey(userID))
        {
            return;
        }
        GameObject obj = Instantiate(shipPrefab, position, Quaternion.identity);
        InputHandler inputHandler = obj.GetComponent<InputHandler>();
        inputHandler.userID = userID;
        userToInput.Add(userID, inputHandler);
        if(userID == DiscordManager.CurrentUser.Id)
        {
            FindObjectOfType<UserData>().controller = inputHandler;
        }
        if(lobbyService.IsTheHost())
        {
            //discordNetworkLayerService.SendMessegeToAllOthers(NetworkChannel.OBJECT_POSITION)
        }
        //Send A package that we have spawned in a Ship for User
    }
}
