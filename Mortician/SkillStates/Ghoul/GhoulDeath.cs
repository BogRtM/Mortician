using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using Morris.Components;
using Morris.Modules;
namespace SkillStates.Ghoul
{
    internal class GhoulDeath : GenericCharacterDeath
    {
        public static float duration = 0.375f;
        public static float sacrificedDamageCoefficient = 7f;
        public static float sacrificedRadius = 18f;
        public static float smallHopVelocity = 7f;

        private MorrisMinionController minionController;
        private bool sacrificed;

        private BlastAttack attack;
        private float radius;
        private bool hasExploded;

        public override void OnEnter()
        {
            base.OnEnter();

            minionController = base.gameObject.GetComponent<MorrisMinionController>();
            sacrificed = minionController.sacrificed;

            if (sacrificed)
            {
                base.PlayCrossfade("FullBody, Override", "Sacrificed", 0.1f);

                Transform modelTransform = base.GetModelTransform();
                TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = duration * 1.5f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = Assets.GhoulSacrificedMat;
                temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
            }
            else
            {
                RagdollController ragdollController = base.cachedModelTransform.GetComponent<RagdollController>();
                if (ragdollController)
                {
                    Vector3 vector = base.characterMotor.velocity + Vector3.up * 5f;
                    ragdollController.BeginRagdoll(vector);
                }
            }

            if(sacrificed && base.isAuthority)
            {
                base.characterMotor.velocity = Vector3.zero;
                base.SmallHop(base.characterMotor, smallHopVelocity);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= duration && !hasExploded)
            {   
                hasExploded = true;

                if (sacrificed)
                {
                    base.DestroyModel();
                    this.Explode();
                }

                if(NetworkServer.active)
                {
                    base.DestroyBodyAsapServer();
                }
            }
        }

        public void Explode()
        {
            EffectData effectData = new EffectData()
            {
                origin = base.characterBody.footPosition,
                scale = radius
            };
            EffectManager.SpawnEffect(Assets.GhoulSacrificeExplosion, effectData, true);

            if (base.isAuthority)
            {
                attack = new BlastAttack();
                attack.attacker = minionController.sacrificeOwner ? minionController.sacrificeOwner : minionController.owner;
                attack.inflictor = base.gameObject;
                attack.teamIndex = base.GetTeam();
                attack.baseDamage = sacrificedDamageCoefficient * base.damageStat;
                attack.crit = minionController.owner ? Util.CheckRoll(minionController.ownerBody.crit, minionController.ownerBody.master) : base.RollCrit();
                attack.procCoefficient = 1f;
                attack.damageType = DamageType.Generic;
                attack.baseForce = 0f;
                attack.position = base.transform.position;
                attack.radius = sacrificedRadius;
                attack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                attack.falloffModel = BlastAttack.FalloffModel.None;
                attack.impactEffect = EffectCatalog.FindEffectIndexFromPrefab(Assets.OmniImpactVFXGhoul);

                attack.Fire();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
