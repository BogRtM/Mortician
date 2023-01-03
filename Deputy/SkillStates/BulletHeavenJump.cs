using UnityEngine;
using RoR2;
using EntityStates;
namespace Skillstates.Deputy
{
    internal class BulletHeavenJump : BaseState
    {
        public static float baseDuration = 1f;
        public static float jumpPower = 30f;

        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayCrossfade("FullBody, Override", "BulletHeaven Jump", "Flip.playbackRate", baseDuration, 0.1f);

            if (base.isAuthority)
            {
                base.characterMotor.velocity.y = 0f;
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.velocity += Vector3.up * jumpPower;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= baseDuration && base.isAuthority)
            {
                this.outer.SetNextState(new BulletHeavenLoop());
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
