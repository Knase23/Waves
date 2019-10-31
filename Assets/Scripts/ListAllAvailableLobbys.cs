using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListAllAvailableLobbys : MonoBehaviour
{
    public GameObject lobbyViewPrefab;

    public Dictionary<long, LobbyElement> shownLobbys = new Dictionary<long, LobbyElement>();
    void Start()
    {
        //DiscordLobbyService.INSTANCE.OnLobbyUpdate += LobbyManager_OnLobbyUpdate;
    }

    public void ViewAllAvailableLobbies()
    {
        var lobbyManger = DiscordManager.INSTANCE.GetDiscord().GetLobbyManager();

        Discord.LobbySearchQuery searchQuery = lobbyManger.GetSearchQuery();
        searchQuery.Limit(25);
        
        lobbyManger.Search(searchQuery,(result) =>
        {
            if(result == Discord.Result.Ok)
            {
                var previousShownLobbys = new Dictionary<long, LobbyElement>(shownLobbys);
                shownLobbys = new Dictionary<long, LobbyElement>();
                var count = lobbyManger.LobbyCount();
                for (int i = 0; i < count; i++)
                {
                    long lobbyId = lobbyManger.GetLobbyId(i);
                    if (previousShownLobbys.ContainsKey(lobbyId))
                    {
                        shownLobbys.Add(lobbyId, previousShownLobbys[lobbyId]);
                        continue;
                    }
                    GameObject a = Instantiate(lobbyViewPrefab, transform);
                    LobbyElement b = a.GetComponent<LobbyElement>();
                    b.SetLobbyId(lobbyManger.GetLobbyId(i));
                    shownLobbys.Add(lobbyManger.GetLobbyId(i), b);
                }
            }
        });
    }
    private void LateUpdate()
    {   
        ViewAllAvailableLobbies();
    }
}
