using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
namespace SkillStates.Ghoul
{
    internal class SpawnState : BaseState
    {
        public static float baseDuration = 0.5f;

        public override void OnEnter()
        {
            base.OnEnter();

            if(NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
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
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
