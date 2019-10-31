using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UpdateGameTimeVisualTime : MonoBehaviour
{
    public TMP_InputField gameTimeInputField;

    private void Start()
    {
        gameTimeInputField = GetComponentInChildren<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
      if(!DiscordLobbyService.INSTANCE.IsTheHost())
        {
            string metaData = gameTimeInputField.text;
            try
            {
                metaData = DiscordLobbyService.INSTANCE.GetLobbyMetaData(DiscordLobbyService.INSTANCE.CurrentLobbyId, "gameTime");
            }
            catch (Discord.ResultException resExc)
            {
                if (resExc.Result != Discord.Result.Ok)
                {
                    Debug.Log(resExc);
                }
            }
            gameTimeInputField.text = metaData;
        }
    }
}
