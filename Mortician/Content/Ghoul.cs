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
using SkillStates.Ghoul;
using RoR2.CharacterAI;

namespace Morris.Modules.NPC
{
    internal class Ghoul : CharacterBase
    {
        public override string bodyName => "Ghoul";

        public const string Ghoul_PREFIX = MorrisPlugin.DEVELOPER_PREFIX + "_GHOUL_BODY_";
        //used when registering your survivor's language tokens
        //public override string survivorTokenPrefix => Morris_PREFIX;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "GhoulBody",
            bodyNameToken = MorrisPlugin.DEVELOPER_PREFIX + "_GHOUL_BODY_NAME",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texMorrisIcon"),
            //bodyColor = new Color(62f / 255f, 162f / 255f, 82f / 255f),
            bodyColor = new Color32(33, 255, 189, 255),

            crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/SimpleDotCrosshair.prefab").WaitForCompletion(),
            podPrefab = null,

            capsuleHeight = 2f,
            capsuleRadius = 0.6f,
            modelBasePosition = new Vector3(0f, -1f, 0f),

            maxHealth = 160f,
            healthGrowth = 48f,
            healthRegen = -6f,
            regenGrowth = -1.2f,
            damage = 14f,
            damageGrowth = 2.8f,
            armor = 20f,
            sortPosition = 1f,

            cameraPivotPosition = new Vector3(0f, 0f, 0f),
            cameraParamsDepth = -8f
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "GhoulMesh"
                }
        };

        //public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override ItemDisplaysBase itemDisplays => new MorrisItemDisplays();

        //if you have more than one character, easily create a config to enable/disable them like this
        //public override ConfigEntry<bool> characterEnabledConfig => Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            MorrisPlugin.GhoulBodyPrefab = this.bodyPrefab;

            CapsuleCollider capsule = bodyPrefab.GetComponent<CapsuleCollider>();
            capsule.radius = 0.6f;
            capsule.height = 2.35f;

            SpawnGhoul.placementCapsuleRadius = capsule.radius;
            SpawnGhoul.placementCapsuleHeight = capsule.height;
        }

        protected override void InitializeCharacterMaster()
        {
            GameObject ghoulMasterPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianMaster.prefab").WaitForCompletion(),
                "GhoulMaster");
            CharacterMaster characterMaster = ghoulMasterPrefab.GetComponent<CharacterMaster>();
            characterMaster.bodyPrefab = this.bodyPrefab;

            foreach (AISkillDriver i in ghoulMasterPrefab.GetComponentsInChildren<AISkillDriver>())
            {
                UnityEngine.Object.DestroyImmediate(i);
            }

            BaseAI ghoulAI = ghoulMasterPrefab.GetComponent<BaseAI>();
            ghoulAI.neverRetaliateFriendlies = true;

            SpawnGhoul.GhoulMasterPrefab = ghoulMasterPrefab;
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

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;
            
            Transform swingHitbox = childLocator.FindChild("MeleeHitbox");
            Modules.Prefabs.SetupHitbox(model, swingHitbox, "Melee");
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateGhoulSkillFamilies(bodyPrefab);
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

            SkillDef ghoulPrimary = ScriptableObject.CreateInstance<SkillDef>();
            ghoulPrimary.skillName = prefix + "_GHOUL_BODY_PRIMARY_SHOVEL_NAME";
            ghoulPrimary.skillNameToken = prefix + "_GHOUL_BODY_PRIMARY_SHOVEL_NAME";
            ghoulPrimary.skillDescriptionToken = prefix + "_GHOUL_BODY_PRIMARY_SHOVEL_DESCRIPTION";
            ghoulPrimary.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("QuickTriggerIcon");
            ghoulPrimary.activationState = new EntityStates.SerializableEntityStateType(typeof(GhoulMelee));
            ghoulPrimary.activationStateMachineName = "Body";
            ghoulPrimary.baseMaxStock = 1;
            ghoulPrimary.baseRechargeInterval = 0f;
            ghoulPrimary.beginSkillCooldownOnSkillEnd = false;
            ghoulPrimary.canceledFromSprinting = false;
            ghoulPrimary.forceSprintDuringState = false;
            ghoulPrimary.fullRestockOnAssign = true;
            ghoulPrimary.interruptPriority = EntityStates.InterruptPriority.Any;
            ghoulPrimary.resetCooldownTimerOnUse = false;
            ghoulPrimary.isCombatSkill = true;
            ghoulPrimary.mustKeyPress = false;
            ghoulPrimary.cancelSprintingOnActivation = true;
            ghoulPrimary.rechargeStock = 1;
            ghoulPrimary.requiredStock = 1;
            ghoulPrimary.stockToConsume = 1;


            Modules.Skills.AddPrimarySkills(bodyPrefab, ghoulPrimary);
            #endregion

            #region Secondary
            SkillDef ghoulSecondary = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_GHOUL_BODY_SECONDARY_GHOUL_NAME",
                skillNameToken = prefix + "_GHOUL_BODY_SECONDARY_GHOUL_NAME",
                skillDescriptionToken = prefix + "_GHOUL_BODY_SECONDARY_GHOUL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("GunSlingIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillTemplate)),
                activationStateMachineName = "Body",
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
            Modules.Skills.AddSecondarySkills(bodyPrefab, ghoulSecondary);
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