using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RoR2;
using System;
using Path = System.IO.Path;
using ShaderSwapper;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using RoR2.Orbs;

namespace Morris.Modules
{
    internal static class Assets
    {
        // the assetbundle to load assets from
        internal static AssetBundle mainAssetBundle;

        // networked hit sounds
        internal static NetworkSoundEventDef swordHitSoundEvent;

        // cache these and use to create our own materials
        internal static Shader hotpoo = RoR2.LegacyResourcesAPI.Load<Shader>("Shaders/Deferred/HGStandard");
        internal static Material commandoMat;
        private static string[] assetNames = new string[0];

        // CHANGE THIS
        private const string assetFolder = "MorrisAssets";
        private const string assetbundleName = "morrisassetbundle";
        private const string soundbankFolder = "MorrisSoundbanks";
        private const string soundbankName = "MorrisBank.bnk";
        //change this to your project's name if/when you've renamed it
        private const string csProjName = "Morris";

        public static GameObject LanternIndicator;
        public static GameObject ShovelSwingVFX;
        public static GameObject MorrisShovelHitGhoul;
        public static GameObject MorrisShovelHit;
        public static GameObject MorrisFingerSnap;

        public static Material GhoulSacrificedMat;
        public static GameObject GhoulSacrificeExplosion;
        public static GameObject OmniImpactVFXGhoul;

        public static Dictionary<int, GameObject> GhoulMeleeEffects = new Dictionary<int, GameObject>();
        public static GameObject GhoulBiteEffect;
        public static GameObject GhoulLickEffect;
        public static GameObject GhoulSlashEffect;

        public static GameObject TombstoneBlueprintsPrefab;
        public static Material TombstoneSpawnMat;
        public static GameObject SoulOrbActivatedEffect;
        public static GameObject SoulOrbTrailEffect;
        public static GameObject SoulOrbExplosion;
        public static GameObject OmniImpactVFXTombstone;

        public static string AssetBundlePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(MorrisPlugin.PInfo.Location), assetFolder, assetbundleName);
            }
        }

        public static string SoundBankPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(MorrisPlugin.PInfo.Location), soundbankFolder);
            }
        }

        internal static void Initialize()
        {
            if (assetbundleName == "myassetbundle")
            {
                Log.Error("AssetBundle name hasn't been changed. not loading any assets to avoid conflicts");
                return;
            }

            LoadAssetBundle();
            PopulateAssets();
        }

        internal static void LoadAssetBundle()
        {
            try
            {
                if (mainAssetBundle == null)
                {
                    mainAssetBundle = AssetBundle.LoadFromFile(AssetBundlePath);
                }

                if (mainAssetBundle)
                {
                    Log.Warning("Morris asset bundle loaded successfully");
                }
            }
            catch (Exception e)
            {
                Log.Error("Failed to load assetbundle. Make sure your assetbundle name is setup correctly\n" + e);
                return;
            }

            assetNames = mainAssetBundle.GetAllAssetNames();
        }

        internal static void LoadSoundbank()
        {
            Log.Info("Loading Morris soundbank now");

            var akResult = AkSoundEngine.AddBasePath(SoundBankPath);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Warning($"Added bank base path : {SoundBankPath}");
            }
            else
            {
                Log.Error(
                    $"Error adding base path : {SoundBankPath} " +
                    $"Error code : {akResult}");
            }

            akResult = AkSoundEngine.LoadBank(soundbankName, out _);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Warning($"Added bank : {soundbankName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {soundbankName} " +
                    $"Error code : {akResult}");
            }
        }

        internal static void PopulateAssets()
        {
            if (!mainAssetBundle)
            {
                Log.Error("There is no AssetBundle to load assets from.");
                return;
            }

            #region Morris
            //Lantern tracker
            LanternIndicator = mainAssetBundle.LoadAsset<GameObject>("LanternTrackingIndicator");

            //Shovel swing VFX
            ShovelSwingVFX = LoadEffect("MorrisSwing", true);

            //Shovel impact VFX
            MorrisShovelHit = LoadEffect("OmniImpactVFXMorris", "Morris_ShovelImpact");

            //Finger snap VFX
            MorrisFingerSnap = LoadEffect("HitsparkMorrisFinger", "Morris_FingerSnap", true);
            #endregion

            #region Ghoul
            //Ghoul sacrifice explosion VFX
            Material ghoulSphereMat = mainAssetBundle.LoadAsset<Material>("matGhoulExplosion");
            var newRampTex = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampTritone.png").WaitForCompletion();
            ghoulSphereMat.SetTexture("_RemapTex", newRampTex);

            GhoulSacrificeExplosion = LoadEffect("GhoulSacrificeExplosion", "Play_bleedOnCritAndExplode_explode");

            GhoulSacrificedMat = mainAssetBundle.LoadAsset<Material>("matGhoulSacrificed");

            //Ghoul impact VFX
            OmniImpactVFXGhoul = LoadEffect("OmniImpactVFXGhoul", "Play_acrid_m1_hit");

            //Ghoul melee swings VFX
            GhoulBiteEffect = LoadEffect("GhoulBite", true);
            GhoulLickEffect = LoadEffect("GhoulLick", true);
            GhoulSlashEffect = LoadEffect("GhoulSlash", true);
            
            GhoulMeleeEffects.Add(1, GhoulBiteEffect);
            GhoulMeleeEffects.Add(2, GhoulLickEffect);
            GhoulMeleeEffects.Add(3, GhoulSlashEffect);
            GhoulMeleeEffects.Add(4, GhoulSlashEffect);
            #endregion

            #region Tombstone
            OmniImpactVFXTombstone = LoadEffect("OmniImpactVFXTombstone", "Play_golem_step");

            //Tombstone blueprint mat
            TombstoneBlueprintsPrefab = mainAssetBundle.LoadAsset<GameObject>("mdlTombstoneBlueprint");
            BlueprintController blueprintController = TombstoneBlueprintsPrefab.GetComponent<BlueprintController>();
            blueprintController.okMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Engi/matBlueprintsOk.mat").WaitForCompletion();
            blueprintController.invalidMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Engi/matBlueprintsInvalid.mat").WaitForCompletion();

            TombstoneSpawnMat = mainAssetBundle.LoadAsset<Material>("matTombstoneSpawn");

            SoulOrbActivatedEffect = LoadEffect("SoulOrbActivatedEffect", "Play_item_proc_igniteOnKill");

            SoulOrbTrailEffect = mainAssetBundle.LoadAsset<GameObject>("SoulOrbEffect");
            SoulOrbTrailEffect.AddComponent<EffectComponent>();
            OrbEffect orbEffect = SoulOrbTrailEffect.AddComponent<OrbEffect>();
            orbEffect.startVelocity1 = new Vector3(-5, 16, -5);
            orbEffect.startVelocity2 = new Vector3(5, 8, 5);
            orbEffect.movementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            orbEffect.faceMovement = true;
            SoulOrbTrailEffect.AddComponent<Rigidbody>();
            VFXAttributes soulOrbVFX = SoulOrbTrailEffect.AddComponent<VFXAttributes>();
            soulOrbVFX.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            soulOrbVFX.vfxPriority = VFXAttributes.VFXPriority.Medium;
            AddNewEffectDef(SoulOrbTrailEffect);

            SoulOrbExplosion = LoadEffect("SoulOrbExplosion");
            GameObject orbExplosionLight = SoulOrbExplosion.transform.Find("Point Light").gameObject;
            LightIntensityCurve lightIntensityCurve = orbExplosionLight.AddComponent<LightIntensityCurve>();
            lightIntensityCurve.curve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
            lightIntensityCurve.timeMax = 0.3f;
            #endregion

            // feel free to delete everything in here and load in your own assets instead
            // it should work fine even if left as is- even if the assets aren't in the bundle
            /*
            swordHitSoundEvent = CreateNetworkSoundEventDef("HenrySwordHit");

            bombExplosionEffect = LoadEffect("BombExplosionEffect", "HenryBombExplosion");

            if (bombExplosionEffect)
            {
                ShakeEmitter shakeEmitter = bombExplosionEffect.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.5f;
                shakeEmitter.radius = 200f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 1f,
                    frequency = 40f,
                    cycleOffset = 0f
                };
            }

            swordSwingEffect = Assets.LoadEffect("HenrySwordSwingEffect", true);
            swordHitImpactEffect = Assets.LoadEffect("ImpactHenrySlash");
            */

        }

        private static GameObject CreateTracer(string originalTracerName, string newTracerName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName) == null) return null;

            GameObject newTracer = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName), newTracerName, true);

            if (!newTracer.GetComponent<EffectComponent>()) newTracer.AddComponent<EffectComponent>();
            if (!newTracer.GetComponent<VFXAttributes>()) newTracer.AddComponent<VFXAttributes>();
            if (!newTracer.GetComponent<NetworkIdentity>()) newTracer.AddComponent<NetworkIdentity>();

            newTracer.GetComponent<Tracer>().speed = 250f;
            newTracer.GetComponent<Tracer>().length = 50f;

            AddNewEffectDef(newTracer);

            return newTracer;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            Modules.Content.AddNetworkSoundEventDef(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            if (!objectToConvert) return;

            foreach (Renderer i in objectToConvert.GetComponentsInChildren<Renderer>())
            {
                i?.sharedMaterial?.SetHopooMaterial();
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].sharedMaterial,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }


        public static GameObject LoadSurvivorModel(string modelName) {
            GameObject model = mainAssetBundle.LoadAsset<GameObject>(modelName);
            if (model == null) {
                Log.Error("Trying to load a null model- " + modelName + " - check to see if the name in your code matches the name of the object in Unity");
                return null;
            }

            return PrefabAPI.InstantiateClone(model, model.name, false);
        }

        internal static Texture LoadCharacterIconGeneric(string characterName)
        {
            return mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair") == null) return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            bool assetExists = false;
            for (int i = 0; i < assetNames.Length; i++)
            {
                if (assetNames[i].Contains(resourceName.ToLowerInvariant()))
                {
                    assetExists = true;
                    i = assetNames.Length;
                }
            }

            if (!assetExists)
            {
                Log.Error("Failed to load effect: " + resourceName + " because it does not exist in the AssetBundle");
                return null;
            }

            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 1;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);

            return newEffect;
        }

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            Modules.Content.AddEffectDef(newEffectDef);
        }
    }
}