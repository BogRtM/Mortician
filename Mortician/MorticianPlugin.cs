using BepInEx;
using Morris.Modules.Survivors;
using R2API.Utils;
using R2API;
using RoR2;
using RoR2.Skills;
using RoR2.UI;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using EntityStates.Merc;
using Morris.Modules;
using EmotesAPI;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Morris
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]

    public class MorrisPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.Bog.Morris";
        public const string MODNAME = "Morris";

        public const string MODVERSION = "0.3.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "BOG";

        public static MorrisPlugin instance;
        public static PluginInfo PInfo;


        public static GameObject MorrisBodyPrefab;
        public static BodyIndex MorrisBodyIndex;

        public static DamageAPI.ModdedDamageType grantMorrisBuff;
        public static DamageAPI.ModdedDamageType resetUtilityOnKill;

        private void Awake()
        {
            instance = this;
            Log.Init(Logger);
            PInfo = Info;
            
            Modules.Config.ReadConfig(this);
            Modules.Assets.Initialize(); // load assets and read config
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            grantMorrisBuff = DamageAPI.ReserveDamageType();
            resetUtilityOnKill = DamageAPI.ReserveDamageType();

            // survivor initialization
            new Modules.Survivors.Morris().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            Subscriptions();
            Hook();
        }

        private void Start()
        {
            Modules.Assets.LoadSoundbank();
        }

        private void Subscriptions()
        {
        }

        private void Hook()
        {
            On.RoR2.BodyCatalog.SetBodyPrefabs += BodyCatalog_SetBodyPrefabs;
            

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI"))
            {
                On.RoR2.SurvivorCatalog.Init += SurvivorCatalog_Init;
            }

            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += CharacterBody_AddTimedBuff_BuffDef_float;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void SurvivorCatalog_Init(On.RoR2.SurvivorCatalog.orig_Init orig)
        {
            orig();

            foreach(var survivor in SurvivorCatalog.survivorDefs)
            {
                if(survivor.bodyPrefab.name == "MorrisBody")
                {
                    GameObject skeleton = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("MorrisHumanoidSkeleton");
                    CustomEmotesAPI.ImportArmature(survivor.bodyPrefab, skeleton);
                    skeleton.GetComponentInChildren<BoneMapper>().scale = 1.5f;
                }
            }
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            self.moveSpeed *= 1 + (self.GetBuffCount(Modules.Buffs.MorrisBuff) * Modules.Config.msPerStack.Value);
        }

        private void CharacterBody_AddTimedBuff_BuffDef_float(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
        {
            if(buffDef.buffIndex == Modules.Buffs.MorrisBuff.buffIndex) // && self.bodyIndex == MorrisBodyIndex)
            {
                if(self.GetBuffCount(buffDef) > 0)
                {
                    foreach(CharacterBody.TimedBuff timedBuff in self.timedBuffs)
                    {
                        if(timedBuff.buffIndex == buffDef.buffIndex)
                        {
                            timedBuff.timer = duration;
                        }
                    }
                }

                if(self.GetBuffCount(buffDef) >= Modules.Config.maxStacks.Value)
                {
                    return;
                }
            }

            orig(self, buffDef, duration);
        }

        private void BodyCatalog_SetBodyPrefabs(On.RoR2.BodyCatalog.orig_SetBodyPrefabs orig, GameObject[] newBodyPrefabs)
        {
            orig(newBodyPrefabs);
            MorrisBodyIndex = BodyCatalog.FindBodyIndex(MorrisBodyPrefab);
            Log.Warning("Morris's body index is: " + MorrisBodyIndex);
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);

            if (damageInfo.HasModdedDamageType(grantMorrisBuff) && damageInfo.attacker && !damageInfo.rejected)
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody && attackerBody.bodyIndex == MorrisBodyIndex)
                {
                    if (NetworkServer.active)
                    {
                        attackerBody.AddTimedBuff(Modules.Buffs.MorrisBuff, Modules.Config.msBuffDuration.Value);
                    }
                }
            }

            /*
            if (damageInfo.attacker)
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    if(attackerBody.bodyIndex == MorrisBodyIndex)
                    {
                        if (NetworkServer.active)
                        {
                            attackerBody.AddTimedBuff(Modules.Buffs.MorrisBuff, Modules.StaticValues.msBuffDuration);
                        }
                    }
                }
            }
            */
        }
    }
}