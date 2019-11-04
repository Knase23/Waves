using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LobbyHandler : MonoBehaviour
{
    public Button createLobbyButton;
    public Button showLobbyButton;
    public Button startGameButton;
    public Button readyButton;

    public TMP_InputField gameTimeInputField;

    public TextMeshProUGUI lobbySizeDisplay;
    public TMP_InputField lobbySizeInputField;

    public TMP_InputField titleOfLobbyInputField;

    // Start is called before the first frame update
    void Start()
    {
        //DiscordLobbyService.INSTANCE.OnJoinedLobby += OnJoinedLobby;
        //DiscordLobbyService.INSTANCE.OnLeftLobby += OnLeftLobby;
        DiscordLobbyService.INSTANCE.OnLobbyUpdate += OnLobbyUpdate;
    }

    private void OnLobbyUpdate(long lobbyId)
    {
        lobbySizeDisplay.text = DiscordLobbyService.INSTANCE.GetMemberCount().ToString() + "/";
        UpdateOfReadyStatus();
    }

    public void OnEnable()
    {
        OnJoinedLobby();
    }
    public void OnDisable()
    {
        OnLeftLobby();
    }

    private void OnJoinedLobby()
    {
        //Disable button
        Debug.Log("OnJoinedLobby: LobbyHandler", gameObject);
        createLobbyButton.gameObject.SetActive(false);
        showLobbyButton.gameObject.SetActive(true);

        bool IsTheHost = DiscordLobbyService.INSTANCE.IsTheHost();
        startGameButton.interactable = IsTheHost;
        gameTimeInputField.interactable = IsTheHost;
        lobbySizeInputField.interactable = IsTheHost;
        titleOfLobbyInputField.interactable = IsTheHost;

    }
    private void OnLeftLobby()
    {
        Debug.Log("OnLeftLobby: LobbyHandler", gameObject);
        //Disable button
        createLobbyButton.gameObject.SetActive(true);
        showLobbyButton.gameObject.SetActive(false); 
    }

    public void GameTimeInputFunction()
    {
        DiscordLobbyService.INSTANCE.SetLobbyMetaData("gameTime", gameTimeInputField.text);
    }

    public void ReadyButtonFunction()
    {
        // Set a meta data for the member in the lobby to check if he is ready
        UserElement userElement = FindObjectOfType<LobbyListOfMembers>().GetUserElement(DiscordManager.CurrentUser.Id);
        userElement.ChangeToggleState(!userElement.readyToggle.isOn);
        //StartCoroutine(buttonDisabledForSomeTime(readyButton));
    }

    public void UpdateOfReadyStatus()
    {
        if (DiscordLobbyService.INSTANCE.IsTheHost())
        {
            bool allready = true;
            foreach (var item in DiscordLobbyService.INSTANCE.GetLobbyMembers())
            {
                string metaData = "";
                try
                {
                    metaData = DiscordLobbyService.INSTANCE.GetMetaDataOfMember(item.Id, "ready");
                }
                catch (Discord.ResultException resExc)
                {
                    if (resExc.Result != Discord.Result.Ok)
                    {
                        Debug.Log(resExc);
                    }

                }
                bool result = string.IsNullOrEmpty(metaData) ? false : bool.Parse(metaData);

                if (!result)
                {
                    allready = false;
                    break;
                }

            }
            if (allready)
            {
                startGameButton.interactable = true;
            }
            else
            {
                startGameButton.interactable = false;
            }
        }



    }
    public void StartGameFunciton()
    {
        StartCoroutine(StartGameSequence());
    }
    IEnumerator StartGameSequence()
    {
        if (DiscordLobbyService.INSTANCE.IsTheHost())
        {
            GameTimeInputFunction();
        }
        yield return new WaitForSecondsRealtime(0.2f);

        foreach (var item in DiscordLobbyService.INSTANCE.GetLobbyMembers())
        {
            string metaData = "";
            try
            {
                metaData = DiscordLobbyService.INSTANCE.GetMetaDataOfMember(item.Id, "ready");
            }
            catch (Discord.ResultException resExc)
            {
                if (resExc.Result != Discord.Result.Ok)
                {
                    Debug.Log(resExc);
                }

            }
            bool result = string.IsNullOrEmpty(metaData) ? false : bool.Parse(metaData);

            if (!result)
            {
                yield break;
            }
        }

        Discord.Lobby lobby = DiscordLobbyService.INSTANCE.GetCurrentLobby();
        lobby.Locked = true;
        DiscordLobbyService.INSTANCE.SetLobbyData(lobby);
        GameManager.Instance.LoadGameScene();
        yield break;
    }

    public void UpdateTheCapacityOfLobby()
    {
        Discord.Lobby lobby = DiscordLobbyService.INSTANCE.GetCurrentLobby();
        int capacityNewNew = int.Parse(lobbySizeInputField.text);
        uint capacityNew = (uint)capacityNewNew;
        if (capacityNewNew < 1)
        {
            capacityNew = 1;
            lobbySizeInputField.text = capacityNew.ToString();
        }

        if (lobby.Capacity != capacityNew)
        {
            lobby.Capacity = capacityNew;
            DiscordLobbyService.INSTANCE.SetLobbyData(lobby);
        }
    }

    IEnumerator buttonDisabledForSomeTime(Button button)
    {
        button.interactable = false;
        yield return new WaitForSecondsRealtime(1f);
        button.interactable = true;
        yield break;
    }
    public void SetTitleOfLobby()
    {
        string title = titleOfLobbyInputField.text;
        DiscordLobbyService.INSTANCE.SetLobbyMetaData("title", title);
    }

}
