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
using Morris.Components;
using SkillStates.Morris;
using ShaderSwapper;
using System.Linq;
using System.Collections;

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

        public const string MODVERSION = "0.1.1";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "BOG";

        public static MorrisPlugin instance;
        public static PluginInfo PInfo;

        public static GameObject MorrisBodyPrefab;
        public static BodyIndex MorrisBodyIndex;

        public static GameObject GhoulBodyPrefab;
        public static BodyIndex GhoulBodyIndex;

        internal static GameObject TombstoneBodyPrefab;
        internal static BodyIndex TombstoneBodyIndex;

        public static DamageAPI.ModdedDamageType LaunchGhoul;

        public static bool containsCustomEmoteAPI;

        private void Awake()
        {
            instance = this;
            Log.Init(Logger);
            PInfo = Info;
            
            Modules.Config.ReadConfig(this);

            containsCustomEmoteAPI = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI");
            //Log.Warning("Contains custom emote API: " + containsCustomEmoteAPI);

            Modules.Assets.Initialize(); // load assets and read config

            StartCoroutine(Modules.Assets.mainAssetBundle.UpgradeStubbedShadersAsync());

            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            LaunchGhoul = DamageAPI.ReserveDamageType();

            // survivor initialization
            new Modules.Survivors.Morris().Initialize();
            //Log.Warning("Mortician created successfully");
            new Modules.NPC.GhoulMinion().Initialize();
            //Log.Warning("Ghoul created successfully");
            new Modules.NPC.TombstoneDeployable().Initialize();
            //Log.Warning("Tombstone created successfully");

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            Subscriptions();
            Hook();

            if (containsCustomEmoteAPI)
            {
                CustomEmoteAPICompat();
            }
        }

        public IEnumerator CoroutineMaterialStuff()
        {
            //Log.Warning("Converting stubbed shaders");
            yield return Modules.Assets.mainAssetBundle.UpgradeStubbedShadersAsync();
            //Log.Warning("Shader conversion finished; now fixing render queues");
            yield return Materials.FixRenderQueues();
            //Log.Warning("Render queues have been fixed");
        }

        private void Start()
        {
            Modules.Assets.LoadSoundbank();
        }

        private void Subscriptions()
        {
            
        }

        private void ShowHealthBarToOwner(DamageDealtMessage obj)
        {
            if (!obj.attacker) return;
            if (!obj.victim || obj.isSilent) return;

            MorrisMinionController minionController = obj.attacker.GetComponent<MorrisMinionController>();
            if(!minionController || !minionController.owner) return;

            HealthComponent victimHealthComponent = obj.victim.GetComponent<HealthComponent>();
            if (!victimHealthComponent || victimHealthComponent.dontShowHealthbar) return;

            TeamIndex victimTeamIndex = victimHealthComponent.body.teamComponent.teamIndex;

            foreach (CombatHealthBarViewer combatHealthBarViewer in CombatHealthBarViewer.instancesList)
            {
                if (minionController.owner == combatHealthBarViewer.viewerBodyObject && combatHealthBarViewer.viewerBodyObject)
                {
                    combatHealthBarViewer.HandleDamage(victimHealthComponent, victimTeamIndex);
                }
            }
        }

        private void Hook()
        {
            On.RoR2.BodyCatalog.SetBodyPrefabs += BodyCatalog_SetBodyPrefabs;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self.HasBuff(Buffs.exhaustionDebuff))
            {
                self.moveSpeed *= Buffs.exhaustStatReduction;
                self.damage *= Buffs.exhaustStatReduction;

                if (self.skillLocator)
                {
                    if (self.skillLocator.primary)
                        self.skillLocator.primary.cooldownScale *= Buffs.exhaustCooldownScale;

                    if (self.skillLocator.secondary)
                        self.skillLocator.secondary.cooldownScale *= Buffs.exhaustCooldownScale;

                    if (self.skillLocator.utility)
                        self.skillLocator.utility.cooldownScale *= Buffs.exhaustCooldownScale;

                    if (self.skillLocator.special)
                        self.skillLocator.special.cooldownScale *= Buffs.exhaustCooldownScale;
                }
            }
        }

        private void BodyCatalog_SetBodyPrefabs(On.RoR2.BodyCatalog.orig_SetBodyPrefabs orig, GameObject[] newBodyPrefabs)
        {
            orig(newBodyPrefabs);

            MorrisBodyIndex = BodyCatalog.FindBodyIndex(MorrisBodyPrefab);
            //Log.Warning("Mortician's body index is: " + MorrisBodyIndex);

            GhoulBodyIndex = BodyCatalog.FindBodyIndex(GhoulBodyPrefab);
            //Log.Warning("Mortician's ghoul body index is: " + GhoulBodyIndex);

            TombstoneBodyIndex = BodyCatalog.FindBodyIndex(TombstoneBodyPrefab);
            //Log.Warning("Mortician's tombstone body index is: " + TombstoneBodyIndex);
        }

        private void CustomEmoteAPICompat()
        {
            On.RoR2.BodyCatalog.SetBodyPrefabs += CreateEmoteSkeletons;
            CustomEmotesAPI.boneMapperCreated += CustomEmotesAPI_boneMapperCreated;
            CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged;
        }

        private void CreateEmoteSkeletons(On.RoR2.BodyCatalog.orig_SetBodyPrefabs orig, GameObject[] newBodyPrefabs)
        {
            orig(newBodyPrefabs);

            Log.Warning("Creating Morris emote skeleton");
            GameObject morrisSkeleton = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("MorrisHumanoidSkeleton");
            CustomEmotesAPI.ImportArmature(MorrisBodyPrefab, morrisSkeleton);
            morrisSkeleton.GetComponentInChildren<BoneMapper>().scale = 1f;

            Log.Warning("Creating Ghoul emote skeleton");
            GameObject ghoulSkeleton = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("GhoulHumanoidSkeleton");
            CustomEmotesAPI.ImportArmature(GhoulBodyPrefab, ghoulSkeleton);
            ghoulSkeleton.GetComponentInChildren<BoneMapper>().scale = 1f;
        }

        private void CustomEmotesAPI_boneMapperCreated(BoneMapper mapper)
        {
            if (mapper.mapperBody.bodyIndex == GhoulBodyIndex)
            {
                MorrisMinionController minionController = mapper.mapperBody.GetComponent<MorrisMinionController>();

                if (minionController.owner)
                {
                    BoneMapper ownerMapper = minionController.owner.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>();

                    if (ownerMapper && ownerMapper.currentClipName != "none")
                    {
                        CustomEmotesAPI.PlayAnimation(ownerMapper.currentClipName, mapper);
                    }
                }
            }
        }

        private void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
        {
            if (mapper.mapperBody.bodyIndex == MorrisBodyIndex)
            {
                var minions = CharacterMaster.readOnlyInstancesList.Where(el => el.minionOwnership.ownerMaster == mapper.mapperBody.master);

                foreach (var minion in minions)
                {
                    if (minion.GetBody().bodyIndex == GhoulBodyIndex && minion.GetBody().healthComponent.alive)
                    {
                        ModelLocator modelLocator = minion.GetBodyObject().GetComponent<ModelLocator>();
                        if (modelLocator)
                        {
                            BoneMapper ghoulMapper = modelLocator.modelTransform.GetComponentInChildren<BoneMapper>();
                            CustomEmotesAPI.PlayAnimation(newAnimation, ghoulMapper);
                        }
                    }
                }
            }
        }
    }
}