using UnityEngine;
using RoR2;
using EntityStates;
using EntityStates.Bandit2.Weapon;
using Deputy.Components;

namespace Skillstates.Deputy
{
    internal class GunSling : BaseState
    {
        public static float baseDuration = 1f;
        public static float basePrepTime = 0.5f;

        private GameObject gunsMesh;

        private DeputyAnimatorController DAC;
        private Animator modelAnimator;

        private float duration;
        private float prepTime;
        private bool hasFired;

        public override void OnEnter()
        {
            base.OnEnter();
            DAC = base.GetComponent<DeputyAnimatorController>();

            duration = baseDuration / attackSpeedStat;
            prepTime = duration * basePrepTime;

            gunsMesh = base.FindModelChild("GunsMesh").gameObject;

            modelAnimator = base.GetModelAnimator();
            modelAnimator.SetLayerWeight(modelAnimator.GetLayerIndex("AimYaw"), 0f);

            DAC.SetCombatState(DeputyAnimatorController.combatState.LeavingCombat);

            if (!characterMotor.isGrounded)
            {
                characterMotor.velocity.y = 0f;
                base.SmallHop(characterMotor, 5.5f);
            }

            Util.PlaySound("Play_bandit2_R_load", base.gameObject);

            base.PlayCrossfade("FullBody, Override", "Gun Sling", "Hand.playbackRate", duration, 0.1f * duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.StartAimMode(0.1f, false);

            if(base.fixedAge >= prepTime && !hasFired)
            {
                ThrowGuns();
            }

            if (base.fixedAge >= duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public void ThrowGuns()
        {
            hasFired = true;
            gunsMesh.SetActive(false);
        }

        public override void OnExit()
        {
            gunsMesh.SetActive(true);
            modelAnimator.SetLayerWeight(modelAnimator.GetLayerIndex("AimYaw"), 1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
