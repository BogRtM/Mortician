using UnityEngine;
using RoR2;
using EntityStates;
using EntityStates.Merc;
namespace Skillstates.Deputy
{
    internal class SkullCrackerDash : BaseState
    {
        public static float baseDuration = 0.2f;
        public static float speedCoefficient = 15f;

        private Ray aimRay;
        private Vector3 dashVector;
        public override void OnEnter()
        {
            base.OnEnter();

            aimRay = base.GetAimRay();
            dashVector = aimRay.direction;

            base.PlayAnimation("FullBody, Override", "Dash");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                base.characterMotor.rootMotion += dashVector * (speedCoefficient * moveSpeedStat * Time.fixedDeltaTime);
            }

            if(base.fixedAge >= baseDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.characterMotor.velocity *= 0.1f;
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
