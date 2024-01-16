using UnityEngine;
using RoR2;
using EntityStates;
using EntityStates.Toolbot;
using System;
using System.Collections.Generic;
using Morris;
using Morris.Components;
using UnityEngine.Networking;

namespace SkillStates.SharedStates
{
    internal class BaseLaunchedState : BaseState
    {
        public static float minDuration = 0.3f;
        public static float damageCoefficient = 3.5f;
        public static float launchPower = 70f;
        public static float yOffset = 0.1f;
        public static float minMassToExitState = 200f;
        public static Vector3 downwardForce = Vector3.down * 10f;

        private MorrisMinionController minionController;

        private OverlapAttack attack;

        public Vector3 launchVector;

        private List<HurtBox> victims = new List<HurtBox>();

        private float duration;
        private float cachedAirControl;
        private bool selfIsGhoul;
        public override void OnEnter()
        {
            base.OnEnter();

            minionController = GetComponent<MorrisMinionController>();

            selfIsGhoul = characterBody.bodyIndex == MorrisPlugin.GhoulBodyIndex;

            launchVector.y += yOffset;

            gameObject.layer = LayerIndex.fakeActor.intVal;
            characterMotor.Motor.RebuildCollidableLayers();

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
            attack.isCrit = RollCrit();
            attack.forceVector = downwardForce;
            attack.pushAwayForce = 1f;
            attack.damage = damageCoefficient * damageStat;
            attack.hitBoxGroup = hitBoxGroup;
            attack.hitEffectPrefab = null;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority)
            {
                if (attack.Fire(victims))
                {
                    foreach (HurtBox victim in victims)
                    {
                        if (!victim || !victim.healthComponent || !victim.healthComponent.alive) continue;

                        float victimMass = 0f;
                        CharacterMotor victimMotor = victim.healthComponent.GetComponent<CharacterMotor>();
                        if (victimMotor)
                        {
                            victimMass = victimMotor.mass;
                        }
                        else
                        {
                            Rigidbody rigidBody = victim.healthComponent.GetComponent<Rigidbody>();
                            victimMass = rigidBody.mass;
                        }

                        if (victimMass >= minMassToExitState)
                        {
                            OnHitLargeEnemy(victim);
                        }
                    }
                }
            }

            if (fixedAge >= minDuration && isAuthority && characterMotor.Motor.GroundingStatus.IsStableOnGround)
            {
                outer.SetNextStateToMain();
            }
        }

        public virtual void OnHitLargeEnemy(HurtBox target)
        {

        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            characterMotor.velocity *= 0.05f;
            gameObject.layer = LayerIndex.defaultLayer.intVal;
            characterMotor.Motor.RebuildCollidableLayers();

            characterMotor.airControl = cachedAirControl;

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
