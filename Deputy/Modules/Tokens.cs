﻿using R2API;
using System;
using Deputy.Components;
using Skillstates.Deputy;

namespace Deputy.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Deputy
            string devPrefix = DeputyPlugin.DEVELOPER_PREFIX;
            string prefix = devPrefix + "_DEPUTY_BODY_";

            string desc = "Test";
            
            string lore =
                "";

            string outro = "..and so she left, ";
            string outroFailure = "..and so she vanished, ";

            LanguageAPI.Add(prefix + "NAME", "Deputy");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Birds of Prey");
            LanguageAPI.Add(prefix + "LORE", lore);
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Headhunter");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Dashing Beauty");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "The Deputy can sprint in any direction.");
            #endregion

            #region Keywords
            LanguageAPI.Add("KEYWORD_EVASIVE", $"<style=cKeywordName>Evasive</style><style=cSub>If this skill is used while sprinting, perform a short " +
                $"<style=cIsUtility>dash</style> in the target direction.</style>");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SHOOT_NAME", "Vigor & Valor");
            LanguageAPI.Add(prefix + "PRIMARY_SHOOT_DESCRIPTION", $"<style=cIsUtility>Agile</style>. Fire short-range revolvers for " +
                $"<style=cIsDamage>{150f}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_SLING_NAME", "Gun Sling");
            LanguageAPI.Add(prefix + "SECONDARY_SLING_DESCRIPTION", $"<style=cIsUtility>Evasive</style>. Throw out two revolvers. " +
                $"While in flight, each revolver will shoot a nearby enemy for <style=cIsDamage>6x80%</style> damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_KICK_NAME", "Shooting Star");
            LanguageAPI.Add(prefix + "UTILITY_KICK_DESCRIPTION", $"<style=cIsUtility>Dash</style> forward. On contact with an enemy, perform a <style=cIsUtility>heavy</style> " +
                $" kick for <style=cIsDamage>800% damage</style>. Pressing the jump input while dashing will instead cause you to leap into the air, " +
                $"rapidly shooting enemies below for up to <style=cIsDamage>6x100% damage</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_BULLETHEAVEN_NAME", "Bullet Heaven");
            LanguageAPI.Add(prefix + "SPECIAL_BULLETHEAVEN_DESCRIPTION", $"Nothing yet.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Deputy: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Deputy, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Deputy: Mastery");
            #endregion
            #endregion
            /*
            #region Henry
            string prefix = DeputyPlugin.DEVELOPER_PREFIX + "_HENRY_BODY_";

            string desc = "Henry is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, searching for a new identity.";
            string outroFailure = "..and so he vanished, forever a blank slate.";

            LanguageAPI.Add(prefix + "NAME", "Henry");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Chosen One");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Henry passive");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Sword");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Helpers.agilePrefix + $"Swing forward for <style=cIsDamage>{100f * StaticValues.swordDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_GUN_NAME", "Handgun");
            LanguageAPI.Add(prefix + "SECONDARY_GUN_DESCRIPTION", Helpers.agilePrefix + $"Fire a handgun for <style=cIsDamage>{100f * StaticValues.gunDamageCoefficient}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_ROLL_NAME", "Roll");
            LanguageAPI.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Roll a short distance, gaining <style=cIsUtility>300 armor</style>. <style=cIsUtility>You cannot be hit during the roll.</style>");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_NAME", "Bomb");
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * StaticValues.bombDamageCoefficient}% damage</style>.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Henry: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Henry, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Henry: Mastery");
            #endregion
            #endregion
            */
        }
    }
}