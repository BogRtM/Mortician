using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using Morris.Components;
using Morris.Modules;
namespace SkillStates.Ghoul
{
    internal class DeathState : GenericCharacterDeath
    {
        public static float duration = 0.375f;
        public static float sacrificedDamageCoefficient = 6f;
        public static float sacrificedRadius = 16f;
        public static float smallHopVelocity = 7f;

        public static GameObject sacrificedEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BleedOnHitAndExplode/BleedOnHitAndExplode_Explosion.prefab").WaitForCompletion();

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
            }
            else
            {
                RagdollController ragdollController = base.cachedModelTransform.GetComponent<RagdollController>();
                if (ragdollController)
                {
                    Vector3 vector = base.characterMotor.velocity + Vector3.up * 3f;
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
            EffectManager.SpawnEffect(sacrificedEffectPrefab, effectData, true);

            if (base.isAuthority)
            {
                attack = new BlastAttack();
                attack.attacker = minionController.sacrificeOwner ? minionController.sacrificeOwner : minionController.owner;
                attack.inflictor = base.gameObject;
                attack.teamIndex = base.GetTeam();
                attack.baseDamage = sacrificedDamageCoefficient * base.damageStat;
                attack.crit = base.RollCrit();
                attack.procCoefficient = 1f;
                attack.damageType = DamageType.Generic;
                attack.baseForce = 0f;
                attack.position = base.transform.position;
                attack.radius = sacrificedRadius;
                attack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                attack.falloffModel = BlastAttack.FalloffModel.None;
                attack.impactEffect = EffectCatalog.FindEffectIndexFromPrefab(Assets.OmniImpactVFXMorris);

                attack.Fire();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
