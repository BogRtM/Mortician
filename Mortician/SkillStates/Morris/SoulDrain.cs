using UnityEngine;
using RoR2;
using EntityStates;
namespace Skillstates.Morris
{
    internal class SoulDrain : BaseState
    {
        public static float minDamageCoefficient = 1.5f;

        public HurtBox drainTarget;

        public override void OnEnter()
        {
            base.OnEnter();

            Chat.AddMessage("Casting soulDrain on: " + drainTarget.healthComponent.name);

            StartAimMode(2f, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            outer.SetNextStateToMain();
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
