using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Deputy.Modules
{
    public static class Config
    {
        private static ConfigEntry<string> modVersion;

        private static string deputyPrefix = "Deputy - ";
        private static string versionSuffix = " - " + DeputyPlugin.MODVERSION;

        #region Deputy General
        public static ConfigEntry<bool> bulletRicochet;
        #endregion

        public static void ReadConfig(DeputyPlugin plugin)
        {
            #region Deputy
            bulletRicochet = plugin.Config.Bind<bool>("General", "Enable Bullet Ricochet SFX", false, "Enable this for wacky cartoony bullet ricochet SFX");
            #endregion
        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true)
        {
            return DeputyPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }
    }
}