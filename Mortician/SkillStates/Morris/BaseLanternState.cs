using UnityEngine;
using RoR2;
using EntityStates;
using Morris.Components;
using Morris;
namespace Skillstates.Morris
{
    internal class LanternSkillState : BaseState
    {
        public static float baseDuration = 0.7f;

        private LanternTracker lanternTracker;

        private float duration;

        private HurtBox target;
        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / base.attackSpeedStat;

            lanternTracker = base.GetComponent<LanternTracker>();
            target = lanternTracker.GetTrackingTarget();

            if(target.healthComponent.body.bodyIndex == MorrisPlugin.GhoulBodyIndex)
            {
                MorrisMinionController minionController = target.healthComponent.GetComponent<MorrisMinionController>();
                SacrificeGhoul(minionController);
            }
            else
            {
                Chat.AddMessage("That's not a ghoul");
            }

            StartAimMode(2f, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public void SacrificeGhoul(MorrisMinionController minionController)
        {
            minionController.Sacrifice();
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
