using UnityEngine;
using RoR2;
using EntityStates;
namespace SkillStates.Ghoul
{
    internal class SpawnState : BaseState
    {
        public static float baseDuration = 0.5f;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= baseDuration && base.isAuthority) 
            {
                outer.SetNextStateToMain();
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
