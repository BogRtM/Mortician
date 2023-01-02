using UnityEngine;
using RoR2;
using EntityStates;
using Deputy.Components;

namespace Skillstates.Deputy
{
    internal class GunSling : BaseState
    {
        public static float baseDuration = 0.1f;

        private DeputyAnimatorController DAC;

        public override void OnEnter()
        {
            base.OnEnter();
            DAC = base.GetComponent<DeputyAnimatorController>();

            DAC.SetCombatState(DeputyAnimatorController.combatState.LeavingCombat);
            float rightSpeed = base.GetModelAnimator().GetFloat("rightSpeed");
            float forwardSpeed = base.GetModelAnimator().GetFloat("forwardSpeed");
            Chat.AddMessage("forward: " + forwardSpeed + ", right: " + rightSpeed);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= baseDuration)
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
