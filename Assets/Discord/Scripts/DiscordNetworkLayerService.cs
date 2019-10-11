using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
/// <summary>
/// A Singlton, handles Networking with the help of Discord's own SDK
/// </summary>
public class DiscordNetworkLayerService : MonoBehaviour
{
    public static DiscordNetworkLayerService INSTANCE;

    public bool Debugging = false;
    public bool DebugOnMessege = false;

    NetworkManager manager;

    ulong myPeerId;
    List<ulong> othersUserPeerIds = new List<ulong>();
    Dictionary<long, ulong> memberIdToPeerId = new Dictionary<long, ulong>();
    string myRoute;

    private void Awake()
    {
        if (INSTANCE)
        {
            Destroy(this);
        }
        else
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        manager = DiscordManager.INSTANCE.GetDiscord().GetNetworkManager();
        manager.OnRouteUpdate += OnRouteUpdate;
        manager.OnMessage += OnMessage;
        myPeerId = manager.GetPeerId();
        GameManager.OnJoinedLobby += SetMyPeerId;
    }
    /// <summary>
    /// Takes a Message and does something with it. Here is were most MAGIC happens
    /// </summary>
    /// <param name="peerId"></param>
    /// <param name="channelId"></param>
    /// <param name="data"></param>
    private void OnMessage(ulong peerId, byte channelId, byte[] data)
    {
        NetworkChannel channel = (NetworkChannel)channelId;
        //Debug.Log("Got Messege from: " + peerId + " on Channel: " + channel);
        switch (channel)
        {
            case NetworkChannel.INPUT_DATA:
                // If the data is from a client
                // Do stuff with it.
                InputData iData = new InputData(data);
                if(DebugOnMessege)
                    Debug.Log("Got Input Data from: " + iData.id);

                if (SpawnLocationHandler.userToInput.ContainsKey(iData.id))
                {
                    SpawnLocationHandler.userToInput[iData.id].ReciveInputData(iData);
                }
                else
                {
                    Debug.Log("Could not find Key");
                }
                break;
            case NetworkChannel.LOADSCENE:
                // If the data is from a host
                // LoadScene and wait for map to be generated.
                LoadScenePackage loadScenePackage = new LoadScenePackage(data);
                GameManager.Instance.LoadScene(loadScenePackage);
                break;
            case NetworkChannel.SHIP_TRANSFORM:
                // If the data is from the host
                // Update the position of those ships position object
                TransformDataPackage transformData = new TransformDataPackage(data);
                if (DebugOnMessege)
                    Debug.Log("Got Transform Data for: " + transformData.id);
                if (SpawnLocationHandler.userToInput.ContainsKey(transformData.id))
                {
                    SpawnLocationHandler.userToInput[transformData.id].shipControl.ChangeTransform(transformData);
                }
                else
                {
                    Debug.Log("Could not find Key");
                }

                break;
            case NetworkChannel.SHIPTRANSFORM_SYNC_REQUEST:
                // If the data is from a client
                // Send data to it
                ShipTransformRequest request = new ShipTransformRequest(data);
                SpawnLocationHandler.RequestFromMemberOfShipPositions(request.id);

                break;
            case NetworkChannel.SCORE_SYNC:
                //If the data is from the host
                // Make the score display the right numbers for each player.
                break;

            case NetworkChannel.SPAWN_IN_OBJECT:
                //If the data is from the host
                // Make the score display the right numbers for each player.
                Debug.Log("Spawn in Object");
                TransformDataPackage transformPackage = new TransformDataPackage(data);
                SpawnObject.INSTANCE.SpawnInObject(transformPackage);

                break;
            case NetworkChannel.ACTION_TRIGGER:
                //If the data is from the host
                // Activate the action of that inputHandler for that user that wanted to do that action.
                TriggerActionPackage actionPackage = new TriggerActionPackage(data);
                SpawnLocationHandler.userToInput[actionPackage.userId].ProcessTriggerActionPackage(actionPackage);
                break;
            case NetworkChannel.START_LOADING_MAP:
                //If the data is from the host
                // Make the score display the right numbers for each player.
                Debug.Log("Got Start Loading_Map");
                LevelDetailsPackage levelDetailsPackage = new LevelDetailsPackage(data);
                LoadGameIn loadGameIn = FindObjectOfType<LoadGameIn>();
                loadGameIn.GetMapDetails(levelDetailsPackage);

                //Setup Loading screen for level

                break;
            default:
                Debug.Log(peerId + " : Sent messege was not reognized");
                break;
        }
    }
    /// <summary>
    /// Makes a connection to the provided member in specified lobby
    /// </summary>
    /// <param name="lobbyId">Lobby to find Metadata for the member</param>
    /// <param name="member">Member Id of the User</param>
    public void EstablishConnectionWithMember(long lobbyId, long member)
    {
        if(Debugging)
            Debug.Log("Trying to make Connection with: " + member);
        #region Setup and Error Handling
        if (member == DiscordManager.CurrentUser.Id)
        {
            if(Debugging)
                Debug.LogWarning("Tried to connect to self, but we dissmissed it");
            return;
        }

        if (memberIdToPeerId.ContainsKey(member))
        {
            if(Debugging)
                Debug.LogWarning("Already have connection to: " + member);
            return;
        }
        DiscordLobbyService.INSTANCE.SetMyMetaData("peer_id", myPeerId.ToString());
        DiscordLobbyService.INSTANCE.SetMyMetaData("route", myRoute);

        LobbyManager lobbyManager = DiscordManager.INSTANCE.GetDiscord().GetLobbyManager();
        ulong peer_id = ulong.Parse(lobbyManager.GetMemberMetadataValue(lobbyId, member, "peer_id"));
        string route = lobbyManager.GetMemberMetadataValue(lobbyId, member, "route");
        #endregion

        manager.OpenPeer(peer_id, route);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.INPUT_DATA, false);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.LOADSCENE, true);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.SHIPTRANSFORM_SYNC_REQUEST, true);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.SCORE_SYNC, true);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.SHIP_TRANSFORM, false);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.ACTION_TRIGGER, true);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.START_LOADING_MAP, true);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.SPAWN_IN_OBJECT, true);

        if (!othersUserPeerIds.Contains(peer_id))
        {
            othersUserPeerIds.Add(peer_id);
            memberIdToPeerId.Add(member, peer_id);
        }
        Debug.Log("Connection with: " + member);
    }
    #region Code That is a must, and is working as far as i know
    private void LateUpdate()
    {
        manager.Flush();
    }
    
    private void OnRouteUpdate(string routeData)
    {
        myRoute = routeData;
        DiscordLobbyService.INSTANCE.SetMyMetaData("route", routeData);
    }

    /// <summary>
    /// Sets the Networking MetaData for this client.
    /// </summary>
    public void SetMyPeerId()
    {
        DiscordLobbyService.INSTANCE.SetMyMetaData("peer_id", myPeerId.ToString());
        DiscordLobbyService.INSTANCE.SetMyMetaData("route", myRoute);
    }

    /// <summary>
    /// Sends a Network to everyone that is connected to me
    /// </summary>
    /// <param name="networkChannel"> The kind of data we are sending </param>
    /// <param name="data">The actual Data</param>
    /// <returns></returns>
    public bool SendMessegeToAllOthers(NetworkChannel networkChannel, byte[] data)
    {
        bool anyMessageSent = false;
        foreach (var peerId in othersUserPeerIds)
        {
            SendMessegeToPeer(peerId, networkChannel, data);
            anyMessageSent = true;
        }
        return anyMessageSent;
    }

    /// <summary>
    /// Sends a Network package to the owner of the lobby
    /// </summary>
    /// <param name="networkChannel"> The kind of data we are sending </param>
    /// <param name="data">The actual Data</param>
    /// <returns></returns>
    public bool SendMessegeToOwnerOfLobby(NetworkChannel networkChannel, byte[] data)
    {
        return SendMessegeToOneUser(DiscordLobbyService.INSTANCE.CurrentLobbyOwnerId, networkChannel,data);
    }

    /// <summary>
    /// Send a Network package to a spesific user in the lobby
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="networkChannel"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool SendMessegeToOneUser(long userId,NetworkChannel networkChannel, byte[] data)
    {
        ulong peerId;
        if (memberIdToPeerId.TryGetValue(userId, out peerId))
        {
            SendMessegeToPeer(peerId, networkChannel, data);
            return true;
        }
        return false;
    }

    private void SendMessegeToPeer(ulong peerId, NetworkChannel networkChannel, byte[] data)
    {
        manager.SendMessage(peerId, (byte)networkChannel, data);
    }
    

    /// <summary>
    /// Updates so the route to the connection is correct
    /// </summary>
    /// <param name="lobbyId"></param>
    /// <param name="member"></param>
    public void UpdateAPeer(long lobbyId, long member)
    {
        if (!memberIdToPeerId.ContainsKey(member))
        {
            EstablishConnectionWithMember(lobbyId, member);
            return;
        }
        try
        {
            LobbyManager lobbyManager = DiscordManager.INSTANCE.GetDiscord().GetLobbyManager(); 
            ulong peer_id = ulong.Parse(lobbyManager.GetMemberMetadataValue(lobbyId, member, "peer_id"));
            string route = lobbyManager.GetMemberMetadataValue(lobbyId, member, "route");

            manager.UpdatePeer(peer_id, route);
        }
        catch (ResultException result)
        {
            if(result.Result != Result.InternalError && result.Result != Result.Ok)
            {
                Debug.LogError(result.Result);
            }
        }
        
    }
    
    /// <summary>
    /// Disconnects a connection with a member
    /// </summary>
    /// <param name="member"></param>
    public void DisconnectPeer(long member)
    {
        ulong peer_id;
        if (memberIdToPeerId.TryGetValue(member, out peer_id))
        {
            manager.ClosePeer(peer_id);
            othersUserPeerIds.Remove(peer_id);
            memberIdToPeerId.Remove(member);
        }
    }
    #endregion


}
/// <summary>
/// Describes what kind of data is going through channel
/// </summary>
public enum NetworkChannel
{
    /// <summary>
    /// Handeling the input from client to host
    /// </summary>
    INPUT_DATA = 1,
    
    /// <summary>
    /// Load a scene and sends the correct map data
    /// </summary>
    LOADSCENE,
    
    /// <summary>
    /// The position for a given character/ship
    /// </summary>
    SHIP_TRANSFORM,
    
    /// <summary>
    /// A Request for the host to Send out all Ships Transforms to the clients  
    /// </summary>
    SHIPTRANSFORM_SYNC_REQUEST,
    
    /// <summary>
    /// Syncing so the score is displayed right
    /// </summary>
    SCORE_SYNC,

    /// <summary>
    /// Host Spawned in a object, client spawn exactly the samething!!!
    /// </summary>
    SPAWN_IN_OBJECT,
    
    /// <summary>
    /// When the host executes a action, the client will get whom used a action
    /// </summary>
    ACTION_TRIGGER,

    /// <summary>
    /// A Package that gives the clients the expected amount of objects to spawn in
    /// </summary>
    START_LOADING_MAP,
    /// <summary>
    /// A Package gives the Host a clear ahead for that user to start playing
    /// </summary>
    USER_DONE_LOADING_MAP,
    /// <summary>
    /// A Package from the Host to start the game Session for the users. AKA they see there ship and can start moving.
    /// </summary>
    START_GAME,

        

}