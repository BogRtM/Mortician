using UnityEngine;
using RoR2;
using EntityStates;
namespace SkillStates.Morris
{
    internal class SkillTemplate : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            StartAimMode(2f, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            outer.SetNextStateToMain();
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
