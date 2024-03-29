﻿using BepInEx.Configuration;
using Morris.Modules.Characters;
using Morris.Components;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using EntityStates;
using SkillStates.Ghoul;
using RoR2.CharacterAI;
using UnityEngine.Events;

namespace Morris.Modules.NPC
{
    internal class GhoulMinion : CharacterBase
    {
        public override string bodyName => "Ghoul";

        public const string Ghoul_PREFIX = MorrisPlugin.DEVELOPER_PREFIX + "_GHOUL_BODY_";
        //used when registering your survivor's language tokens
        //public override string survivorTokenPrefix => Morris_PREFIX;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "GhoulBody",
            bodyNameToken = MorrisPlugin.DEVELOPER_PREFIX + "_GHOUL_BODY_NAME",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texGhoulIcon"),
            //bodyColor = new Color(62f / 255f, 162f / 255f, 82f / 255f),
            bodyColor = new Color32(33, 255, 189, 255),

            crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/SimpleDotCrosshair.prefab").WaitForCompletion(),
            podPrefab = null,

            capsuleHeight = 2f,
            capsuleRadius = 0.6f,
            modelBasePosition = new Vector3(0f, -1f, 0f),

            maxHealth = 150f,
            healthGrowth = 45f,
            healthRegen = -10f,
            regenGrowth = -3f,
            moveSpeed = 10f,
            damage = StaticValues.morrisBaseDamage,
            damageGrowth = StaticValues.morrisDamageGrowth,
            armor = 20f,

            cameraPivotPosition = new Vector3(0f, 0f, 0f),
            cameraParamsDepth = -8f
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "GhoulMesh",
                    dontHotpoo = true
                }
        };

        //public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);
        public override Type characterSpawnState => typeof(GhoulSpawn);

        public override ItemDisplaysBase itemDisplays => null;

        //if you have more than one character, easily create a config to enable/disable them like this
        //public override ConfigEntry<bool> characterEnabledConfig => Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public static GameObject ghoulMasterPrefab;
        public static SpawnCard ghoulSpawnCard;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            MorrisPlugin.GhoulBodyPrefab = this.bodyPrefab;

            bodyPrefab.layer = LayerIndex.fakeActor.intVal;

            EntityStateMachine ghoulBodyESM = EntityStateMachine.FindByCustomName(bodyPrefab, "Body");

            CharacterBody characterBody = bodyPrefab.GetComponent<CharacterBody>();
            characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            var minionController = bodyPrefab.AddComponent<MorrisMinionController>();
            minionController.minionType = MorrisMinionController.MorrisMinionType.Ghoul;

            CharacterDeathBehavior CDB = bodyPrefab.GetComponent<CharacterDeathBehavior>();
            CDB.deathStateMachine = ghoulBodyESM;
            CDB.deathState = new SerializableEntityStateType(typeof(GhoulDeath));

            SfxLocator sfxLocator = bodyPrefab.GetComponent<SfxLocator>();
            sfxLocator.deathSound = null;
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;
            
            Transform meleeHitbox = childLocator.FindChild("MeleeHitbox");
            Modules.Prefabs.SetupHitbox(model, meleeHitbox, "GhoulMelee");

            Transform bodyHitbox = childLocator.FindChild("LaunchHitbox");
            Modules.Prefabs.SetupHitbox(model, bodyHitbox, "LaunchHitbox");
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateGhoulSkillFamilies(bodyPrefab);
            string prefix = MorrisPlugin.DEVELOPER_PREFIX;

            
            #region Primary

            SkillDef ghoulPrimary = ScriptableObject.CreateInstance<SkillDef>();
            ghoulPrimary.skillName = prefix + "_GHOUL_BODY_PRIMARY_MELEE_NAME";
            ghoulPrimary.skillNameToken = prefix + "_GHOUL_BODY_PRIMARY_MELEE_NAME";
            ghoulPrimary.skillDescriptionToken = prefix + "_GHOUL_BODY_PRIMARY_MELEE_DESCRIPTION";
            ghoulPrimary.icon = null;
            ghoulPrimary.activationState = new EntityStates.SerializableEntityStateType(typeof(GhoulMelee));
            ghoulPrimary.activationStateMachineName = "Weapon";
            ghoulPrimary.baseMaxStock = 1;
            ghoulPrimary.baseRechargeInterval = 1f;
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
                skillName = prefix + "_GHOUL_BODY_SECONDARY_SPIT_NAME",
                skillNameToken = prefix + "_GHOUL_BODY_SECONDARY_SPIT_NAME",
                skillDescriptionToken = prefix + "_GHOUL_BODY_SECONDARY_SPIT_DESCRIPTION",
                skillIcon = null,
                activationState = new EntityStates.SerializableEntityStateType(typeof(BileSpit)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
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
        protected override void InitializeCharacterMaster()
        {
            GameObject ghoulMasterPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianMaster.prefab").WaitForCompletion(),
                "GhoulMaster");
            CharacterMaster characterMaster = ghoulMasterPrefab.GetComponent<CharacterMaster>();
            characterMaster.bodyPrefab = this.bodyPrefab;

            BaseAI ghoulAI = ghoulMasterPrefab.GetComponent<BaseAI>();
            ghoulAI.neverRetaliateFriendlies = true;
            ghoulAI.aimVectorMaxSpeed *= 3f;
            ghoulAI.fullVision = true;

            InitializeSkillDrivers(ghoulMasterPrefab);

            GhoulMinion.ghoulMasterPrefab = ghoulMasterPrefab;
            Modules.Content.AddMasterPrefab(ghoulMasterPrefab);
            CreateSpawnCard();

            //Make ghoul a deployable
            if (Config.ghoulLimit.Value > 0)
            {
                Deployable deployable = ghoulMasterPrefab.AddComponent<Deployable>();
            }
        }

        private void CreateSpawnCard()
        {
            CharacterSpawnCard card = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            card.name = "cscMorrisGhoul";
            card.prefab = ghoulMasterPrefab;
            card.sendOverNetwork = true;
            card.hullSize = HullClassification.Human;
            card.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            card.requiredFlags = RoR2.Navigation.NodeFlags.None;
            card.forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn;
            card.eliteRules = SpawnCard.EliteRules.Default;
            card.occupyPosition = false;

            GhoulMinion.ghoulSpawnCard = card;
        }

        protected virtual void InitializeSkillDrivers(GameObject masterPrefab)
        {
            foreach (AISkillDriver i in masterPrefab.GetComponentsInChildren<AISkillDriver>())
            {
                UnityEngine.Object.DestroyImmediate(i);
            }

            AISkillDriver biteDriver = masterPrefab.AddComponent<AISkillDriver>();
            biteDriver.customName = "BiteOffNodeGraph";
            biteDriver.skillSlot = SkillSlot.Primary;
            biteDriver.requireSkillReady = true;
            biteDriver.minUserHealthFraction = float.NegativeInfinity;
            biteDriver.maxUserHealthFraction = float.PositiveInfinity;
            biteDriver.minTargetHealthFraction = float.NegativeInfinity;
            biteDriver.maxTargetHealthFraction = float.PositiveInfinity;
            biteDriver.minDistance = 0f;
            biteDriver.maxDistance = 4f;
            biteDriver.activationRequiresAimConfirmation = false;
            biteDriver.activationRequiresTargetLoS = false;
            biteDriver.selectionRequiresTargetLoS = true;
            biteDriver.maxTimesSelected = -1;
            biteDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            biteDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            biteDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            biteDriver.moveInputScale = 1f;
            biteDriver.ignoreNodeGraph = true;
            biteDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver spitDriver = masterPrefab.AddComponent<AISkillDriver>();
            spitDriver.customName = "SpitBile";
            spitDriver.skillSlot = SkillSlot.Secondary;
            spitDriver.requireSkillReady = true;
            spitDriver.minUserHealthFraction = float.NegativeInfinity;
            spitDriver.maxUserHealthFraction = float.PositiveInfinity;
            spitDriver.minTargetHealthFraction = float.NegativeInfinity;
            spitDriver.maxTargetHealthFraction = float.PositiveInfinity;
            spitDriver.minDistance = 0f;
            spitDriver.maxDistance = 40f;
            spitDriver.activationRequiresAimConfirmation = true;
            spitDriver.activationRequiresTargetLoS = true;
            spitDriver.selectionRequiresTargetLoS = true;
            spitDriver.maxTimesSelected = -1;
            spitDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            spitDriver.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            spitDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            spitDriver.moveInputScale = 1f;
            spitDriver.ignoreNodeGraph = false;
            spitDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver strafeDriver = masterPrefab.AddComponent<AISkillDriver>();
            strafeDriver.customName = "StrafeNearTarget";
            strafeDriver.skillSlot = SkillSlot.None;
            strafeDriver.requireSkillReady = true;
            strafeDriver.minUserHealthFraction = float.NegativeInfinity;
            strafeDriver.maxUserHealthFraction = float.PositiveInfinity;
            strafeDriver.minTargetHealthFraction = float.NegativeInfinity;
            strafeDriver.maxTargetHealthFraction = float.PositiveInfinity;
            strafeDriver.minDistance = 0f;
            strafeDriver.maxDistance = 5f;
            strafeDriver.activationRequiresAimConfirmation = false;
            strafeDriver.activationRequiresTargetLoS = false;
            strafeDriver.selectionRequiresTargetLoS = true;
            strafeDriver.maxTimesSelected = -1;
            strafeDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            strafeDriver.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            strafeDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            strafeDriver.moveInputScale = 1f;
            strafeDriver.ignoreNodeGraph = true;
            strafeDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver chaseNearDriver = masterPrefab.AddComponent<AISkillDriver>();
            chaseNearDriver.customName = "ChaseTargetClose";
            chaseNearDriver.skillSlot = SkillSlot.None;
            chaseNearDriver.requireSkillReady = true;
            chaseNearDriver.minUserHealthFraction = float.NegativeInfinity;
            chaseNearDriver.maxUserHealthFraction = float.PositiveInfinity;
            chaseNearDriver.minTargetHealthFraction = float.NegativeInfinity;
            chaseNearDriver.maxTargetHealthFraction = float.PositiveInfinity;
            chaseNearDriver.minDistance = 0f;
            chaseNearDriver.maxDistance = 10f;
            chaseNearDriver.activationRequiresAimConfirmation = false;
            chaseNearDriver.activationRequiresTargetLoS = false;
            chaseNearDriver.selectionRequiresTargetLoS = true;
            chaseNearDriver.maxTimesSelected = -1;
            chaseNearDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseNearDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chaseNearDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            chaseNearDriver.moveInputScale = 1f;
            chaseNearDriver.ignoreNodeGraph = true;
            chaseNearDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver chaseFarDriver = masterPrefab.AddComponent<AISkillDriver>();
            chaseFarDriver.customName = "PathFromAfar";
            chaseFarDriver.skillSlot = SkillSlot.None;
            chaseFarDriver.requireSkillReady = true;
            chaseFarDriver.minUserHealthFraction = float.NegativeInfinity;
            chaseFarDriver.maxUserHealthFraction = float.PositiveInfinity;
            chaseFarDriver.minTargetHealthFraction = float.NegativeInfinity;
            chaseFarDriver.maxTargetHealthFraction = float.PositiveInfinity;
            chaseFarDriver.minDistance = 0f;
            chaseFarDriver.maxDistance = float.PositiveInfinity;
            chaseFarDriver.activationRequiresAimConfirmation = false;
            chaseFarDriver.activationRequiresTargetLoS = false;
            chaseFarDriver.selectionRequiresTargetLoS = false;
            chaseFarDriver.maxTimesSelected = -1;
            chaseFarDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseFarDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chaseFarDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            chaseFarDriver.moveInputScale = 1f;
            chaseFarDriver.ignoreNodeGraph = false;
            chaseFarDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            chaseFarDriver.shouldSprint = true;

            AISkillDriver idleDriver = masterPrefab.AddComponent<AISkillDriver>();
            idleDriver.customName = "DoNothing";
            idleDriver.skillSlot = SkillSlot.None;
            idleDriver.requireSkillReady = true;
            idleDriver.minUserHealthFraction = float.NegativeInfinity;
            idleDriver.maxUserHealthFraction = float.PositiveInfinity;
            idleDriver.minTargetHealthFraction = float.NegativeInfinity;
            idleDriver.maxTargetHealthFraction = float.PositiveInfinity;
            idleDriver.minDistance = 0f;
            idleDriver.maxDistance = float.PositiveInfinity;
            idleDriver.activationRequiresAimConfirmation = false;
            idleDriver.activationRequiresTargetLoS = false;
            idleDriver.selectionRequiresTargetLoS = false;
            idleDriver.maxTimesSelected = -1;
            idleDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            idleDriver.movementType = AISkillDriver.MovementType.Stop;
            idleDriver.aimType = AISkillDriver.AimType.None;
            idleDriver.moveInputScale = 1f;
            idleDriver.ignoreNodeGraph = false;
            idleDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
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