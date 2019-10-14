using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LevelGenerator))]
public class SpawnLocationHandler : MonoBehaviour
{

    public static SpawnLocationHandler INSTANCE;
    public GameObject shipPrefab;
    public GameObject spawnPointPrefab;
    public Sample placementRules;

    public static Dictionary<long, InputHandler> userToInput = new Dictionary<long, InputHandler>();
    DiscordLobbyService lobbyService;
    DiscordNetworkLayerService discordNetworkLayerService;
    LevelGenerator level;

    public bool Debugging = false;
    private void Awake()
    {
        if(INSTANCE)
        {
            Destroy(this);
        }
        else
        {
            INSTANCE = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        discordNetworkLayerService = GetComponent<DiscordNetworkLayerService>();
        level = GetComponent<LevelGenerator>();
        lobbyService = DiscordLobbyService.INSTANCE;
        lobbyService.OnMemberConnect += LobbyManager_OnMemberConnect;
        lobbyService.OnMemberDisconnect += LobbyManager_OnMemberDisconnect;
        GameManager.OnJoinedLobby += SpawnInShipsForAllMembers;
        //SpawnInShipsForAllMembers();
    }

    private void LobbyManager_OnMemberDisconnect(long lobbyId, long userId)
    {
        if (Debugging)
            Debug.Log("Lobby ID: " + lobbyId + "-  User ID: " + userId + " - Member Disconnected");
        if (userToInput.ContainsKey(userId))
            userToInput[userId].gameObject.SetActive(false);
        //Disable the ship controlled by the user, if the user is not connected after a certain amount, we will destroy the ship and its data.         
    }

    private void LobbyManager_OnMemberConnect(long lobbyId, long userId)
    {
        if (Debugging)
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
    public void RequestForAllShipPositions()
    {
        if (!DiscordLobbyService.INSTANCE.IsTheHost())
        {
            Debug.Log("Request to for Positions for Ships");
            ShipTransformRequest request = new ShipTransformRequest(DiscordManager.CurrentUser.Id);
            DiscordNetworkLayerService.INSTANCE.SendMessegeToOwnerOfLobby(NetworkChannel.SHIPTRANSFORM_SYNC_REQUEST, request.ToBytes());
        }
    }
    public static void RequestFromMemberOfShipPositions(long userId)
    {
        if (DiscordLobbyService.INSTANCE.IsTheHost())
        {
            foreach (var item in userToInput)
            {
                TransformDataPackage transformData = new TransformDataPackage(item.Value.transform.position, item.Value.transform.rotation, item.Key);
                DiscordNetworkLayerService.INSTANCE.SendMessegeToOneUser(userId, NetworkChannel.SHIP_TRANSFORM, transformData.ToBytes());
            }
        }
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
        if (listOfUsers != null)
        {
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

        if(!DiscordLobbyService.IsOnline)
        {
            Vector3 position = GetAvailableSpawnPoint();
            SpawnInOneShipForUser(DiscordManager.CurrentUser.Id, position);
        }

        GameManager.Instance.gameState = GameManager.GameState.GAME_IN_SESSION;
        Invoke("RequestForAllShipPositions", 0.5f);
    }
    private Vector3 GetAvailableSpawnPoint()
    {
        //Check a spawn point if its available
        Vector3 foundPosition = Vector3.down;
        int numberOfTries = 0;
        do
        {
            int spawnPointIndex = UnityEngine.Random.Range(0, level.spawnpoints.Count);
            if (!IsAShipNearSpawnPoint(level.spawnpoints[spawnPointIndex].transform.position))
            {
                foundPosition = level.spawnpoints[spawnPointIndex].transform.position;
            }
        } while (foundPosition == Vector3.down && numberOfTries >= level.spawnpoints.Count);

        return foundPosition;
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
        inputHandler.UserId = userID;
        userToInput.Add(userID, inputHandler);
        if (userID == DiscordManager.CurrentUser.Id)
        {
            FindObjectOfType<UserData>().controller = inputHandler;
        }
        if (lobbyService.IsTheHost())
        {
            //discordNetworkLayerService.SendMessegeToAllOthers(NetworkChannel.OBJECT_POSITION)
        }
        //Send A package that we have spawned in a Ship for User
    }

    public void Respawn(Ship ship)
    {

        StartCoroutine(Respawning(ship));

    }

    IEnumerator Respawning(Ship ship)
    {
        yield return new WaitForSeconds(1);
        if (DiscordLobbyService.INSTANCE.IsTheHost())
        {
            Vector3 respawnPosition = GetAvailableSpawnPoint();
            ship.transform.position = respawnPosition;
        }
        ship.gameObject.SetActive(true);

        yield break;
    }
}
public struct ShipTransformRequest
{
    public long id;
    public ShipTransformRequest(long id)
    {
        this.id = id;

    }
    public ShipTransformRequest(byte[] data)
    {
        id = BitConverter.ToInt64(data, 0);

    }
    public byte[] ToBytes()
    {
        List<byte> vs = new List<byte>();
        vs.AddRange(BitConverter.GetBytes(id));

        return vs.ToArray();
    }
}