using UnityEngine;
using RoR2;
using EntityStates;
namespace Skillstates.Deputy
{
    internal class DeputyMainState : GenericCharacterMain
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.characterBody.isSprinting && !aimAnimator.fullYaw)
            {
                aimAnimator.fullYaw = true;
            } else if(!base.characterBody.isSprinting && aimAnimator.fullYaw)
            {
                aimAnimator.fullYaw = false;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void UpdateAnimationParameters()
        {
            base.UpdateAnimationParameters();

            if(base.inputBank.moveVector != Vector3.zero)
            {
                base.modelAnimator.SetFloat("movingPitch", 1f, 0.1f, Time.deltaTime);
            } else
            {
                base.modelAnimator.SetFloat("movingPitch", 0f);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
