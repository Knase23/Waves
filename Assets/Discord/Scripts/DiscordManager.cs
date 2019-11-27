using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
public class DiscordManager : MonoBehaviour
{
    public static DiscordManager INSTANCE;
    Discord.Discord discord;


    public const long CLIENT_ID = 574643243728240642;

    [SerializeField]
    private static Discord.User currentUser;
    public static Discord.User CurrentUser
    {
        get { return currentUser; }
        set { currentUser = value; }
    }

    private void Awake()
    {
        if (INSTANCE)
        {
            Destroy(this);
        }
        else
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
        //var discord0 = new Discord.Discord(ApplicationId, (System.UInt64)Discord.CreateFlags.Default);

        //// This makes the SDK connect to PTB
#if UNITY_EDITOR
        System.Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "1");
        //var discord1 = new Discord.Discord(ApplicationId, (System.UInt64)Discord.CreateFlags.Default);
#endif
        /*
            Grab that Client ID from earlier
            Discord.CreateFlags.Default will require Discord to be running for the game to work
            If Discord is not running, it will:
            1. Close your game
            2. Open Discord
            3. Attempt to re-open your game
            Step 3 will fail when running directly from the Unity editor
            Therefore, always keep Discord running during tests, or use Discord.CreateFlags.NoRequireDiscord
        */
        try
        {
            discord = new Discord.Discord(CLIENT_ID, (System.UInt64)Discord.CreateFlags.Default);
        }
        catch (System.Exception r)
        {
            Debug.Log(r);
        }
       
        
        //discord = new Discord.Discord(ApplicationId, (System.UInt64)Discord.CreateFlags.Default);
        Invoke("UpdateCurrentUser", 1);
    }

    // Update is called once per frame
    void Update()
    {
        discord.RunCallbacks();
    }

    private void OnDisable()
    {
        if(discord != null)
            discord.RunCallbacks();
    }
    public void UpdateCurrentUser()
    {
        CurrentUser = discord.GetUserManager().GetCurrentUser();
    }

    public Discord.Discord GetDiscord()
    {
        if (discord == null)
        {
            Debug.Log("Failed to find Discord");
            return null;
        }

        return discord;
    }
}
