using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JesperDiscord
{
    using Discord;
    /// <summary>
    /// Holds the connection with Discord Application
    /// </summary>
    public static class DiscordManager
    {
        private static Discord discord;

        private static User localUser;
        /// <summary>
        /// Should be runned the start of the Application
        /// </summary>
        /// <param name="Client_ID"></param>
        public static void  Initialize(long Client_ID)
        {
            System.Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "0");
            try
            {
                discord = new Discord(Client_ID, (System.UInt64)CreateFlags.NoRequireDiscord);
            }
            catch (System.Exception r)
            {
                Debug.Log(r);
            }
        }

        /// <summary>
        /// This should be run in a Monobehaviour update.
        /// </summary>
        public static void RunCallBack()
        {
            discord.RunCallbacks();
        }

        /// <summary>
        /// Get acess to Discords SDK Manager, use this if non of the provided scripts are up for the tasks. Look into Discord SDK Dokumantation for more info.
        /// </summary>
        /// <returns></returns>
        public static Discord GetDiscord()
        {
            if(discord == null)
            {
                throw new System.NullReferenceException("Need to Initialize DiscordManager, or Discord could not be found");
            }
            return discord;
        }

        public static User GetLocalUser()
        {
            return localUser;
        }

    }
}

