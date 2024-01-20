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

            base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
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
            base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
