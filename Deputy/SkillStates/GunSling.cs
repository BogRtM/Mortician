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

            DAC.SetCombatWeight(0f);

            Chat.AddMessage("Leaving combat");
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
            return InterruptPriority.Skill;
        }
    }
}
