using UnityEngine;
using RoR2;
using RoR2.Orbs;
using EntityStates;
using Morris.Components;
using Morris;
namespace Skillstates.Morris
{
    internal class LanternSkillState : BaseState
    {
        public static float baseDuration = 0.7f;
        public static float sacrificePercentHealAmount = 0.2f;

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
            HealOrb healOrb = new HealOrb();
            healOrb.origin = minionController.transform.position;
            healOrb.target = base.characterBody.mainHurtBox;
            healOrb.healValue = base.characterBody.maxHealth * sacrificePercentHealAmount;
            healOrb.overrideDuration = 0.5f;
            OrbManager.instance.AddOrb(healOrb);

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
