﻿using R2API;
using System;
using Morris.Components;
using Skillstates.Morris;
using Skillstates.Ghoul;

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
            desc += "< ! > Use Soul Drain for dealing with large enemies, and Sacrifice for clearing large crowds." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > " + Environment.NewLine + Environment.NewLine + Environment.NewLine;

            desc += modderNote;

            string lore =
                "";

            string outro = "..and so he left, to the sound of roaring applause.";
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
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Corpse Explosion");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", $"Ghouls will explode for <style=cIsDamage>{GhoulDeathState.baseDamageCoefficient * 100f}% damage</style> and " +
                $"<style=cIsUtility>activate your On-Kill effects</style> when they are slain.");
            #endregion

            #region Keywords
            LanguageAPI.Add("KEYWORD_SACRIFICE", $"<style=cKeywordName>Sacrifice</style><style=cSub>" +
                $"Kill the target ghoul to heal <style=cIsHealing>{LanternSkillState.sacrificePercentHealAmount * 100f}%</style> of your maximum health. " +
                $"This ghoul's <style=cIsDamage>Corpse Explosion</style> will be empowered to have a larger radius and " +
                $"deal <style=cIsDamage>{GhoulDeathState.sacrificedDamageCoefficient * 100f}% damage</style></style>.");

            LanguageAPI.Add("KEYWORD_SOULDRAIN", $"<style=cKeywordName>Soul Drain</style><style=cSub>Deal the greater of <style=cIsDamage>{150}% damage</style> or " +
                $"<style=cIsDamage>{1f}%</style> of the target's maximum health per second, " +
                $"and heal <style=cIsHealing>{1f}%</style> of your maximum health per second.</style>");

            LanguageAPI.Add("KEYWORD_LINKED", $"<style=cKeywordName>Linked</style><style=cSub>Ghouls do not inherit your items, but all damage they deal " +
                $"is treated as your own.</style>");
            //Additionally, ghouls inherit your <style=cIsDamage>damage</style> and <style=cIsDamage>attack speed</style> stats.
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SHOVEL_NAME", "Shovel Strike");
            LanguageAPI.Add(prefix + "PRIMARY_SHOVEL_DESCRIPTION", $"Swing your shovel for <style=cIsDamage>{SwingShovel.damageCoefficient * 100f}% damage</style>. " +
                $"Hit ghouls and tombstones to <style=cIsUtility>launch</style> them for <style=cIsDamage>{LaunchedState.damageCoefficient * 100f}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_GHOUL_NAME", "Raise Dead");
            LanguageAPI.Add(prefix + "SECONDARY_GHOUL_DESCRIPTION", $"<style=cIsUtility>Linked</style>. Spawn a ghoul on the ground in front of you. Ghouls bite for " +
                $"<style=cIsDamage>{GhoulMelee.damageCoefficient * 100f}% damage</style>, " +
                $"spit bile for <style=cIsDamage>{BileSpit.damageCoefficient * 100f}% damage</style>, and inflict <style=cIsDamage>Blight</style> with each attack.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_LANTERN_NAME", "Charon's Lantern");
            LanguageAPI.Add(prefix + "UTILITY_LANTERN_DESCRIPTION", $"If used on an enemy, cast <style=cIsDamage>Soul Drain</style>." + Environment.NewLine +
                "If used on a ghoul, cast <style=cIsUtility>Sacrifice</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_TOMBSTONE_NAME", "Tombstone");
            LanguageAPI.Add(prefix + "SPECIAL_TOMBSTONE_DESCRIPTION", $"Erect a Tombstone.");
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
            #endregion
        }
    }
}