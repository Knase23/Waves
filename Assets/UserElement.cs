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
    bool toggleChanged = false;

    public void UpdateUserElement(Discord.User user)
    {
        userName.text = user.Username;
        string readyMetaData = "";
        if(user.Id != DiscordManager.CurrentUser.Id)
        {
            try
            {
                readyMetaData = DiscordLobbyService.INSTANCE.GetMetaDataOfMember(user.Id, "ready");
            }
            catch (Discord.ResultException resExp)
            {
                if (resExp.Result != Discord.Result.Ok)
                {
                    Debug.Log(resExp);
                }
            }

            bool ready = readyToggle.isOn;
            if (!string.IsNullOrEmpty(readyMetaData))
            {
                ready = bool.Parse(readyMetaData);
            }
            readyToggle.SetIsOnWithoutNotify(ready);
        }
        else
        {
            if (toggleChanged)
            {
                toggleChanged = false;
                DiscordLobbyService.INSTANCE.SetMyMetaData("ready", readyToggle.isOn.ToString());   
            }
        }
        //DiscordManager.INSTANCE.GetDiscord().GetImageManager(); 
        //Display Avatar

        //Check Meta data for is he ready
    }
    public void ChangeToggleState(bool isOn)
    {
        toggleChanged = true;
        readyToggle.isOn = isOn;
    }
}
