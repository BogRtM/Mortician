using R2API;
using System;
using Morris.Components;
using SkillStates.Morris;
using SkillStates.Ghoul;
using SkillStates.SharedStates;
using SkillStates.Tombstone;

namespace Morris.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Morris
            string devPrefix = MorrisPlugin.DEVELOPER_PREFIX;
            string prefix = devPrefix + "_MORRIS_BODY_";

            string modderNote = "<style=cShrine>Modder's Note:</style> <style=cUserSetting></style>";

            string desc = "The Mortician is a lumbering melee tank who faces the horde with an army of undead and an array of ghastly powers.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Ghouls launched by your shovel will latch onto larger enemies and bite them repeatedly." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Ghouls make no effort to follow you, and instead focus solely on their targets." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > You can quickly clear groups of flying enemies by launching your ghouls up high then sacrificing them mid-air." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > " + Environment.NewLine + Environment.NewLine + Environment.NewLine;

            desc += modderNote;

            string lore =
                "";

            string outro = "..and so he left, his somber duty yet unfilled.";
            string outroFailure = "..and so he vanished, finally put to rest.";

            LanguageAPI.Add(prefix + "NAME", "Mortician");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Dead Man Walking");
            LanguageAPI.Add(prefix + "LORE", lore);
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Soul Offering");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", $"Ghouls <style=cIsUtility>activate your On-Kill effects</style> when they are slain.");
            #endregion

            #region Keywords
            LanguageAPI.Add("KEYWORD_LINKED", $"<style=cKeywordName>Linked</style><style=cSub>Ghouls do not inherit your items, but all damage they deal " +
                $"is treated as your own.</style>");
            LanguageAPI.Add("KEYWORD_EXHAUST", $"<style=cKeywordName>Exhaustion</style><style=cSub>Reduces movement speed and damage by " +
                $"<style=cIsUtility>{(1 - Buffs.exhaustStatReduction) * 100f}%</style>. Increases cooldowns by " +
                $"<style=cIsUtility>{(Buffs.exhaustCooldownScale - 1) * 100f}%</style>.</style>");

            //Additionally, ghouls inherit your <style=cIsDamage>damage</style> and <style=cIsDamage>attack speed</style> stats.
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SHOVEL_NAME", "Shovel Strike");
            LanguageAPI.Add(prefix + "PRIMARY_SHOVEL_DESCRIPTION", $"Swing your shovel for <style=cIsDamage>{SwingShovel.damageCoefficient * 100f}% damage</style>. " +
                $"Hit ghouls and tombstones to <style=cIsUtility>launch</style> them for <style=cIsDamage>{BaseLaunchedState.damageCoefficient * 100f}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_GHOUL_NAME", "Raise Dead");
            LanguageAPI.Add(prefix + "SECONDARY_GHOUL_DESCRIPTION", $"<style=cIsUtility>Linked</style>. Spawn a ghoul on the ground in front of you. Ghouls bite for " +
                $"<style=cIsDamage>{GhoulMelee.damageCoefficient * 100f}% damage</style>, and " +
                $"spit <style=cIsDamage>Blighted</style> bile for <style=cIsDamage>{BileSpit.damageCoefficient * 100f}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_LANTERN_NAME", "Sacrifice");
            LanguageAPI.Add(prefix + "UTILITY_LANTERN_DESCRIPTION", 
                $"<style=cIsHealth>Detonate</style> the target ghoul for <style=cIsDamage>{DeathState.sacrificedDamageCoefficient * 100f}% damage</style>, " +
                $"and <style=cIsHealing>heal {Sacrifice.sacrificePercentHealAmount * 100f}% of your maximum health</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_TOMBSTONE_NAME", "Tombstone");
            LanguageAPI.Add(prefix + "SPECIAL_TOMBSTONE_DESCRIPTION", $"Erect a Tombstone that spawns a ghoul every " +
                $"<style=cIsUtility>{SpawnGhoulOnTimer.spawnTime} seconds</style>.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Morris: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Morris, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Morris: Mastery");
            #endregion
            #endregion

            #region Ghoul
            string ghoulPrefix = devPrefix + "_GHOUL_BODY_";
            LanguageAPI.Add(ghoulPrefix + "NAME", "Ghoul");

            string tombstonePrefix = devPrefix + "_TOMBSTONE_BODY_";
            LanguageAPI.Add(tombstonePrefix + "NAME", "Tombstone");
            #endregion
        }
    }
}