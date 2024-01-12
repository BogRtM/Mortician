using UnityEngine;
using RoR2;
using R2API;
using EntityStates;
using RoR2.Skills;
using Morris;
using Morris.Modules;
using System;
using Morris.Components;
namespace Skillstates.Morris
{
    internal class SwingShovel : BaseState, SteppedSkillDef.IStepSetter
    {
        public static float baseDuration = 1.833f;
        public static float damageCoefficient = 6f;
        public static float smallHopVelocity = 5.5f;

        private Animator animator;

        private OverlapAttack attack;
        private HitBoxGroup hitBoxGroup;

        private float duration;
        private float step;
        private float fireTime;
        private float earlyExitTime;

        private bool hasFired;
        private bool hasHopped;

        private float hitPauseTimer;
        private bool inHitPause;

        public override void OnEnter()
        {
            base.OnEnter();

            animator = base.GetModelAnimator();

            duration = baseDuration / base.attackSpeedStat;
            fireTime = duration * 0.18f;
            earlyExitTime = duration * 0.54f;

            base.StartAimMode(2f, false);

            string swingIndex = "Swing" + step;

            if(!animator.GetBool("isMoving") && base.characterMotor.isGrounded)
                base.PlayCrossfade("FullBody, Override", swingIndex, "Swing.playbackRate", duration, 0.1f);

            base.PlayCrossfade("Gesture, Override", swingIndex, "Swing.playbackRate", duration, 0.1f);

            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Swing");
            }

            attack = new OverlapAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.damageType = DamageType.Generic;
            attack.procCoefficient = 1f;
            attack.teamIndex = base.GetTeam();
            attack.isCrit = base.RollCrit();
            attack.forceVector = Vector3.zero;
            attack.pushAwayForce = 1f;
            attack.damage = damageCoefficient * base.damageStat;
            attack.hitBoxGroup = hitBoxGroup;
            attack.hitEffectPrefab = null;
            attack.AddModdedDamageType(MorrisPlugin.LaunchGhoul);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= fireTime && !hasFired)
            {
                FireAttack();
            }

            if (base.fixedAge >= this.duration && hasFired && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public void FireAttack()
        {
            hasFired = true;

            if (base.isAuthority)
            {
                if (attack.Fire())
                {
                    if (!base.characterMotor.isGrounded && !hasHopped)
                    {
                        hasHopped = true;
                        base.SmallHop(base.characterMotor, smallHopVelocity);
                    }
                }

                HitGhoul(this.hitBoxGroup);
            }
        }

        public void HitGhoul(HitBoxGroup hitBoxGroup)
        {
            HurtBox hurtBox;

            Ray aimRay = base.GetAimRay();
            Vector3 launchVector = aimRay.direction;

            foreach(HitBox hitBox in hitBoxGroup.hitBoxes)
            {
                Transform transform = hitBox.transform;
                Vector3 position = transform.position;
                Vector3 halfExtent = transform.lossyScale * 0.5f;
                Quaternion rotation = transform.rotation;

                Collider[] hitObjects = Physics.OverlapBox(position, halfExtent, rotation, LayerIndex.defaultLayer.mask);

                for (int i = 0; i < hitObjects.Length; i++)
                {
                    MorrisMinionController launchComponent = hitObjects[i].GetComponent<MorrisMinionController>();
                    if (launchComponent && launchComponent.teamIndex == base.teamComponent.teamIndex)
                    {
                        launchComponent.Launch(launchVector);
                    }
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return (base.fixedAge >= earlyExitTime) ? InterruptPriority.Any : InterruptPriority.Skill;
        }

        public void SetStep(int i)
        {
            step = i;
        }
    }
}
