using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameTimer : MonoBehaviour
{ 
    TextMeshProUGUI gameTimeInputField;

    float timer;

    // Start is called before the first frame update
    void Start()
    {
        gameTimeInputField = GetComponent<TextMeshProUGUI>();
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

        if (!float.TryParse(metaData, out timer))
        {
            timer = 10;
        }
        gameTimeInputField.text = timer.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.gameState == GameManager.GameState.GAME_IN_SESSION)
        {
            timer -= Time.deltaTime;
            
            if (timer <= 0)
            {
                GameManager.Instance.SetGameState(GameManager.GameState.GAME_END);
                timer = 0;
            }
            gameTimeInputField.text = timer.ToString();
        }
    }
}
