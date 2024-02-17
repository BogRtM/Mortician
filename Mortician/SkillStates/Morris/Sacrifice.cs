using UnityEngine;
using RoR2;
using RoR2.Orbs;
using EntityStates;
using Morris.Components;
using Morris;
using UnityEngine.Networking;
using Morris.Modules;

namespace SkillStates.Morris
{
    internal class Sacrifice : BaseState
    {
        public static float baseDuration = 1.333f;
        public static float sacrificePercentHealAmount = 0.15f;

        private LanternTracker lanternTracker;

        private float duration;
        private float effectTime;
        private float earlyExitTime;
        private bool hasSnapped;

        private HurtBox target;
        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / base.attackSpeedStat;
            effectTime = duration * 0.35f;
            earlyExitTime = duration * 0.625f;

            lanternTracker = base.GetComponent<LanternTracker>();
            target = lanternTracker.GetTrackingTarget();

            PlayCrossfade("Right Arm, Override", "FingerSnap", "Swing.playbackRate", duration, 0.05f);

            SacrificeGhoul(target);

            StartAimMode(2f, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= effectTime && !hasSnapped)
            {
                hasSnapped = true;
                EffectManager.SimpleMuzzleFlash(Assets.MorrisFingerSnap, base.gameObject, "IndexFingerR", false);
            }

            if(base.fixedAge >= duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public void SacrificeGhoul(HurtBox sacrificeTarget)
        {
            MorrisMinionController minionController = sacrificeTarget.healthComponent.GetComponent<MorrisMinionController>();
            HealOrb healOrb = new HealOrb();
            healOrb.origin = minionController.transform.position;
            healOrb.target = base.characterBody.mainHurtBox;
            healOrb.healValue = base.characterBody.maxHealth * sacrificePercentHealAmount;
            healOrb.overrideDuration = 0.5f;

            if (NetworkServer.active)
            {
                OrbManager.instance.AddOrb(healOrb);
            }

            minionController.Sacrifice(base.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return base.fixedAge >= earlyExitTime ? InterruptPriority.Any : InterruptPriority.PrioritySkill;
        }
    }
}
