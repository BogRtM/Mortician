using UnityEngine;
using RoR2;
using EntityStates;
namespace Skillstates.Deputy
{
    internal class DeputyMainState : GenericCharacterMain
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void UpdateAnimationParameters()
        {
            base.UpdateAnimationParameters();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
