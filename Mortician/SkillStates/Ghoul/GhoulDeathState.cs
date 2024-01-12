using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using EntityStates;
using EntityStates.Croco;
using UnityEngine.Networking;
using Morris.Components;
namespace Skillstates.Ghoul
{
    internal class GhoulDeathState : GenericCharacterDeath
    {
        public static float duration = 0.3f;
        public static float baseDamageCoefficient = 2.5f;
        public static float sacrificedDamageCoefficient = 8f;
        public static float baseRadius = 8f;
        public static float sacrificedRadius = 16f;
        public static float smallHopVelocity = 5.5f;

        public static GameObject blastEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoLeapExplosion.prefab").WaitForCompletion();
        public static GameObject sacrificedEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BleedOnHitAndExplode/BleedOnHitAndExplode_Explosion.prefab").WaitForCompletion();

        private MorrisMinionController minionController;
        private bool sacrificed;

        private BlastAttack attack;

        private float damage;
        private float radius;
        private GameObject blastEffect;

        public override void OnEnter()
        {
            base.OnEnter();

            minionController = base.gameObject.GetComponent<MorrisMinionController>();
            sacrificed = minionController.sacrificed;

            damage = sacrificed ? sacrificedDamageCoefficient : baseDamageCoefficient;
            radius = sacrificed ? sacrificedRadius : baseRadius;
            blastEffect = sacrificed ? sacrificedEffectPrefab : blastEffectPrefab;

            if(sacrificed && base.isAuthority)
            {
                base.characterMotor.velocity = Vector3.zero;
                base.SmallHop(base.characterMotor, smallHopVelocity);
            }

            attack = new BlastAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = base.GetTeam();
            attack.baseDamage = damage * base.damageStat;
            attack.crit = base.RollCrit();
            attack.procCoefficient = 1f;
            attack.damageType = DamageType.BlightOnHit;
            attack.baseForce = 0f;
            attack.position = base.transform.position;
            attack.radius = radius;
            attack.attackerFiltering = AttackerFiltering.NeverHitSelf;
            attack.falloffModel = BlastAttack.FalloffModel.None;
            attack.impactEffect = EffectCatalog.FindEffectIndexFromPrefab(GhoulMelee.hitPrefab);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= duration)
            {
                base.DestroyModel();
                this.Explode();
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
                scale = baseRadius
            };
            EffectManager.SpawnEffect(blastEffect, effectData, true);

            if (NetworkServer.active)
            {
                attack.Fire();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
