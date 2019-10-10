using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UserElement : MonoBehaviour
{
    public TextMeshProUGUI userName;
    public RawImage imageAvatarDisplay;
    public Toggle readyToggle;

    public void UpdateUserElement(Discord.User user)
    {
        userName.text = user.Username;
        //DiscordManager.INSTANCE.GetDiscord().GetImageManager(); 
        //Display Avatar

        //Check Meta data for is he ready
    }
}
