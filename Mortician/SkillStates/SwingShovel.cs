using UnityEngine;
using RoR2;
using EntityStates;
using EntityStates.Croco;
using RoR2.Skills;
using Morris.Modules;
using System;
namespace Skillstates.Morris
{
    internal class SwingShovel : BaseState, SteppedSkillDef.IStepSetter
    {
        public static float baseDuration = 1.667f;
        public static float damageCoefficient = 8f;
        public static float smallHopVelocity = 5.5f;

        private Animator animator;

        private float duration;
        private float step;
        private float fireTime;
        private float earlyExitTime;

        private bool hasFired;
        private bool hasHopped;

        public override void OnEnter()
        {
            base.OnEnter();

            animator = base.GetModelAnimator();

            duration = baseDuration / base.attackSpeedStat;
            fireTime = duration * 0.2f;
            earlyExitTime = duration * 0.54f;

            base.StartAimMode(2f, false);

            if(!animator.GetBool("isMoving") && base.characterMotor.isGrounded)
                base.PlayCrossfade("FullBody, Override", "Swing1", "Swing.playbackRate", duration, 0.05f);

            base.PlayCrossfade("Gesture, Override", "Swing1", "Swing.playbackRate", duration, 0.05f);
            
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= fireTime && !hasFired && base.isAuthority)
            {
                FireAttack();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public void FireAttack()
        {
            hasFired = true;

            Transform modelTransform = base.GetModelTransform();
            HitBoxGroup hitBoxGroup = null;

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Swing");
            }

            OverlapAttack attack = new OverlapAttack();
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

            if(attack.Fire())
            {
                if(!base.characterMotor.isGrounded && !hasHopped)
                {
                    hasHopped = true;
                    base.SmallHop(base.characterMotor, smallHopVelocity);
                }
            }
        }

        public override void OnExit()
        {
            base.PlayAnimation("FullBody, Override", "BufferEmpty");

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            InterruptPriority priority;
            priority = (base.fixedAge >= earlyExitTime) ? InterruptPriority.Any : InterruptPriority.Skill;
            return priority;
        }

        public void SetStep(int i)
        {
            step = i;
        }
    }
}
