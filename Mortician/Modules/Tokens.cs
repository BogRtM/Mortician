using R2API;
using System;
using Morris.Components;
using Skillstates.Morris;

namespace Morris.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Morris
            string devPrefix = MorrisPlugin.DEVELOPER_PREFIX;
            string prefix = devPrefix + "_MORRIS_BODY_";

            string modderNote = "<style=cShrine>Modder's Note:</style> <style=cUserSetting>Thank you for showing interest in <color=#e1c100>The Morris</color>! " +
                "For feedback and bug reports, please contact <style=cIsUtility>Bog#4770</style> on Discord.</style>";

            string desc = "The Mortician is a lumbering melee tank who faces the horde with an army of undead and an array of ghastly powers.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Trigger Tap has perfect accuracy and no damage falloff, but its maximum range is quite limited compared to most other gun-based attacks." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > The revolvers thrown out by Gun Sling will always prioritize the enemy closest to them." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Shooting Star can be slightly angled upwards by looking up." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Crashing Comet can deal devastating burst damage at high movement speed values. Use your guns to build up stacks of Hot Pursuit, " +
                "then go in for the kill." + Environment.NewLine + Environment.NewLine + Environment.NewLine;

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
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", $"Ghouls explode for <style=cIsDamage>{250f}% damage</style> and " +
                $"<style=cIsUtility>activate your On-Kill effects</style> when they are slain.");
            #endregion

            #region Keywords
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SHOVEL_NAME", "Shovel Strike");
            LanguageAPI.Add(prefix + "PRIMARY_SHOVEL_DESCRIPTION", $"Swing your shovel for <style=cIsDamage>{SwingShovel.damageCoefficient * 100f} damage</style>. " +
                $"Hit ghouls and tombstones to <style=cIsUtility>launch</style> them for <style=cIsDamage>{400f}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_GHOUL_NAME", "Raise Dead");
            LanguageAPI.Add(prefix + "SECONDARY_GHOUL_DESCRIPTION", $"Spawn a ghoul on the ground in front of you. Ghouls bite for <style=cIsDamage>{200f}% damage</style>, " +
                $"spit bile for <style=cIsDamage>{150f}% damage</style>, and inflict <style=cEvent>Rot</style> with every attack.");
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