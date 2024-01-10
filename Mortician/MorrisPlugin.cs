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
        public const string MODNAME = "Mortician";

        public const string MODVERSION = "0.1.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "BOG";

        public static MorrisPlugin instance;
        public static PluginInfo PInfo;

        public static GameObject MorrisBodyPrefab;
        public static BodyIndex MorrisBodyIndex;

        public static GameObject GhoulBodyPrefab;
        public static BodyIndex GhoulBodyIndex;

        public static DamageAPI.ModdedDamageType LaunchGhoul;

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

            LaunchGhoul = DamageAPI.ReserveDamageType();

            // survivor initialization
            new Modules.Survivors.Morris().Initialize();
            Log.Warning("Mortician created successfully");
            new Modules.NPC.Ghoul().Initialize();
            Log.Warning("Ghoul created successfully");

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            Subscriptions();
            Hook();
        }

        private void Start()
        {
            //Modules.Assets.LoadSoundbank();
        }

        private void Subscriptions()
        {
        }

        private void Hook()
        {
            On.RoR2.BodyCatalog.SetBodyPrefabs += BodyCatalog_SetBodyPrefabs;
            

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI"))
            {
                //On.RoR2.SurvivorCatalog.Init += SurvivorCatalog_Init;
            }

            //On.RoR2.GlobalEventManager.OnHitAll += GlobalEventManager_OnHitAll;
        }

        private void GlobalEventManager_OnHitAll(On.RoR2.GlobalEventManager.orig_OnHitAll orig, GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            orig(self, damageInfo, hitObject);

            if(damageInfo.HasModdedDamageType(LaunchGhoul))
            {
                Chat.AddMessage("You hit: " + hitObject.name);
            }
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

        private void BodyCatalog_SetBodyPrefabs(On.RoR2.BodyCatalog.orig_SetBodyPrefabs orig, GameObject[] newBodyPrefabs)
        {
            orig(newBodyPrefabs);
            MorrisBodyIndex = BodyCatalog.FindBodyIndex(MorrisBodyPrefab);
            Log.Warning("Mortician's body index is: " + MorrisBodyIndex);
            GhoulBodyIndex = BodyCatalog.FindBodyIndex(GhoulBodyPrefab);
            Log.Warning("Mortician's ghoul body index is: " + GhoulBodyIndex);
        }

    }
}