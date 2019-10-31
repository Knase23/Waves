using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyElement : MonoBehaviour
{
    public TextMeshProUGUI lobbyName;
    public Button joinButton;
    private long lobbyId;
    private void FixedUpdate()
    {
        UpdateView();
    }
    void UpdateView()
    {
        Discord.Lobby lobby = DiscordLobbyService.INSTANCE.GetLobby(lobbyId);

        if (lobby.Id == 0)
        {
            Destroy(gameObject);
            return;
        }

        try
        {
            string lobbyTitle = DiscordLobbyService.INSTANCE.GetLobbyMetaData(lobbyId, "title");
            lobbyName.SetText(lobbyTitle);
        }
        catch (System.Exception)
        {
            lobbyName.SetText(lobbyId.ToString());
        }

    }
    public void SetLobbyId(long id)
    {
        lobbyId = id;
        UpdateView();
    }
    public void JoinButtonFuction()
    {
        var lobby = DiscordLobbyService.INSTANCE.GetLobby(lobbyId);
        if (lobby.Id == 0)
        {
            Destroy(gameObject);
            return;
        }
        if (DiscordLobbyService.INSTANCE.CurrentLobbyId != lobby.Id && DiscordLobbyService.INSTANCE.CurrentLobbyId != 0)
        {
            DiscordLobbyService.INSTANCE.DisconnectLobby();
        }
        DiscordLobbyService.INSTANCE.ConnectToLobbyWithActivitySecret(lobby.Id + ":" + lobby.Secret);


    }
}
