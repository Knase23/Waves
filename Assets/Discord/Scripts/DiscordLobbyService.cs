using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Discord;


public class DiscordLobbyService : MonoBehaviour
{
    public static DiscordLobbyService INSTANCE;

    public bool Debugging = false;
    public bool DebugOnMessege = false;
    //Add delegates so we can change other items when something happens when something happens

    private LobbyManager lobbyManager;
    public UserManager userManager;

    [SerializeField]
    private long currentLobbyId = 0;
    public long CurrentLobbyId
    {
        get { return currentLobbyId; }
        set { currentLobbyId = value; }
    }
    private static bool isOnline = false;
    public static bool IsOnline
    {
        get { return isOnline; }
        set
        {
            isOnline = value;
            GameManager.CheckOnline();
        }
    }

    [SerializeField]
    private string currentSecret;
    public string CurrentSecret
    {
        get { return currentSecret; }
        set { currentSecret = value; }
    }

    [SerializeField]
    private long currentLobbyOwnerId;
    public long CurrentLobbyOwnerId
    {
        get { return currentLobbyOwnerId; }
        set { currentLobbyOwnerId = value; }
    }
    Coroutine coroutine;

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

    //public delegate void MemberDisconnect(long lobbyId);
    public event LobbyManager.LobbyUpdateHandler OnLobbyUpdate;

    public event LobbyManager.LobbyDeleteHandler OnLobbyDelete;

    public event LobbyManager.MemberConnectHandler OnMemberConnect;

    public event LobbyManager.MemberUpdateHandler OnMemberUpdate;

    public event LobbyManager.MemberDisconnectHandler OnMemberDisconnect;

    public event LobbyManager.LobbyMessageHandler OnLobbyMessage;

    public event LobbyManager.SpeakingHandler OnSpeaking;

    public event LobbyManager.NetworkMessageHandler OnNetworkMessage;
    // Start is called before the first frame update
    void Start()
    {
        lobbyManager = DiscordManager.INSTANCE.GetDiscord().GetLobbyManager();
        userManager = DiscordManager.INSTANCE.GetDiscord().GetUserManager();
        lobbyManager.OnLobbyUpdate += LobbyUpdate;
        lobbyManager.OnLobbyUpdate += DiscordActivityService.INSTANCE.OnLobbyUpdate;
        lobbyManager.OnMemberConnect += MemberConnect;
        lobbyManager.OnMemberUpdate += MemberUpdate;
        lobbyManager.OnMemberDisconnect += MemberDisconnect;
        lobbyManager.OnNetworkMessage += NetworkMessage;
        lobbyManager.OnSpeaking += LobbyManager_OnSpeaking;
        lobbyManager.OnLobbyMessage += LobbyManager_OnLobbyMessage;
        lobbyManager.OnLobbyDelete += LobbyManager_OnLobbyDelete;
    }

    private void LobbyManager_OnLobbyDelete(long lobbyId, uint reason)
    {
        OnLobbyDelete?.Invoke(lobbyId, reason); 
    }

    private void LobbyManager_OnLobbyMessage(long lobbyId, long userId, byte[] data)
    {
        OnLobbyMessage?.Invoke(lobbyId, userId, data);
    }

    private void LobbyManager_OnSpeaking(long lobbyId, long userId, bool speaking)
    {
        OnSpeaking?.Invoke(lobbyId, userId, speaking);
    }

    private void NetworkMessage(long lobbyId, long userId, byte channelId, byte[] data)
    {
        OnNetworkMessage?.Invoke(lobbyId, userId, channelId, data);
    }

    private void MemberConnect(long lobbyId, long userId)
    {
        if(DebugOnMessege)
            Debug.Log("MemberConnected "+ userId);

        if (userId != DiscordManager.CurrentUser.Id && lobbyManager.MemberMetadataCount(lobbyId, userId) >= 2)
        {
            if (DebugOnMessege)
                Debug.Log("Try doing EstablishConnectionWithMember " + userId);
            DiscordNetworkLayerService.INSTANCE.EstablishConnectionWithMember(lobbyId, userId);
        }
        OnMemberConnect?.Invoke(lobbyId,userId);
    }
    private void MemberUpdate(long lobbyId, long userId)
    {
        //Debug.Log("MemberUpdate " + userId);
        if (userId != DiscordManager.CurrentUser.Id && lobbyManager.MemberMetadataCount(lobbyId, userId) >= 2)
        {
            if (DebugOnMessege)
                Debug.Log("Try doing EstablishConnectionWithMember " + userId);
            DiscordNetworkLayerService.INSTANCE.UpdateAPeer(lobbyId, userId);
        }
        OnMemberUpdate?.Invoke(lobbyId, userId);
    }
    private void MemberDisconnect(long lobbyId, long userId)
    {
        if (DebugOnMessege)
            Debug.Log("MemberDisconnect " + userId);

        DiscordNetworkLayerService.INSTANCE.DisconnectPeer(userId);

        if (currentLobbyOwnerId == userId)
        {
            //Give hosting to someone else and join them
            DisconnectLobby();
        }
        OnMemberDisconnect?.Invoke(lobbyId, userId);
    }

    private void Update()
    {
        if (coroutine == null && IsOnline)
        {
            coroutine = StartCoroutine(UpdateLobbyTransaction());
        }
        CheckOnlineStatus();
    }
    private void CheckOnlineStatus()
    {
        if (CurrentLobbyId != 0)
        {
            if (!isOnline)
            {
                IsOnline = true;
            }
        }
        else
        {
            if (isOnline)
            {
                IsOnline = false;
            }
        }
    }
    private void LobbyUpdate(long lobbyId)
    {
        foreach (var member in GetLobbyMembers())
        {
            if (member.Id != DiscordManager.CurrentUser.Id && lobbyManager.MemberMetadataCount(lobbyId, member.Id) >= 2)
                DiscordNetworkLayerService.INSTANCE.EstablishConnectionWithMember(lobbyId, member.Id);
        }

        OnLobbyUpdate(lobbyId);
    }
    // Functions to Use to Call from other scripts
    public void CreateLobby()
    {
        if (IsOnline)
            return;

        var txn = lobbyManager.GetLobbyCreateTransaction();
        txn.SetCapacity(10);
        txn.SetType(LobbyType.Public);
        lobbyManager.CreateLobby(txn, (Result result, ref Lobby lobby) =>
        {
            SetCurrent(lobby.Id, lobby.Secret, lobby.OwnerId);
            NewUpdateLobbyTransaction();
        });
    }
    public void UpdateLobbySize(uint numberOfLocalPlayersConnected)
    {
        if (!IsOnline)
            return;
        var transaction = lobbyManager.GetLobbyUpdateTransaction(CurrentLobbyId);
        transaction.SetCapacity(5 - numberOfLocalPlayersConnected);
        lobbyManager.UpdateLobby(CurrentLobbyId, transaction, (Result result) =>
         {
             if (result != Result.Ok)
                 Debug.Log(result);
         });
    }

    public void DisconnectLobby()
    {
        if (!IsOnline)
            return;
        if(Debugging)
            Debug.Log("Try to leave lobby");
        lobbyManager.DisconnectLobby(CurrentLobbyId, (Result result) =>
        {
            if (result != Result.Ok)
            {
                Debug.Log(result);
            }
            else
            {
                Debug.Log("Left Lobby");
                SetCurrent(0, string.Empty, 0);
            }
        });

        DiscordActivityService.INSTANCE.Activity(new DiscordActivityService.ActivityInformation());
    }
    public void RemoveLobby()
    {
        if (!IsOnline)
            return;

        if (CurrentLobbyOwnerId != DiscordManager.CurrentUser.Id)
            return;

        lobbyManager.DeleteLobby(CurrentLobbyId, (result) =>
        {
            if (result != Result.Ok)
                Debug.Log(result);
            else
            {
                Debug.Log("Deleted Lobby");
            }
        });
    }
    public void ConnectToLobby()
    {
        var l = GetLobby();
        lobbyManager.ConnectLobby(l.Id, l.Secret, (Result result, ref Lobby lobby) =>
        {
            if (result == Result.Ok)
            {
                SetCurrent(lobby.Id, lobby.Secret, lobby.OwnerId);
            }
            else
            {
                //Debug.Log(result);
            }

        });
    }
    public void ConnectToLobbyWithActivitySecret(string activitySecret)
    {
        lobbyManager.ConnectLobbyWithActivitySecret(activitySecret, (Result result, ref Lobby lobby) =>
        {
            //Debug.Log("ConnectToLobbyThroughSecret");
            if (result == Result.Ok)
            {
                SetCurrent(lobby.Id, lobby.Secret, lobby.OwnerId);
                DiscordNetworkLayerService.INSTANCE.SetMyPeerId();
            }
            else
            {
                Debug.Log(result);
            }
        });
    }


    //Getters

    public Lobby GetLobby()
    {
        if (lobbyManager == null || !IsOnline || CurrentLobbyId == 0)
            return new Lobby();

        return lobbyManager.GetLobby(CurrentLobbyId);
    }
    public int GetMemberCount()
    {
        if (lobbyManager == null || CurrentLobbyId == 0)
            return 0;

        return lobbyManager.MemberCount(CurrentLobbyId);
    }
    public IEnumerable<User> GetLobbyMembers()
    {
        if (!IsOnline || CurrentLobbyId == 0)
            return null;

        return lobbyManager.GetMemberUsers(CurrentLobbyId);
    }
    public User GetUser(long userId)
    {
        foreach (var item in GetLobbyMembers())
        {
            if(item.Id == userId)
            {
                return item;
            }

        }
        return new User();
    }

    // Statements 
    public bool IsTheHost()
    {
        return CurrentLobbyOwnerId == 0 || userManager.GetCurrentUser().Id == CurrentLobbyOwnerId;
    }
    //Functions for this script
    private void SetCurrent(long lobbyId, string secret, long ownerId)
    {
        CurrentLobbyId = lobbyId;
        CurrentSecret = secret;
        CurrentLobbyOwnerId = ownerId;
    }


    IEnumerator UpdateLobbyTransaction()
    {
        yield return new WaitForSecondsRealtime(0.6f);
        NewUpdateLobbyTransaction();
        coroutine = null;
        yield break;
    }

    void NewUpdateLobbyTransaction()
    {
        if (!IsOnline)
            return;

        var transaction = lobbyManager.GetLobbyUpdateTransaction(CurrentLobbyId);

        #region Set Meta Data For Lobby
        if (SceneManager.GetActiveScene().name == "Game")
        {
            transaction.SetLocked(true);
        }
        else
        {
            transaction.SetLocked(false);
        }
        #endregion

        lobbyManager.UpdateLobby(CurrentLobbyId, transaction, (newResult) =>
        {
            if (newResult == Result.Ok)
            {
                //Debug.Log("Lobby updated");
            }
            else
            {
                //Debug.Log(newResult, gameObject);
            }
        });
    }

    public void SetMetaDataOfMember(long userid, string key, string value)
    {
        if (userid == 0)
            return;
        if (Debugging)
            Debug.Log("SetMetaData: UserID: " + userid + " Key: " + key + " Value: " + value);
        var memberTransaction = lobbyManager.GetMemberUpdateTransaction(CurrentLobbyId, userid);
        memberTransaction.SetMetadata(key, value);
        lobbyManager.UpdateMember(CurrentLobbyId, userid, memberTransaction, (result) =>
           {
               if (result != Result.Ok)
                   Debug.Log(result);
           });

    }
    public void SetMyMetaData(string key, string value)
    {
        if (!IsOnline)
        {
            return;
        }
        if (Debugging)
            Debug.Log("SetMetaData: UserID: " + DiscordManager.CurrentUser.Id + " Key: " + key + " Value: " + value);
        var memberTransaction = lobbyManager.GetMemberUpdateTransaction(CurrentLobbyId, DiscordManager.CurrentUser.Id);
        memberTransaction.SetMetadata(key, value);
        lobbyManager.UpdateMember(CurrentLobbyId, DiscordManager.CurrentUser.Id, memberTransaction, (result) =>
        {
            if (result != Result.Ok)
                Debug.Log(result);
        });
        //Debug.Log("Setted: " + key + " : " + value);
    }
    public string GetMetaDataOfMember(long userid, string key)
    {
        return lobbyManager.GetMemberMetadataValue(CurrentLobbyId, userid, key);
    }
}
