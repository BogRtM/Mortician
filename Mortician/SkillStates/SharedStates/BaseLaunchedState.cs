using UnityEngine;
using RoR2;
using EntityStates;
using EntityStates.Toolbot;
using System;
using System.Collections.Generic;
using Morris;
using Morris.Components;
using UnityEngine.Networking;
using Morris.Modules;

namespace SkillStates.SharedStates
{
    internal class BaseLaunchedState : BaseState
    {
        public static float minDuration = 0.3f;
        public static float maxDuration = 5f;
        public static float damageCoefficient = 3.5f;
        public static float yOffset = 0.1f;
        public static float minMassToExitState = 200f;
        public static Vector3 downwardForce = Vector3.zero;
        public float launchPower;

        private MorrisMinionController minionController;

        private OverlapAttack attack;

        public Vector3 launchVector;

        private List<HurtBox> victims = new List<HurtBox>();

        private float cachedAirControl;

        protected GameObject impactVFX;

        public override void OnEnter()
        {
            base.OnEnter();

            PlayLaunchEntry();

            minionController = GetComponent<MorrisMinionController>();

            launchVector.y += yOffset;

            cachedAirControl = characterMotor.airControl;
            characterMotor.airControl = 0.15f;

            if (NetworkServer.active)
            {
                characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            if (isAuthority)
            {
                characterDirection.forward = launchVector;
                characterBody.isSprinting = true;
                characterMotor.Motor.ForceUnground();
                characterMotor.velocity = Vector3.zero;
                characterMotor.velocity += launchVector * launchPower;
            }

            Transform modelTransform = GetModelTransform();
            HitBoxGroup hitBoxGroup = new HitBoxGroup();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (element) => element.groupName == "LaunchHitbox");
            }

            attack = new OverlapAttack();
            attack.attacker = minionController.owner ? minionController.owner : gameObject;
            attack.inflictor = gameObject;
            attack.damageType = DamageType.Generic;
            attack.procCoefficient = 1f;
            attack.teamIndex = GetTeam();
            attack.isCrit = minionController.owner ? Util.CheckRoll(minionController.ownerBody.crit, minionController.ownerBody.master) : base.RollCrit();
            attack.forceVector = downwardForce;
            attack.pushAwayForce = 1f;
            attack.damage = damageCoefficient * damageStat;
            attack.hitBoxGroup = hitBoxGroup;
            attack.hitEffectPrefab = impactVFX;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                characterDirection.forward = characterMotor.velocity.normalized;

                if (attack.Fire(victims))
                {
                    foreach (HurtBox victim in victims)
                    {
                        if (victim.healthComponent && victim.healthComponent.alive)
                        {
                            Rigidbody victimRigidBody = victim.healthComponent.GetComponent<Rigidbody>();
                            if (victimRigidBody)
                            {
                                float victimMass = victimRigidBody.mass;

                                if (victimMass >= minMassToExitState)
                                {
                                    OnHitLargeEnemy(victim);
                                }
                            }
                            else
                            {
                                CharacterBody victimBody = victim.healthComponent.body;
                                if(victimBody.hullClassification == HullClassification.Golem || victimBody.hullClassification == HullClassification.BeetleQueen)
                                {
                                    OnHitLargeEnemy(victim);
                                }
                            }
                        }
                    }
                }

                if ((base.fixedAge >= minDuration && base.characterMotor.Motor.GroundingStatus.IsStableOnGround) || base.fixedAge >= maxDuration)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public virtual void PlayLaunchEntry()
        {

        }

        public virtual void PlayLaunchExit()
        {
            
        }

        public virtual void OnHitLargeEnemy(HurtBox target)
        {

        }

        public override void OnExit()
        {
            PlayLaunchExit();

            if (NetworkServer.active)
            {
                characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            characterMotor.velocity *= 0.05f;

            characterMotor.airControl = cachedAirControl;

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
