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
using SkillStates.Tombstone;
using RoR2.CharacterAI;

namespace Morris.Modules.NPC
{
    internal class TombstoneDeployable : CharacterBase
    {
        public override string bodyName => "Tombstone";

        public const string Tombstone_PREFIX = MorrisPlugin.DEVELOPER_PREFIX + "_TOMBSTONE_BODY_";
        //used when registering your survivor's language tokens
        //public override string survivorTokenPrefix => Morris_PREFIX;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "TombstoneBody",
            bodyNameToken = MorrisPlugin.DEVELOPER_PREFIX + "_TOMBSTONE_BODY_NAME",

            bodyNameToClone = "EngiWalkerTurret",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texTombstoneIcon"),
            //bodyColor = new Color(62f / 255f, 162f / 255f, 82f / 255f),
            bodyColor = new Color32(33, 255, 189, 255),

            crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/SimpleDotCrosshair.prefab").WaitForCompletion(),
            podPrefab = null,

            capsuleHeight = 7f,
            capsuleRadius = 1f,
            modelBasePosition = new Vector3(0f, -2.5f, 0f),

            maxHealth = 200f,
            healthGrowth = 66f,
            healthRegen = 2.5f,
            regenGrowth = 0.5f,
            moveSpeed = 10f,
            jumpCount = 0,
            jumpPower = 0,
            damage = StaticValues.morrisBaseDamage,
            damageGrowth = StaticValues.morrisDamageGrowth,
            armor = 20f,

            cameraPivotPosition = new Vector3(0f, 0.5f, 0f),
            cameraParamsDepth = -12f,
            hasAimAnimator = false
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "TombstoneMesh",
                    dontHotpoo = true,
                }
        };

        //public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(TombstoneMain);
        public override Type characterSpawnState => typeof(TombstoneSpawn);

        public override ItemDisplaysBase itemDisplays => new MorrisItemDisplays();

        //if you have more than one character, easily create a config to enable/disable them like this
        //public override ConfigEntry<bool> characterEnabledConfig => Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();

            bodyPrefab.gameObject.layer = LayerIndex.defaultLayer.intVal;

            MorrisPlugin.TombstoneBodyPrefab = this.bodyPrefab;

            CharacterBody characterBody = bodyPrefab.GetComponent<CharacterBody>();
            characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            var minionController = bodyPrefab.AddComponent<MorrisMinionController>();
            minionController.minionType = MorrisMinionController.MorrisMinionType.Tombstone;

            UnityEngine.Object.DestroyImmediate(bodyPrefab.GetComponent<AkEvent>());

            bodyPrefab.AddComponent<TombstoneController>();

            /*
            bodyPrefab.AddComponent<TeamFilter>();
            
            BuffWard buffWard = bodyPrefab.AddComponent<BuffWard>();
            buffWard.shape = BuffWard.BuffWardShape.Sphere;
            buffWard.radius = 30f;
            buffWard.interval = 0.5f;
            buffWard.buffDuration = 1f;
            buffWard.rangeIndicator = bodyPrefab.GetComponentInChildren<ChildLocator>().FindChild("SphereIndicator");
            buffWard.buffDef = Buffs.exhaustionDebuff;
            buffWard.invertTeamFilter = true;
            */

            Rigidbody rigidBody = bodyPrefab.GetComponent<Rigidbody>();
            rigidBody.mass = 300f;
            CharacterMotor characterMotor = bodyPrefab.GetComponent<CharacterMotor>();
            characterMotor.muteWalkMotion = true;
            characterMotor.mass = 300f;

            SfxLocator sfxLocator = bodyPrefab.GetComponent<SfxLocator>();
            sfxLocator.deathSound = null;

            
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            Transform bodyHitbox = childLocator.FindChild("LaunchHitbox");
            Modules.Prefabs.SetupHitbox(model, bodyHitbox, "LaunchHitbox");
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateTombstoneSkillFamilies(bodyPrefab);
            string prefix = MorrisPlugin.DEVELOPER_PREFIX;

        }
        protected override void InitializeCharacterMaster()
        {
            GameObject tombstoneMasterPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianMaster.prefab").WaitForCompletion(),
                "TombstoneMaster");
            CharacterMaster characterMaster = tombstoneMasterPrefab.GetComponent<CharacterMaster>();
            characterMaster.bodyPrefab = this.bodyPrefab;

            UnityEngine.Object.DestroyImmediate(tombstoneMasterPrefab.GetComponent<BaseAI>());

            foreach (AISkillDriver i in tombstoneMasterPrefab.GetComponentsInChildren<AISkillDriver>())
            {
                UnityEngine.Object.DestroyImmediate(i);
            }

            /*
            BaseAI tombstoneAI = tombstoneMasterPrefab.GetComponent<BaseAI>();
            tombstoneAI.neverRetaliateFriendlies = true;
            tombstoneAI.aimVectorMaxSpeed *= 2f;
            tombstoneAI.fullVision = true;
            */

            //InitializeSkillDrivers(tombstoneMasterPrefab);

            PlaceTombstone.tombstoneMasterPrefab = tombstoneMasterPrefab;
            Modules.Content.AddMasterPrefab(tombstoneMasterPrefab);
        }

        protected virtual void InitializeSkillDrivers(GameObject masterPrefab)
        {
            foreach (AISkillDriver i in masterPrefab.GetComponentsInChildren<AISkillDriver>())
            {
                UnityEngine.Object.DestroyImmediate(i);
            }

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