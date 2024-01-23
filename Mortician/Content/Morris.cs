using BepInEx.Configuration;
using Morris.Modules.Characters;
using Morris.Components;
using SkillStates.Morris;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using UnityEngine.UI;
using EntityStates;
using static RoR2.TeleporterInteraction;
using SkillStates.Morris;

namespace Morris.Modules.Survivors
{
    internal class Morris : SurvivorBase
    {
        public override string bodyName => "Morris";

        public const string Morris_PREFIX = MorrisPlugin.DEVELOPER_PREFIX + "_MORRIS_BODY_";
        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => Morris_PREFIX;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "MorrisBody",
            bodyNameToken = MorrisPlugin.DEVELOPER_PREFIX + "_MORRIS_BODY_NAME",
            subtitleNameToken = MorrisPlugin.DEVELOPER_PREFIX + "_MORRIS_BODY_SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texMorrisIcon"),
            //bodyColor = new Color(62f / 255f, 162f / 255f, 82f / 255f),
            bodyColor = new Color32(33, 255, 189, 255),

            crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/SimpleDotCrosshair.prefab").WaitForCompletion(),
            podPrefab = null,

            
            capsuleHeight = 3.2f,
            capsuleRadius = 0.9f,
            modelBasePosition = new Vector3(0f, -1.6f, 0f),

            maxHealth = 160f,
            healthGrowth = 48f,
            healthRegen = 2.5f,
            regenGrowth = 0.5f,
            damage = 14f,
            damageGrowth = 2.8f,
            armor = 20f,
            sortPosition = 1f,

            cameraPivotPosition = new Vector3(0f, 0.3f, 0f),
            cameraParamsDepth = -12f
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "MorrisMesh",
                    dontHotpoo = true,
                },
                new CustomRendererInfo
                {
                    childName = "NecklaceMesh",
                    dontHotpoo = true,
                },
                new CustomRendererInfo
                {
                    childName = "ShawlMesh",
                    dontHotpoo = true,
                },
                new CustomRendererInfo
                {
                    childName = "ShovelMesh",
                    dontHotpoo = true,
                },
                new CustomRendererInfo
                {
                    childName = "LanternMesh",
                    dontHotpoo = true,
                }
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override ItemDisplaysBase itemDisplays => new MorrisItemDisplays();

        //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public static DeployableSlot tombstoneSlot;
        public DeployableAPI.GetDeployableSameSlotLimit GetTombstoneSlotLimit;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            MorrisPlugin.MorrisBodyPrefab = this.bodyPrefab;

            Rigidbody rb = bodyPrefab.GetComponent<Rigidbody>();
            rb.mass = 300f;
            CharacterMotor cm = bodyPrefab.GetComponent<CharacterMotor>();
            cm.mass = 300f;

            bodyPrefab.AddComponent<LanternTracker>();

            EntityStateMachine lanternESM = Array.Find(bodyPrefab.GetComponents<EntityStateMachine>(), (EntityStateMachine ESM) => ESM.customName == "Slide");
            lanternESM.customName = "Lantern";

            RegisterDeployables();
        }

        private void RegisterDeployables()
        {
            GetTombstoneSlotLimit += GetMaxTombstones;
            tombstoneSlot = DeployableAPI.RegisterDeployableSlot(GetTombstoneSlotLimit);

            On.RoR2.CharacterMaster.AddDeployable += CharacterMaster_AddDeployable;
        }

        private void CharacterMaster_AddDeployable(On.RoR2.CharacterMaster.orig_AddDeployable orig, CharacterMaster self, Deployable deployable, DeployableSlot slot)
        {
            if (MasterCatalog.FindMasterIndex(deployable.gameObject) == MasterCatalog.FindMasterIndex(PlaceTombstone.tombstoneMasterPrefab))
            {
                slot = tombstoneSlot;
            }
             
            orig(self, deployable, slot);
        }

        public int GetMaxTombstones(CharacterMaster self, int deployableCountMulitplier)
        {
            return 1;
            //cope
        }

        private void SetCoreTransform()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;
            CharacterBody characterBody = bodyPrefab.GetComponent<CharacterBody>();

            Transform baseTransform = childLocator.FindChild("BaseBone");
            model.GetComponent<CharacterModel>().coreTransform = baseTransform;
            characterBody.coreTransform = baseTransform;
        }

        public override void InitializeUnlockables()
        {
            //uncomment this when you have a mastery skin. when you do, make sure you have an icon too
            //masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.MasteryAchievement>();
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;
            
            Transform swingHitbox = childLocator.FindChild("SwingHitbox");
            Modules.Prefabs.SetupHitbox(model, swingHitbox, "Swing");
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = MorrisPlugin.DEVELOPER_PREFIX;

            
            #region Primary
            SteppedSkillDef shovelSkillDef = ScriptableObject.CreateInstance<SteppedSkillDef>();
            shovelSkillDef.skillName = prefix + "_MORRIS_BODY_PRIMARY_SHOVEL_NAME";
            shovelSkillDef.skillNameToken = prefix + "_MORRIS_BODY_PRIMARY_SHOVEL_NAME";
            shovelSkillDef.skillDescriptionToken = prefix + "_MORRIS_BODY_PRIMARY_SHOVEL_DESCRIPTION";
            shovelSkillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("QuickTriggerIcon");
            shovelSkillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(SwingShovel));
            shovelSkillDef.activationStateMachineName = "Weapon";
            shovelSkillDef.baseMaxStock = 1;
            shovelSkillDef.baseRechargeInterval = 0f;
            shovelSkillDef.beginSkillCooldownOnSkillEnd = false;
            shovelSkillDef.canceledFromSprinting = false;
            shovelSkillDef.forceSprintDuringState = false;
            shovelSkillDef.fullRestockOnAssign = true;
            shovelSkillDef.interruptPriority = EntityStates.InterruptPriority.Any;
            shovelSkillDef.resetCooldownTimerOnUse = false;
            shovelSkillDef.isCombatSkill = true;
            shovelSkillDef.mustKeyPress = false;
            shovelSkillDef.cancelSprintingOnActivation = true;
            shovelSkillDef.rechargeStock = 1;
            shovelSkillDef.requiredStock = 1;
            shovelSkillDef.stockToConsume = 1;
            Modules.Content.AddSkillDef(shovelSkillDef);
            Modules.Skills.AddPrimarySkills(bodyPrefab, shovelSkillDef);
            #endregion

            #region Secondary
            SkillDef ghoulSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_MORRIS_BODY_SECONDARY_GHOUL_NAME",
                skillNameToken = prefix + "_MORRIS_BODY_SECONDARY_GHOUL_NAME",
                skillDescriptionToken = prefix + "_MORRIS_BODY_SECONDARY_GHOUL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("GunSlingIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SpawnGhoul)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 2,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] {"KEYWORD_LINKED"}
            });
            Modules.Skills.AddSecondarySkills(bodyPrefab, ghoulSkillDef);
            #endregion

            #region Utility
            LanternSkillDef lanternSkillDef = ScriptableObject.CreateInstance<LanternSkillDef>();
            lanternSkillDef.skillName = prefix + "_MORRIS_BODY_UTILITY_LANTERN_NAME";
            lanternSkillDef.skillNameToken = prefix + "_MORRIS_BODY_UTILITY_LANTERN_NAME";
            lanternSkillDef.skillDescriptionToken = prefix + "_MORRIS_BODY_UTILITY_LANTERN_DESCRIPTION";
            lanternSkillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("QuickTriggerIcon");
            lanternSkillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(Sacrifice));
            lanternSkillDef.activationStateMachineName = "Weapon";
            lanternSkillDef.baseMaxStock = 1;
            lanternSkillDef.baseRechargeInterval = 6f;
            lanternSkillDef.beginSkillCooldownOnSkillEnd = false;
            lanternSkillDef.canceledFromSprinting = false;
            lanternSkillDef.forceSprintDuringState = false;
            lanternSkillDef.fullRestockOnAssign = false;
            lanternSkillDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            lanternSkillDef.resetCooldownTimerOnUse = false;
            lanternSkillDef.isCombatSkill = true;
            lanternSkillDef.mustKeyPress = true;
            lanternSkillDef.cancelSprintingOnActivation = true;
            lanternSkillDef.rechargeStock = 1;
            lanternSkillDef.requiredStock = 1;
            lanternSkillDef.stockToConsume = 1;
            Modules.Content.AddSkillDef(lanternSkillDef);
            Modules.Skills.AddUtilitySkills(bodyPrefab, lanternSkillDef);
            #endregion

            #region Special
            SkillDef tombstoneSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_MORRIS_BODY_SPECIAL_TOMBSTONE_NAME",
                skillNameToken = prefix + "_MORRIS_BODY_SPECIAL_TOMBSTONE_NAME",
                skillDescriptionToken = prefix + "_MORRIS_BODY_SPECIAL_TOMBSTONE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("SkullBreakerIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(PlaceTombstone)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 30f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0
            });
            Modules.Skills.AddSpecialSkills(bodyPrefab, tombstoneSkillDef);
            #endregion
        }

        public override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(MorrisPlugin.DEVELOPER_PREFIX + "_Morris_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"), 
                defaultRenderers,
                mainRenderer,
                model);

            skins.Add(defaultSkin);
            #endregion

            skinController.skins = skins.ToArray();
        }
    }
}