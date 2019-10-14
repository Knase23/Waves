using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyListOfMembers : MonoBehaviour
{
    public GameObject UserElementPrefab;

    Dictionary<long, UserElement> userIdToUserElement = new Dictionary<long, UserElement>();

    // Start is called before the first frame update
    void Start()
    {
        DiscordLobbyService.INSTANCE.OnLobbyUpdate += LobbyManager_OnLobbyUpdate;
    }

    private void LobbyManager_OnLobbyUpdate(long lobbyId)
    {
        Dictionary<long, UserElement> newUserIdToUserElement = new Dictionary<long, UserElement>();

        IEnumerable<Discord.User> users = DiscordLobbyService.INSTANCE.GetLobbyMembers();
        foreach (var user in users)
        {
            if(userIdToUserElement.ContainsKey(user.Id))
            {
                userIdToUserElement[user.Id].UpdateUserElement(user);
                newUserIdToUserElement.Add(user.Id, userIdToUserElement[user.Id]);
            }
            else
            {
                UserElement element = Instantiate(UserElementPrefab, transform).GetComponent<UserElement>();
                element.UpdateUserElement(user);
                newUserIdToUserElement.Add(user.Id, element);
            }
        }
        foreach (var item in userIdToUserElement)
        {
            if(!newUserIdToUserElement.ContainsKey(item.Key))
            {
                Destroy(item.Value);
            }
        }
        userIdToUserElement.Clear();
        userIdToUserElement = newUserIdToUserElement;
        

    }

    public UserElement GetUserElement(long userId )
    {
        return userIdToUserElement[userId];
    }

}
