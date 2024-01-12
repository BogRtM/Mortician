using UnityEngine;
using RoR2;
using EntityStates;
using EntityStates.Toolbot;
using System;
using System.Collections.Generic;
namespace Skillstates.Ghoul
{
    internal class LaunchedState : BaseState
    {
        public static float minDuration = 0.3f;
        public static float damageCoefficient = 4f;
        public static float launchPower = 80f;
        public static float yOffset = 0.1f;

        private OverlapAttack attack;

        public Vector3 launchVector;
        public string hitboxGroupName;

        private List<HurtBox> victims = new List<HurtBox>();

        private float duration;
        private float cachedAirControl;
        public override void OnEnter()
        {
            base.OnEnter();

            launchVector.y += yOffset;

            base.gameObject.layer = LayerIndex.fakeActor.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            cachedAirControl = base.characterMotor.airControl;
            base.characterMotor.airControl = 0.1f;

            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            if(base.isAuthority)
            {
                base.characterDirection.forward = launchVector;
                base.characterBody.isSprinting = true;
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.velocity += launchVector * launchPower;
            }

            Transform modelTransform = base.GetModelTransform();
            HitBoxGroup hitBoxGroup = new HitBoxGroup();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxGroupName);
            }

            attack = new OverlapAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.damageType = DamageType.BlightOnHit;
            attack.procCoefficient = 1f;
            attack.teamIndex = base.GetTeam();
            attack.isCrit = base.RollCrit();
            attack.forceVector = Vector3.zero;
            attack.pushAwayForce = 1f;
            attack.damage = damageCoefficient * base.damageStat;
            attack.hitBoxGroup = hitBoxGroup;
            attack.hitEffectPrefab = null;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.isAuthority)
            {
                attack.Fire(victims);

                foreach(HurtBox victim in victims)
                {
                    float victimMass = 0f;
                    if(victim.healthComponent)
                    {
                        CharacterMotor victimMotor = victim.healthComponent.GetComponent<CharacterMotor>();
                        if(victimMotor)
                        {
                            victimMass = victimMotor.mass;
                        }else
                        {
                            Rigidbody rigidBody = victim.healthComponent.GetComponent<Rigidbody>();
                            victimMass = rigidBody.mass;
                        }
                    }

                    if(victimMass >= ToolbotDash.massThresholdForKnockback)
                    {
                        this.outer.SetNextStateToMain();
                    }
                }
            }

            if(base.fixedAge >= minDuration && base.isAuthority && base.characterMotor.Motor.GroundingStatus.IsStableOnGround)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.characterMotor.velocity *= 0.05f;
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            base.characterMotor.airControl = cachedAirControl;

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
