using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShipLabel : MonoBehaviour
{

    private long userId;
    public long UserId
    {
        get
        {
            return userId;
        }
        set
        {
            userId = value;
            UpdateText();
        }
    }


    public FollowTarget followTarget;
    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        followTarget = GetComponent<FollowTarget>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        UpdateText();
    }
    public void UpdateText()
    {
        if(userId != 0 && text)
        {
            if(userId == DiscordManager.CurrentUser.Id)
            {
                text.text = "";
            }
            else
            {
                text.text = DiscordLobbyService.INSTANCE.GetUser(userId).Username;
            }

        }
    }
}
