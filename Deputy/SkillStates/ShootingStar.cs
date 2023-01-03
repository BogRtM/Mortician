using UnityEngine;
using RoR2;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using Deputy.Components;
using Deputy.Modules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Bindings;

namespace Skillstates.Deputy
{
    internal class ShootingStar : BaseState
    {
        public static float maxDuration = 2f;
        public static float minDuration;
        public static float damageCoefficient = 2f;
        public static int maxShots = 4;
        public static float baseFireInterval = 0.18f;
        public static float prepTime = 0.2f;
        public static float jumpPower = 35f;
        public static Vector3 jumpAngle = new Vector3(0f, 0.7f, 0f);

        private Animator modelAnimator;

        private BullseyeSearch search = new BullseyeSearch();
        private HurtBox bestCandidate;

        private float currentShots;
        private float fireIndex;
        private float fireTimer;
        private Vector3 jumpVector;
        private Vector3 shootVector = Vector3.down;

        private string shootLayer;
        private string shootAnimName;
        private string muzzleIndex;

        public override void OnEnter()
        {
            base.OnEnter();

            modelAnimator = base.GetModelAnimator();
            modelAnimator.SetLayerWeight(modelAnimator.GetLayerIndex("AimYaw"), 0f);

            base.PlayAnimation("FullBody, Override", "Shooting Star", "Flip.playbackRate", maxDuration);

            jumpVector = base.inputBank.moveVector == Vector3.zero ? base.characterDirection.forward : base.inputBank.moveVector;
            base.characterDirection.forward = jumpVector;

            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(jumpVector);
            effectData.origin = characterBody.footPosition;
            EffectManager.SpawnEffect(Assets.shootingStarEffect, effectData, false);

            jumpVector += jumpAngle;

            if (base.isAuthority)
            {
                base.characterMotor.velocity.y = 0f;
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.velocity += (jumpVector * jumpPower);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            fireTimer += Time.fixedDeltaTime;

            if (base.fixedAge >= prepTime && fireTimer >= baseFireInterval && currentShots < maxShots)
            {
                fireTimer = 0f;
                this.SearchForTarget();
                this.FireAttack();
            }

            if(base.fixedAge > minDuration &&
                (base.fixedAge >= maxDuration || (base.characterMotor.Motor.GroundingStatus.IsStableOnGround && !base.characterMotor.Motor.LastGroundingStatus.IsStableOnGround)) 
                && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }
        private void SearchForTarget()
        {
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = base.characterBody.corePosition;
            this.search.searchDirection = Vector3.down;
            this.search.sortMode = BullseyeSearch.SortMode.Distance;
            this.search.maxDistanceFilter = 50f;
            this.search.maxAngleFilter = 80f;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);
            this.bestCandidate = this.search.GetResults().FirstOrDefault<HurtBox>();
        }

        private void FireAttack()
        {
            currentShots++;
            Util.PlaySound(FireBarrage.fireBarrageSoundString, base.gameObject);

            if (bestCandidate)
            {
                if(bestCandidate.healthComponent.alive)
                    shootVector = (bestCandidate.transform.position - characterBody.corePosition).normalized;
            }

            if(fireIndex % 2 == 0)
            {
                shootLayer = "Left Arm, Additive";
                shootAnimName = "ShootL";
                muzzleIndex = "MuzzleL";
            }
            else
            {
                shootLayer = "Right Arm, Additive";
                shootAnimName = "ShootR";
                muzzleIndex = "MuzzleR";
            }

            base.PlayAnimation(shootLayer, shootAnimName, "Hand.playbackRate", baseFireInterval);
            EffectManager.SimpleMuzzleFlash(FirePistol2.muzzleEffectPrefab, base.gameObject, muzzleIndex, false);

            BulletAttack bulletAttack = new BulletAttack
            {
                owner = base.gameObject,
                weapon = base.gameObject,
                origin = base.characterBody.corePosition,
                aimVector = shootVector,
                minSpread = 0f,
                maxSpread = base.characterBody.spreadBloomAngle,
                damage = damageCoefficient * this.damageStat,
                force = FirePistol2.force,
                tracerEffectPrefab = Assets.deputyTracerEffect,
                muzzleName = muzzleIndex,
                hitEffectPrefab = FirePistol2.hitEffectPrefab,
                isCrit = base.RollCrit(),
                radius = 2f,
                smartCollision = true,
                damageType = DamageType.Stun1s,
                maxDistance = 80f,
                falloffModel = BulletAttack.FalloffModel.None
            };

            bulletAttack.Fire();

            fireIndex++;
        }

        public override void OnExit()
        {
            base.PlayAnimation("Fullbody, Override", "BufferEmpty");
            modelAnimator.SetLayerWeight(modelAnimator.GetLayerIndex("AimYaw"), 1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
