﻿using UnityEngine;
using RoR2;
using R2API;
using EntityStates;
using EntityStates.Merc;
using EntityStates.Loader;
using Deputy.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using Deputy;

namespace Skillstates.Deputy
{
    internal class SkullCrackerDash : BaseState
    {
        public static float baseDuration = 0.3f;
        public static float speedCoefficient = 15f;
        public static float dashPower = 7f;
        public static float damageCoefficient = 8f;
        public static float pushAwayForce = 30f;
        public static float pushAwayYFactor = 0.5f;

        private Ray aimRay;
        private Vector3 dashVector;

        private OverlapAttack attack;
        private List<HurtBox> victimsStruck = new List<HurtBox>();

        private bool hasHit;
        private bool hasKnockedBack;

        public override void OnEnter()
        {
            base.OnEnter();

            aimRay = base.GetAimRay();
            dashVector = aimRay.direction;

            base.characterMotor.disableAirControlUntilCollision = false;

            Transform modelTransform = base.GetModelTransform();
            HitBoxGroup hitBoxGroup = null;

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Dash");

                TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = baseDuration * 2f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
            }

            this.attack = new OverlapAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.damageType = DamageType.Stun1s;
            attack.procCoefficient = 1f;
            attack.teamIndex = base.GetTeam();
            attack.isCrit = base.RollCrit();
            attack.forceVector = Vector3.zero;
            attack.pushAwayForce = 1f;
            attack.damage = damageCoefficient * damageStat * this.GetDamageBoostFromSpeed();
            attack.hitBoxGroup = hitBoxGroup;
            attack.hitEffectPrefab = BaseSwingChargedFist.overchargeImpactEffectPrefab;
            attack.AddModdedDamageType(DeputyPlugin.grantDeputyBuff);
            attack.AddModdedDamageType(DeputyPlugin.resetUtilityOnKill);

            EffectData effectData = new EffectData()
            {
                origin = base.characterBody.corePosition,
                rotation = Util.QuaternionSafeLookRotation(dashVector)
            };
            EffectManager.SpawnEffect(EvisDash.blinkPrefab, effectData, true);
            //Util.PlaySound(, base.gameObject);
            base.PlayAnimation("FullBody, Override", "Dash");

            base.characterMotor.velocity.y = 0f;
            base.characterMotor.velocity += dashVector * (dashPower * moveSpeedStat);

            if (NetworkServer.active)
            {
                characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                base.characterDirection.forward = dashVector;
                base.characterBody.isSprinting = true;

                if (attack.Fire(victimsStruck))
                {
                    hasHit = true;
                    base.characterMotor.Motor.ForceUnground();
                    Vector3 knockback = -base.characterDirection.forward;
                    knockback.y = pushAwayYFactor;
                    base.characterMotor.velocity = knockback * pushAwayForce;

                    SkullCrackerBounce nextState = new SkullCrackerBounce()
                    {
                        faceDirection = -knockback
                    };

                    this.outer.SetNextState(nextState);
                }

                if (base.fixedAge >= baseDuration)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        private float GetDamageBoostFromSpeed()
        {
            return Mathf.Max(1f, base.characterBody.moveSpeed / base.characterBody.baseMoveSpeed);
        }

        public override void OnExit()
        {
            if (!hasHit)
                base.characterMotor.velocity *= 0.2f;

            if (NetworkServer.active)
            {
                characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            base.PlayCrossfade("FullBody, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
