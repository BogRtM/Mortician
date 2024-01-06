using UnityEngine;
using RoR2;
using EntityStates;
namespace Skillstates.Morris
{
    internal class SkillTemplate : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            base.StartAimMode(2f, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.outer.SetNextStateToMain();
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
