using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using RoR2.Projectile;
using EntityStates;
using Morris.Modules;
using Morris.Components;
namespace SkillStates.Ghoul
{
    internal class BileSpit : BaseState
    {
        public static GameObject projectilePrefab = Projectiles.ghoulBilePrefab;
        public static GameObject muzzleEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/MuzzleflashCroco.prefab").WaitForCompletion();

        public static float baseDuration = 1f;
        public static float damageCoefficient = 1f;

        private MorrisMinionController minionController;

        private float duration;
        private float fireTime;
        private bool hasFired;
        public override void OnEnter()
        {
            base.OnEnter();

            minionController = base.GetComponent<MorrisMinionController>();

            duration = baseDuration / this.attackSpeedStat;
            fireTime = duration * 0.2f;

            base.PlayCrossfade("Gesture, Override", "BileSpit", "Attack.playbackRate", duration, 0.1f);

            StartAimMode(duration, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= fireTime && !hasFired)
            {
                hasFired = true;

                EffectManager.SimpleMuzzleFlash(muzzleEffect, base.gameObject, "UpperJaw", true);
                //Util.PlaySound("Play_flyingVermin_attack1_start", base.gameObject);

                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();

                    FireProjectileInfo FPI = new FireProjectileInfo();
                    FPI.crit = base.RollCrit();
                    FPI.damage = damageCoefficient * base.damageStat;
                    FPI.force = 100f;
                    FPI.owner = minionController.owner ? minionController.owner : base.gameObject; ;
                    FPI.position = aimRay.origin;
                    FPI.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
                    FPI.projectilePrefab = projectilePrefab;
                    ProjectileManager.instance.FireProjectile(FPI);
                }
            }

            if(base.fixedAge >= duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
