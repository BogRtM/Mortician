using UnityEngine;
using RoR2;
using EntityStates.SurvivorPod;
using EntityStates;
using UnityEngine.Networking;

namespace SkillStates.CoffinPod
{
    internal class Release : SurvivorPodBaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayAnimation("Base", "OpenDoor");

            if(NetworkServer.active && base.vehicleSeat && base.vehicleSeat.currentPassengerBody)
            {
                base.vehicleSeat.EjectPassenger(base.vehicleSeat.currentPassengerBody.gameObject);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(NetworkServer.active && !base.vehicleSeat.currentPassengerBody)
            {
                this.outer.SetNextStateToMain();
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
