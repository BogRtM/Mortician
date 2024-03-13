using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Morris.Modules
{
    public static class Config
    {
        #region Morris General
        private static ConfigEntry<string> modVersion;

        public static ConfigEntry<float> sortPosition;

        public static ConfigEntry<int> ghoulLimit;

        public static string passiveTitle = "Passive";

        public static string primaryTitle = "Primary";

        public static string secondaryTitle = "Secondary";

        public static string utilityTitle = "Utility";

        public static string specialTitle = "Special";
        #endregion

        public static void ReadConfig(MorrisPlugin plugin)
        {
            #region Morris
            /*
            modVersion = plugin.Config.Bind<string>("General", "Mod Version", MorrisPlugin.MODVERSION, "Current version; don't touch this or it will reset your config");

            if (modVersion.Value != modVersion.DefaultValue.ToString())
            {
                Log.Warning("Mortician - version mismatch detected, clearing config");
                ((Dictionary<ConfigDefinition, string>)AccessTools.PropertyGetter(typeof(ConfigFile), "OrphanedEntries").Invoke(plugin.Config, null)).Clear();
                modVersion.Value = modVersion.DefaultValue.ToString();
            }
            */

            sortPosition = plugin.Config.Bind<float>("General", "Lobby Sort Position", 16f, "Sort position of Mortician in the character select lobby");

            ghoulLimit = plugin.Config.Bind<int>("General", "Ghoul limit", 0, "The maximum amount of ghouls that you can have out in the field. Set this to 0 or a " +
                "negative number to have no limit. The intended experience is to have no limit at all, so I highly suggest only touching this if you are experiencing lag.");

            #endregion
        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true)
        {
            return MorrisPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }
    }
}