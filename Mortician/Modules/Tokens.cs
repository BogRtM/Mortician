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

            string desc = "The Mortician is a slow, yet durable melee necromancer who excels at controlling territory with his army of undead.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Ghouls launched by your shovel will latch onto larger enemies and bite them repeatedly." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Ghouls make no effort to follow you, and instead focus solely on their targets." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > You can quickly clear groups of flying enemies by launching your ghouls up high, then sacrificing them mid-air." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Your tombstone can be used as an effective tool to establish control of an area from a distance." + Environment.NewLine + Environment.NewLine + Environment.NewLine;

            string lore =
                "God has abandoned us.\n\n" +
                "" +
                "In truth, I don't think he ever cared in the first place.\n\n" +
                "" +
                "Somewhere beyond the furthest depths of human comprehension lies a realm to which we are all condemned to one day find ourselves in. It is not a " +
                "realm of pearly gates and angels, nor of fire and brimstone, but one of darkness and despair. It is an abyssal prison in which all its inmates " +
                "are reduced to mere data, governed by curious, sinister beings who shall poke and prod at you and conduct their twisted experiments on you for all eternity.\n\n" +
                "" +
                "Every soul that I pull out of that wretched place tells me a story. Some are forced to relive the worst pain they have ever " +
                "experienced, time and time again. Some are suspended in an eternal, lonely darkness, with no companionship or stimulation. Some tell me of " +
                "crabs and prawns. Each and every one of them begs me not to make them go back to that place. I've managed to create new vessels for these souls to inhabit, " +
                "and should they die, they return to me once again.\n\n" +
                "" +
                "And the creatures that rule this prison, they come to me in my dreams. To taunt me. To tell me of what they will do to me once I am dead and in their clutches. " +
                "I know that what they have planned will be far worse than anything the devil could ever do to me. Each soul that I save is one more toy I take away " +
                "from them, and they will make me pay for ruining their fun.\n\n" +
                "" +
                "I have grown old, and my time shall soon come as well. So I have decided; I will find a passage to this realm, " +
                "slay its wicked overlords, and free all of its prisoners.\n\n" +
                "" +
                "I will not do it alone; the souls that I have freed shall fight alongside me. " +
                "They will guide me, and I will guide them. Together, we will restore the sanctity in death, and restore the peace that we were promised.";

            string outro = "..and so he left, his somber duty yet unfulfilled.";
            string outroFailure = "..and so he vanished, finally granted the peace he desired.";

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
            LanguageAPI.Add("KEYWORD_LINKED", $"<style=cKeywordName>Soulbound</style><style=cSub>Ghouls do not inherit your items, but all damage they deal " +
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
            LanguageAPI.Add(prefix + "SECONDARY_GHOUL_DESCRIPTION", $"<style=cIsUtility>Soulbound</style>. Spawn a ghoul on the ground in front of you. Ghouls bite for " +
                $"<style=cIsDamage>{GhoulMelee.damageCoefficient * 100f}% damage</style>, and " +
                $"spit <style=cIsDamage>Blighted</style> bile for <style=cIsDamage>{BileSpit.damageCoefficient * 100f}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_LANTERN_NAME", "Sacrifice");
            LanguageAPI.Add(prefix + "UTILITY_LANTERN_DESCRIPTION", 
                $"<style=cIsHealth>Detonate</style> the target ghoul for <style=cIsDamage>{GhoulDeath.sacrificedDamageCoefficient * 100f}% damage</style>, " +
                $"and <style=cIsHealing>heal {Sacrifice.sacrificePercentHealAmount * 100f}% of your maximum health</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_TOMBSTONE_NAME", "Tombstone");
            LanguageAPI.Add(prefix + "SPECIAL_TOMBSTONE_DESCRIPTION", $"Erect a tombstone that spawns a ghoul every <style=cIsUtility>{TombstoneController.spawnTime} seconds</style>. " +
                $"Whenever a ghoul is slain, the tombstone generates an <style=cIsDamage>explosive</style> <style=cIsUtility>vengeful soul</style> which it will fire " +
                $"at a nearby enemy for <style=cIsDamage>{TombstoneController.soulOrbDamage * 100f}% damage</style>.");
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