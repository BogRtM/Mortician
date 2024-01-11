using BepInEx.Configuration;
using Morris.Modules.Characters;
using Morris.Components;
using Skillstates.Morris;
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
            capsuleRadius = 0.7f,
            modelBasePosition = new Vector3(0f, -1.6f, 0f),

            maxHealth = 200f,
            healthGrowth = 60f,
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
                    childName = "MorrisMesh"
                },
                new CustomRendererInfo
                {
                    childName = "NecklaceMesh"
                },
                new CustomRendererInfo
                {
                    childName = "ShawlMesh"
                },
                new CustomRendererInfo
                {
                    childName = "ShovelMesh"
                },
                new CustomRendererInfo
                {
                    childName = "LanternMesh"
                }
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override ItemDisplaysBase itemDisplays => new MorrisItemDisplays();

        //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            MorrisPlugin.MorrisBodyPrefab = this.bodyPrefab;

            Rigidbody rb = bodyPrefab.GetComponent<Rigidbody>();
            rb.mass = 300f;
            CharacterMotor cm = bodyPrefab.GetComponent<CharacterMotor>();
            cm.mass = 300f;
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
            
            /*
            SkillDef primarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_Morris_BODY_PRIMARY_SHOOT_NAME",
                skillNameToken = prefix + "_Morris_BODY_PRIMARY_SHOOT_NAME",
                skillDescriptionToken = prefix + "_Morris_BODY_PRIMARY_SHOOT_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texThrustIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillTemplate)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] {"KEYWORD_AGILE"}
            });
            */

            SteppedSkillDef MorrisPrimarySkillDef = ScriptableObject.CreateInstance<SteppedSkillDef>();
            MorrisPrimarySkillDef.skillName = prefix + "_MORRIS_BODY_PRIMARY_SHOVEL_NAME";
            MorrisPrimarySkillDef.skillNameToken = prefix + "_MORRIS_BODY_PRIMARY_SHOVEL_NAME";
            MorrisPrimarySkillDef.skillDescriptionToken = prefix + "_MORRIS_BODY_PRIMARY_SHOVEL_DESCRIPTION";
            MorrisPrimarySkillDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("QuickTriggerIcon");
            MorrisPrimarySkillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(SwingShovel));
            MorrisPrimarySkillDef.activationStateMachineName = "Weapon";
            MorrisPrimarySkillDef.baseMaxStock = 1;
            MorrisPrimarySkillDef.baseRechargeInterval = 0f;
            MorrisPrimarySkillDef.beginSkillCooldownOnSkillEnd = false;
            MorrisPrimarySkillDef.canceledFromSprinting = false;
            MorrisPrimarySkillDef.forceSprintDuringState = false;
            MorrisPrimarySkillDef.fullRestockOnAssign = true;
            MorrisPrimarySkillDef.interruptPriority = EntityStates.InterruptPriority.Any;
            MorrisPrimarySkillDef.resetCooldownTimerOnUse = false;
            MorrisPrimarySkillDef.isCombatSkill = true;
            MorrisPrimarySkillDef.mustKeyPress = false;
            MorrisPrimarySkillDef.cancelSprintingOnActivation = true;
            MorrisPrimarySkillDef.rechargeStock = 1;
            MorrisPrimarySkillDef.requiredStock = 1;
            MorrisPrimarySkillDef.stockToConsume = 1;


            Modules.Skills.AddPrimarySkills(bodyPrefab, MorrisPrimarySkillDef);
            #endregion

            #region Secondary
            SkillDef secondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_MORRIS_BODY_SECONDARY_GHOUL_NAME",
                skillNameToken = prefix + "_MORRIS_BODY_SECONDARY_GHOUL_NAME",
                skillDescriptionToken = prefix + "_MORRIS_BODY_SECONDARY_GHOUL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("GunSlingIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SpawnGhoul)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
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
                stockToConsume = 1
        });
            Modules.Skills.AddSecondarySkills(bodyPrefab, secondarySkillDef);
            #endregion

            #region Utility
            SkillDef utilitySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_MORRIS_BODY_UTILITY_LANTERN_NAME",
                skillNameToken = prefix + "_MORRIS_BODY_UTILITY_LANTERN_NAME",
                skillDescriptionToken = prefix + "_MORRIS_BODY_UTILITY_LANTERN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ShootingStarIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillTemplate)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] {"KEYWORD_STUNNING"}
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, utilitySkillDef);
            #endregion

            #region Special
            SkillDef skullCrackerSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_MORRIS_BODY_SPECIAL_TOMBSTONE_NAME",
                skillNameToken = prefix + "_MORRIS_BODY_SPECIAL_TOMBSTONE_NAME",
                skillDescriptionToken = prefix + "_MORRIS_BODY_SPECIAL_TOMBSTONE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("SkullBreakerIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillTemplate)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 8f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_HEAVY" }
            });
            Modules.Skills.AddSpecialSkills(bodyPrefab, skullCrackerSkillDef);
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