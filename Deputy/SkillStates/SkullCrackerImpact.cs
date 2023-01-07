using UnityEngine;
using RoR2;
using EntityStates;
using EntityStates.Loader;
using UnityEngine.Networking;

namespace Skillstates.Deputy
{
    internal class SkullCrackerImpact : BaseState
    {
        public static float baseDuration = 1f;

        internal Vector3 impactPoint;

        public override void OnEnter()
        {
            base.OnEnter();

            base.StartAimMode(baseDuration, true);
            base.PlayAnimation("FullBody, Override", "Kick1", "Flip.playbackRate", baseDuration);

            if (impactPoint == null)
                impactPoint = base.transform.position;

            //EffectManager.SimpleImpactEffect(SwingZapFist.overchargeImpactEffectPrefab, impactPoint, base.characterDirection.forward, false);

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                base.characterBody.isSprinting = true;

                if (base.fixedAge >= baseDuration)
                {
                    this.outer.SetNextStateToMain();
                }
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
