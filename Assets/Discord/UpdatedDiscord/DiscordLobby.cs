namespace JesperDiscord
{
using Discord;
    
    public static class DiscordLobby
    {
        private static LobbyManager manager;
        private static long lobbyID = 0;
        public static long CurrentLobbyID
        { get { return lobbyID; } }
        private static long lobbyOwner = 0;
        public static long CurrentLobbyOwner
        { get { return lobbyOwner; } }
        private static string lobbySecret;
        public static string CurrentSecret
        { get { return lobbySecret; } }
        private static bool isOnline;
        public static bool IsOnline
        { get { return isOnline; } }

        public static void Initialize()
        {
            Discord discord = DiscordManager.GetDiscord();
            manager = discord.GetLobbyManager();

            manager.OnLobbyDelete += OnLobbyDelete;
            manager.OnLobbyMessage += OnLobbyMessage;
            manager.OnLobbyUpdate += OnLobbyUpdate;
            manager.OnMemberConnect += OnMemberConnect;
            manager.OnMemberDisconnect += OnMemberDisconnect;
            manager.OnMemberUpdate += OnMemberUpdate;
            manager.OnNetworkMessage += OnNetworkMessage;
            manager.OnSpeaking += OnSpeaking;

        }
        private static void SetCurrentLobby(long ID,string secret, long ownerID)
        {
            lobbyID = ID;
            lobbySecret = secret;
            lobbyOwner = ownerID;
            CheckOnlineStatus();
        }

        public static void CreateLobby(uint capacity = 10, LobbyType type = LobbyType.Public)
        {
            if (CheckOnlineStatus()) return;

            var transaction = manager.GetLobbyCreateTransaction();
            transaction.SetCapacity(capacity);
            transaction.SetType(type);
            manager.CreateLobby(transaction, (Result result, ref Lobby lobby) => 
            {
                SetCurrentLobby(lobby.Id, lobby.Secret, lobby.OwnerId);
                // Messege that we Joined a lobby;
                UpdateLobbyTransaction();
            });
        }
        public static void DisconnectLobby()
        {
            if (!CheckOnlineStatus()) return;

            if (manager.MemberCount(CurrentLobbyID) <= 1)
            {
                //Remove lobby
                return;
            }
            manager.DisconnectLobby(CurrentLobbyID, (Result result) =>
             {
                 if (result == Result.Ok)
                 {
                     SetCurrentLobby(0, string.Empty, 0);
                    //Message that we left a Lobby
                }

             });
        }
        public static void RemoveLobby()
        {
            if (!CheckOnlineStatus()) return;

            manager.DeleteLobby(CurrentLobbyID, (result) =>
             {
                 if (result == Result.Ok)
                 {
                     SetCurrentLobby(0, string.Empty, 0);
                    //Message that we left a Lobby
                }
             });
        }

        public static void ConnectLobby(long ID, string secret)
        {
            manager.ConnectLobby(ID, secret, (Result result, ref Lobby lobby) =>
              {
                  if(result == Result.Ok)
                  {
                      SetCurrentLobby(lobby.Id, lobby.Secret, lobby.OwnerId);
                      // Message that we Joined a lobby
                  }
              });
        }
        public static void ConnectLobbyActivitySecret(string activitySecret)
        { 
            manager.ConnectLobbyWithActivitySecret(activitySecret, (Result result, ref Lobby lobby) =>
             {
                 if(result == Result.Ok)
                 {
                     SetCurrentLobby(lobby.Id, lobby.Secret, lobby.OwnerId);
                     // Message that we Joined a lobby
                 }
             });
        }
        public static void SetMetaDataOfMember(long userID,string key, string value)
        {
            if (userID == 0) return;
            if (!CheckOnlineStatus()) return;

            var memberTransaction = manager.GetMemberUpdateTransaction(CurrentLobbyID, userID);
            memberTransaction.SetMetadata(key, value);
            manager.UpdateMember(CurrentLobbyID, userID, memberTransaction, (result) =>
               {
                   if (result != Result.Ok)
                   {
                       return;
                   }
               });
        }
        public static void SetLobbyData(Lobby lobby)
        {
            if (!CheckOnlineStatus()) return;

            var transaction = manager.GetLobbyUpdateTransaction(CurrentLobbyID);
            transaction.SetCapacity(lobby.Capacity);
            transaction.SetOwner(lobby.OwnerId);
            transaction.SetType(lobby.Type);
            transaction.SetLocked(lobby.Locked);
            manager.UpdateLobby(CurrentLobbyID, transaction, (result) =>
              {
                  if(result != Result.Ok)
                  {
                      return;
                  }
              });
        }
        public static void SetLobbyMetaDat(string key, string value)
        {
            if (!CheckOnlineStatus()) return;
            var transaction = manager.GetLobbyUpdateTransaction(CurrentLobbyID);
            transaction.SetMetadata(key, value);
            manager.UpdateLobby(CurrentLobbyID, transaction, (result) =>
            {
                if (result != Result.Ok)
                {
                    return;
                }
            });
        }

        private static void UpdateLobbyTransaction()
        {
            if (!CheckOnlineStatus())
                return;
            var transaction = manager.GetLobbyUpdateTransaction(CurrentLobbyID);

            manager.UpdateLobby(CurrentLobbyID, transaction, (newResult) =>
            {
                if (newResult == Result.Ok || newResult == Result.InternalError)
                {
                    //Debug.Log("Lobby updated");
                }
                
            });
        }

        private static bool CheckOnlineStatus()
        {
            if(CurrentLobbyID != 0)
            {
                if(!IsOnline)
                {
                    isOnline = true;
                }
            }
            else
            {
                if(IsOnline)
                {
                    isOnline = false;
                    
                }
            }
            return IsOnline;
        }
        public static bool IsHost()
        {
            return !CheckOnlineStatus() && DiscordManager.GetLocalUser().Id == CurrentLobbyOwner;
        }

        #region DISCORD EVENT HANDELING
        private static void OnSpeaking(long lobbyId, long userId, bool speaking)
        {
            throw new System.NotImplementedException();
        }

        private static void OnNetworkMessage(long lobbyId, long userId, byte channelId, byte[] data)
        {
            throw new System.NotImplementedException();
        }

        private static void OnMemberUpdate(long lobbyId, long userId)
        {
            throw new System.NotImplementedException();
        }

        private static void OnMemberDisconnect(long lobbyId, long userId)
        {
            throw new System.NotImplementedException();
        }

        private static void OnMemberConnect(long lobbyId, long userId)
        {
            throw new System.NotImplementedException();
        }

        private static void OnLobbyUpdate(long lobbyId)
        {
            throw new System.NotImplementedException();
        }

        private static void OnLobbyMessage(long lobbyId, long userId, byte[] data)
        {
            throw new System.NotImplementedException();
        }

        private static void OnLobbyDelete(long lobbyId, uint reason)
        {
            throw new System.NotImplementedException();
        }
        #endregion


    }
}
